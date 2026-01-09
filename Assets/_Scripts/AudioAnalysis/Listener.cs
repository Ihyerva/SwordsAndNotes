using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Listener : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameEvent spawnNote;

    [Header("Audio Settings")]
    [Tooltip("Minimum volume to start detection. -50dB is sensitive, -30dB is standard.")]
    public float loudnessThreshold = -50f;
    
    [Tooltip("FFT Resolution. 4096 is recommended for accurate low-note detection (E4).")]
    public int fftSampleSize = 4096; 
    
    [SerializeField, Tooltip("Allowed error in Hz when matching a note.")]
    private float noteMarginHz = 15f;

    [Header("Harmonic Settings (The Fix)")]
    [Tooltip("A peak must be at least this % of the maximum volume to be considered a note. 0.3 = 30%.")]
    [Range(0.1f, 0.9f)]
    public float harmonicThreshold = 0.30f; 

    [Header("Stability Settings")]
    [SerializeField] private float listeningDuration = 0.12f;
    [SerializeField] private float ignoreAttackTime = 0.05f;
    [SerializeField] private float noteCooldown = 0.75f;

    private AudioSource audioSource;
    private AudioClip micClip;
    private string microphoneDevice;
    private SimpleFFT simpleFFT = new SimpleFFT();
    
    private float[] clipSampleData;
    private int sampleRate;

    private bool isListening = false;
    private float listenTimer = 0f;
    private float cooldownTimer = 0f;
    private List<float> windowFrequencies = new List<float>();

