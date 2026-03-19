using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HunterRedLight : MonoBehaviour
{
    [Header("Tham chiếu")]
    public Transform survivor;
    public Light2D redLight2D;

    [Header("Cài đặt")]
    public float minDistance = 3f;
    public float maxDistance = 20f;
    public float maxLightIntensity = 3f;
    public float maxLightRadius = 10f;

    private void Update()
    {
        // THUẬT TOÁN RADAR: TÌM PLAYER 
        if (survivor == null)
        {
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject go in allObjects)
            {
                if (go.name.ToLower().Contains("player"))
                {
                    survivor = go.transform;
                    break;
                }
            }

            if (survivor == null)
            {
                if (redLight2D != null && redLight2D.gameObject.activeSelf)
                    redLight2D.gameObject.SetActive(false);
                return;
            }
        }

        if (redLight2D == null) return;

        // TÍNH KHOẢNG CÁCH
        float distance = Vector2.Distance(transform.position, survivor.position);
        float intensity = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);
        intensity = Mathf.Clamp01(intensity);

        bool shouldActive = intensity > 0.001f;

        if (redLight2D.gameObject.activeSelf != shouldActive)
            redLight2D.gameObject.SetActive(shouldActive);

        if (!shouldActive) return;

        // XỬ LÝ ĐÈN SÁNG
        redLight2D.intensity = Mathf.Lerp(0f, maxLightIntensity, intensity);
        redLight2D.pointLightOuterRadius = Mathf.Lerp(1f, maxLightRadius, intensity);
    }
}