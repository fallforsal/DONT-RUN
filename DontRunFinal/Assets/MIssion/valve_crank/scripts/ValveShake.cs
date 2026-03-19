using UnityEngine;

public class ValveShake : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 originalPos;

    private float shakeStrength = 0f;
    private float shakeDecay = 0f;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
    }

    void Update()
    {
        if (shakeStrength > 0)
        {
            Vector2 offset = Random.insideUnitCircle * shakeStrength;
            rect.anchoredPosition = originalPos + offset;

            shakeStrength -= shakeDecay * Time.deltaTime;

            if (shakeStrength <= 0)
            {
                shakeStrength = 0;
                rect.anchoredPosition = originalPos;
            }
        }
    }

    // Yellow zone
    public void StartCautiousShake()
    {
        shakeStrength = 3f;
        shakeDecay = 0f; // constant shake
    }

    // Overload
    public void StartOverloadShake()
    {
        shakeStrength = 10f;
        shakeDecay = 0f;
    }

    // Resting decay
    public void StartRestDecay(float restTime)
    {
        shakeDecay = shakeStrength / restTime;
    }

    public void StopShake()
    {
        shakeStrength = 0;
        rect.anchoredPosition = originalPos;
    }
}