private static readonly Dictionary<float, NoteType> frequencyToNoteType = new Dictionary<float, NoteType>
    {
        {82.41f, NoteType.E2},
        {87.31f, NoteType.F2},
        {92.50f, NoteType.F2 | NoteType.Sharp}, // F#2
        {98.00f, NoteType.G2},
        {103.83f, NoteType.G2 | NoteType.Sharp}, // G#2
        {110.00f, NoteType.A2},
        {116.54f, NoteType.A2 | NoteType.Sharp}, // A#2
        {123.47f, NoteType.B2},

        {130.81f, NoteType.C3},
        {138.59f, NoteType.C3 | NoteType.Sharp}, // C#3
        {146.83f, NoteType.D3},
        {155.56f, NoteType.D3 | NoteType.Sharp}, // D#3
        {164.81f, NoteType.E3},
        {174.61f, NoteType.F3},
        {185.00f, NoteType.F3 | NoteType.Sharp}, // F#3
        {196.00f, NoteType.G3},
        {207.65f, NoteType.G3 | NoteType.Sharp}, // G#3
        {220.00f, NoteType.A3},
        {233.08f, NoteType.A3 | NoteType.Sharp}, // A#3
        {246.94f, NoteType.B3},

        {261.63f, NoteType.C4},
        {277.18f, NoteType.C4 | NoteType.Sharp}, // C#4
        {293.66f, NoteType.D4},
        {311.13f, NoteType.D4 | NoteType.Sharp}, // D#4
        {329.63f, NoteType.E4},
        {349.23f, NoteType.F4},
        {369.99f, NoteType.F4 | NoteType.Sharp}, // F#4
        {392.00f, NoteType.G4},
        {415.30f, NoteType.G4 | NoteType.Sharp}, // G#4
        {440.00f, NoteType.A4},
        {466.16f, NoteType.A4 | NoteType.Sharp}, // A#4
        {493.88f, NoteType.B4},

        {523.25f, NoteType.C5},
        {554.37f, NoteType.C5 | NoteType.Sharp}, // C#5
        {587.33f, NoteType.D5},
        {622.25f, NoteType.D5 | NoteType.Sharp}, // D#5
        {659.25f, NoteType.E5},
        {698.46f, NoteType.F5},
        {739.99f, NoteType.F5 | NoteType.Sharp}, // F#5
        {783.99f, NoteType.G5},
        {830.61f, NoteType.G5 | NoteType.Sharp}  // G#5
    };

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found.");
            enabled = false;
            return;
        }

        microphoneDevice = null;
        sampleRate = AudioSettings.outputSampleRate;
        clipSampleData = new float[fftSampleSize];

        micClip = Microphone.Start(microphoneDevice, true, 10, sampleRate);
        audioSource.clip = micClip;
        audioSource.loop = true;

        while (!(Microphone.GetPosition(microphoneDevice) > 0)) { }
        audioSource.Play();
    }

    void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        float currentLoudness = GetLoudnessFromMic();

        if (!isListening && currentLoudness >= loudnessThreshold && cooldownTimer <= 0)
        {
            isListening = true;
            listenTimer = 0f;
            windowFrequencies.Clear();
            cooldownTimer = noteCooldown;
        }

        if (isListening)
        {
            listenTimer += Time.deltaTime;

            if (listenTimer > ignoreAttackTime)
            {
                float freq = GetPitchFromCustomFFT();
                windowFrequencies.Add(freq);
            }

            if (listenTimer >= listeningDuration)
            {
                DetermineNote();
                isListening = false; 
            }
        }
    }

    private void DetermineNote()
    {
        if (windowFrequencies.Count == 0) return;

        windowFrequencies.Sort();
        float medianFreq = windowFrequencies[windowFrequencies.Count / 2];

        float minDiff = float.MaxValue;
        NoteType bestNote = 0;

        foreach (var kvp in frequencyToNoteType)
        {
            float diff = Mathf.Abs(medianFreq - kvp.Key);
            if (diff < minDiff)
            {
                minDiff = diff;
                bestNote = kvp.Value;
            }
        }

        if (minDiff <= noteMarginHz)
        {
            Debug.Log($"<color=green>DETECTED: {bestNote} ({medianFreq:F1}Hz)</color>");
            spawnNote.Raise(this, bestNote);
        }
    }

    private float GetLoudnessFromMic()
    {
        int micPos = Microphone.GetPosition(microphoneDevice);
        int startPos = micPos - fftSampleSize;
        
        if (startPos < 0) return -80f; 

        micClip.GetData(clipSampleData, startPos);

        float sum = 0f;
        for (int i = 0; i < fftSampleSize; i++)
        {
            sum += clipSampleData[i] * clipSampleData[i];
        }
        return 20 * Mathf.Log10(Mathf.Sqrt(sum / fftSampleSize) + 1e-6f);
    }

    private float GetPitchFromCustomFFT()
    {
        simpleFFT.ApplyHanningWindow(clipSampleData);

        float[] spectrum = simpleFFT.ComputeFFT(clipSampleData);

        float maxVal = 0f;
        int searchLimit = spectrum.Length / 2; 
        for (int i = 7; i < searchLimit; i++)
        {
            if (spectrum[i] > maxVal) maxVal = spectrum[i];
        }

        if (maxVal < 0.0001f) return 0f;

        float threshold = maxVal * harmonicThreshold; 
        int fundamentalIndex = 0;

        for (int i = 1; i < searchLimit; i++)
        {
            if (spectrum[i] > spectrum[i - 1] && spectrum[i] > spectrum[i + 1])
            {
                if (spectrum[i] > threshold)
                {
                    fundamentalIndex = i;
                    break;
                }
            }
        }

        float freqIndex = fundamentalIndex;
        if (fundamentalIndex > 0 && fundamentalIndex < spectrum.Length - 1)
        {
            float dL = spectrum[fundamentalIndex - 1];
            float dC = spectrum[fundamentalIndex];
            float dR = spectrum[fundamentalIndex + 1];
            
            if (Mathf.Abs(dL - 2f * dC + dR) > 0.00001f)
            {
                freqIndex += 0.5f * (dL - dR) / (dL - 2f * dC + dR);
            }
        }

        return freqIndex * (sampleRate / (float)fftSampleSize);
    }
}