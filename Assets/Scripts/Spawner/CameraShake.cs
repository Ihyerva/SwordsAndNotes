using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float magnitude = 0.03f;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake(Component sender, object data)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
        shakeCoroutine = null;
    }
} 