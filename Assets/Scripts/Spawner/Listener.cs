using UnityEngine;
using System.Collections.Generic;

// This attribute ensures that an AudioSource component is automatically added to the GameObject.
[RequireComponent(typeof(AudioSource))]
public class Listener : MonoBehaviour
{

    [SerializeField] private GameEvent spawnNote;
    
    [Header("Audio Analysis Settings")]
    [Tooltip("The volume threshold in decibels (dB) to trigger logging. Negative values are quieter.")]
    public float loudnessThreshold = -30f;

    [Tooltip("The quality of the spectral analysis. Higher values are more accurate but use more CPU. Must be a power of 2 (e.g., 512, 1024, 2048).")]
    public int fftSampleSize = 2048;

    [SerializeField, Tooltip("Margin of error in hertz for note detection (e.g., 10 = +/-10 Hz)")]
    private float noteMarginHz = 10f;

    private AudioSource audioSource;
    private string microphoneDevice;
    private AudioClip micClip;

    // Data buffers for audio analysis
    private float[] clipSampleData;
    private float[] spectrumData;
    private int sampleRate;

    // --- State variables for the listening window ---
    private bool isListening = false;
    private float listenTimer = 0f;
    private List<float> windowFrequencies = new List<float>();
    private List<float> windowLoudnesses = new List<float>();
    [SerializeField] private float listeningDuration = 0.1f;
    [SerializeField] private float ignoreDuration = 0.1f; // Ignore the first 0.1s of the window

    // All notes with octaves from D4 to B5 (MIDI 62 to 83)
    private static readonly string[] allNotes = new string[] {
        "D4","D#4","E4","F4","F#4","G4","G#4","A4","A#4","B4",
        "C5","C#5","D5","D#5","E5","F5","F#5","G5","G#5","A5","A#5","B5"
    };

    // Predefined frequencies for D4 to B5 (MIDI 62 to 83)
    private static readonly float[] noteFrequencies = new float[] {
        145.00f, 153.68f, 162.87f, 172.62f, 182.97f, 193.97f, 205.67f, 218.13f, 231.41f, 245.57f,
        260.68f, 276.80f, 293.99f, 312.33f, 331.89f, 352.75f, 374.99f, 398.70f, 423.97f, 450.89f, 479.57f, 510.12f
    };

    void Start()
    {
        // Get the required AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Check if any microphones are available
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found! Please connect a microphone.");
            this.enabled = false; // Disable the script if no mic is found
            return;
        }

        // Use the default microphone and start recording
        microphoneDevice = Microphone.devices[0];
        Debug.Log("Using microphone: " + microphoneDevice);

        // Get the sample rate of the system's audio output for accuracy
        sampleRate = AudioSettings.outputSampleRate;
        
        // Initialize our data buffers
        clipSampleData = new float[fftSampleSize];
        spectrumData = new float[fftSampleSize];
        
        // Start a 10-second looping clip from the microphone
        micClip = Microphone.Start(microphoneDevice, true, 10, sampleRate);
        audioSource.clip = micClip;
        audioSource.loop = true;

        // Wait until the microphone has positioned itself
        while (!(Microphone.GetPosition(microphoneDevice) > 0)) { }
        
