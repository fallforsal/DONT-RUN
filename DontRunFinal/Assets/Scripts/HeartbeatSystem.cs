using UnityEngine;
using UnityEngine.UI;

public class HeartbeatSystem : MonoBehaviour
{
    [Header("Tham chiếu")]
    public Transform hunter;
    public RectTransform heartUI;
    public AudioSource heartbeatAudio;

    [Header("Cài đặt Tỷ lệ")]
    public float minDistance = 3f;
    public float maxDistance = 20f;
    public float minHeartScale = 0.8f;
    public float maxHeartScale = 1.2f;

    [Header("Cài đặt Âm thanh & Độ giật")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.8f;
    public float audioSensitivity = 25f;

    // TRẦN NHÀ CHỐNG NỔ KÍCH THƯỚC:
    public float absoluteMaxScale = 2.5f;

    private void Update()
    {
        if (hunter == null)
        {
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject go in allObjects)
            {
                if (go.name.ToLower().Contains("boss") || go.name.ToLower().Contains("enemy"))
                { hunter = go.transform; break; }
            }
            if (hunter == null)
            { if (heartUI.gameObject.activeSelf) heartUI.gameObject.SetActive(false); return; }
        }

        if (heartUI == null) return;

        float distance = Vector2.Distance(transform.position, hunter.position);
        float intensity = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);
        intensity = Mathf.Clamp01(intensity);

        bool shouldActive = intensity > 0.01f;
        if (heartUI.gameObject.activeSelf != shouldActive) heartUI.gameObject.SetActive(shouldActive);

        if (!shouldActive)
        { if (heartbeatAudio != null) heartbeatAudio.mute = true; return; }

        if (heartbeatAudio != null)
        {
            heartbeatAudio.mute = false;
            heartbeatAudio.volume = intensity;
            heartbeatAudio.pitch = Mathf.Lerp(minPitch, maxPitch, intensity);

            float[] samples = new float[64];
            heartbeatAudio.GetOutputData(samples, 0);
            float currentVolume = 0;
            foreach (float s in samples) currentVolume += Mathf.Abs(s);
            currentVolume /= 64f;

            float baseScale = Mathf.Lerp(minHeartScale, maxHeartScale, intensity);
            float pulse = currentVolume * audioSensitivity * intensity;
            float finalScale = baseScale + pulse;

            // LỆNH KẸP KÍCH THƯỚC (Sẽ chặn đứng mọi cú giật lố):
            finalScale = Mathf.Clamp(finalScale, minHeartScale, absoluteMaxScale);

            heartUI.localScale = new Vector3(finalScale, finalScale, finalScale);
        }
    }
}