        // Play the audio from the microphone into the AudioSource
        audioSource.Play();
        Debug.Log("Microphone is now listening.");
    }

    void Update()
    {
        float currentLoudness = GetLoudnessFromMic();
        // --- Logic for entering the listening window ---
        if (!isListening && currentLoudness >= loudnessThreshold)
        {
            isListening = true;
            listenTimer = 0f;
            windowFrequencies.Clear();
            windowLoudnesses.Clear();
        }

        // --- Logic while inside the listening window ---
        if (isListening)
        {
            listenTimer += Time.deltaTime;

            // Only collect data after the ignoreDuration (attack phase)
            if (listenTimer > ignoreDuration)
            {
                float freq = GetPitchFromAutocorrelation();
                windowFrequencies.Add(freq);
                windowLoudnesses.Add(currentLoudness);
            }

            // --- Logic for when the listening window ends ---
            if (listenTimer >= listeningDuration)
            {
                // The window is over. Log the median frequency and its decibel
                float medianFreq = 0f;
                float medianDb = 0f;
                if (windowFrequencies.Count > 0)
                {
                    windowFrequencies.Sort();
                    int mid = windowFrequencies.Count / 2;
                    if (windowFrequencies.Count % 2 == 0)
                        medianFreq = (windowFrequencies[mid - 1] + windowFrequencies[mid]) / 2f;
                    else
                        medianFreq = windowFrequencies[mid];
                    // Find the decibel value closest to the median frequency
                    float minDiff = float.MaxValue;
                    for (int i = 0; i < windowFrequencies.Count; i++)
                    {
                        float diff = Mathf.Abs(windowFrequencies[i] - medianFreq);
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            medianDb = windowLoudnesses[i];
                        }
                    }
                }
                // Only log at the end of the window
                Debug.Log($"Frequency: {medianFreq:F2} Hz");
                // Find the closest note in the array
                int closestIndex = -1;
                float minNoteDiff = float.MaxValue;
                for (int i = 0; i < noteFrequencies.Length; i++)
                {
                    float diff = Mathf.Abs(medianFreq - noteFrequencies[i]);
                    if (diff < minNoteDiff)
                    {
                        minNoteDiff = diff;
                        closestIndex = i;
                    }
                }
                // Use margin in Hz for comparison
                if (closestIndex != -1 && minNoteDiff <= noteMarginHz)
                {
                    string noteName = allNotes[closestIndex];
                    Debug.Log($"Note: {noteName}");
                    spawnNote.Raise(this, noteName);
                }
                else
                {
                    Debug.Log("Note: Unknown");
                }
                // Reset for the next trigger
                isListening = false;
                listenTimer = 0f;
            }
        }
    }

    private float GetLoudnessFromMic()
    {
        int micPosition = Microphone.GetPosition(microphoneDevice);
        if (micPosition < clipSampleData.Length)
            return -160f; // Not enough data yet

        int startPosition = micPosition - clipSampleData.Length;
        if (startPosition < 0)
        {
            // Buffer wraps around, so get the end and the beginning
            int tail = -startPosition;
            micClip.GetData(clipSampleData, micClip.samples - tail);
            micClip.GetData(clipSampleData, 0);
        }
        else
        {
            micClip.GetData(clipSampleData, startPosition);
        }

        float sum = 0f;
        for (int i = 0; i < clipSampleData.Length; i++)
        {
            sum += clipSampleData[i] * clipSampleData[i];
        }
        float rmsValue = Mathf.Sqrt(sum / clipSampleData.Length);
        if (rmsValue < 1e-5f) rmsValue = 1e-5f;
        float dbValue = 20 * Mathf.Log10(rmsValue);
        return dbValue;
    }


    private float GetPitchFromMic()
    {
        // Use the latest filled clipSampleData buffer
        // Apply a window function to reduce spectral leakage
        SimpleFFT.ApplyHanningWindow(clipSampleData);

        // Compute the FFT and get the spectrum
        float[] spectrum = SimpleFFT.ComputeFFT(clipSampleData);

        int maxIndex = 0;
        float maxVal = 0f;
        for (int i = 0; i < spectrum.Length; i++)
        {
            if (spectrum[i] > maxVal)
            {
                maxVal = spectrum[i];
                maxIndex = i;
            }
        }

        // Convert the index of the peak to frequency
        float freq = maxIndex * (float)sampleRate / clipSampleData.Length;
        return freq;
    }

    // Returns the frequency with the highest FFT peak and outputs the peak magnitude
    private float GetPeakFrequencyFromFFT(out float peakMagnitude)
    {
        SimpleFFT.ApplyHanningWindow(clipSampleData);
        float[] spectrum = SimpleFFT.ComputeFFT(clipSampleData);
        float maxVal = 0f;
        int maxIndex = 0;
        for (int i = 0; i < spectrum.Length; i++)
        {
            if (spectrum[i] > maxVal)
            {
                maxVal = spectrum[i];
                maxIndex = i;
            }
        }
        peakMagnitude = maxVal;
        // Parabolic interpolation for sub-bin accuracy
        float trueIndex = maxIndex;
        if (maxIndex > 0 && maxIndex < spectrum.Length - 1)
        {
            float left = spectrum[maxIndex - 1];
            float center = spectrum[maxIndex];
            float right = spectrum[maxIndex + 1];
            float denominator = left - 2 * center + right;
            if (Mathf.Abs(denominator) > 1e-6f)
            {
                float p = 0.5f * (left - right) / denominator;
                trueIndex = maxIndex + p;
            }
        }
        float freq = trueIndex * (float)sampleRate / clipSampleData.Length;
        return freq;
    }

    // --- Autocorrelation-based pitch detection ---
    private float GetPitchFromAutocorrelation()
    {
        int n = clipSampleData.Length;
        float[] data = clipSampleData;
        int minLag = sampleRate / 1000;
        int maxLagLimit = sampleRate / 50;
        float mean = 0f;
        for (int i = 0; i < n; i++) mean += data[i];
        mean /= n;
        for (int i = 0; i < n; i++) data[i] -= mean;
        float bestLag = 0;
        float bestValue = float.MinValue;
        float zeroLag = 0f;
        for (int i = 0; i < n; i++) zeroLag += data[i] * data[i];
        for (int lag = minLag; lag < maxLagLimit; lag++)
        {
            float sum = 0f;
            for (int i = 0; i < n - lag; i++)
                sum += data[i] * data[i + lag];
            if (sum > bestValue)
            {
                bestValue = sum;
                bestLag = lag;
            }
        }
        // Only accept if the peak is at least 30% of the zero-lag value (adjust as needed)
        if (bestLag > 0 && bestValue > 0.3f * zeroLag)
            return sampleRate / bestLag;
        else
            return 0f;
    }

    #region FFT Implementation
    // --- Simple FFT Implementation ---
    private static class SimpleFFT
    {
        public static void ApplyHanningWindow(float[] data)
        {
            int n = data.Length;
            for (int i = 0; i < n; i++)
            {
                data[i] *= 0.5f * (1 - Mathf.Cos(2 * Mathf.PI * i / (n - 1)));
            }
        }

        public static float[] ComputeFFT(float[] data)
        {
            int n = data.Length;
            Complex[] buffer = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                buffer[i] = new Complex(data[i], 0);
            }
            FFT(buffer);
            float[] mag = new float[n / 2];
            for (int i = 0; i < mag.Length; i++)
            {
                mag[i] = (float)buffer[i].Magnitude;
            }
            return mag;
        }

        private static void FFT(Complex[] buffer)
        {
            int n = buffer.Length;
            if (n <= 1) return;

            // Radix-2 Cooley-Tukey FFT
            Complex[] even = new Complex[n / 2];
            Complex[] odd = new Complex[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                even[i] = buffer[i * 2];
                odd[i] = buffer[i * 2 + 1];
            }
            FFT(even);
            FFT(odd);

            for (int k = 0; k < n / 2; k++)
            {
                Complex t = Complex.FromPolar(1.0, -2 * Mathf.PI * k / n) * odd[k];
                buffer[k] = even[k] + t;
                buffer[k + n / 2] = even[k] - t;
            }
        }

        public struct Complex
        {
            public double Real, Imag;

            public Complex(double r, double i) { Real = r; Imag = i; }
            public double Magnitude => System.Math.Sqrt(Real * Real + Imag * Imag);

            public static Complex operator +(Complex a, Complex b) => new Complex(a.Real + b.Real, a.Imag + b.Imag);
            public static Complex operator -(Complex a, Complex b) => new Complex(a.Real - b.Real, a.Imag - b.Imag);
            public static Complex operator *(Complex a, Complex b) => new Complex(a.Real * b.Real - a.Imag * b.Imag, a.Real * b.Imag + a.Imag * b.Real);
            public static Complex FromPolar(double r, double theta) => new Complex(r * System.Math.Cos(theta), r * System.Math.Sin(theta));
        }
    }
    #endregion

    #region Median Filter Logic
    // Converts a frequency in Hz to the closest note name and octave (e.g., "E2")
    private string FrequencyToNoteName(float frequency)
    {
        if (frequency <= 0f) return "Unknown";
        int noteNumber = Mathf.RoundToInt(12f * Mathf.Log(frequency / 440f, 2f) + 69f);
        noteNumber = Mathf.Clamp(noteNumber, 62, 83); // Clamp to D4..B5
        return allNotes[noteNumber - 62];
    }
    #endregion
}