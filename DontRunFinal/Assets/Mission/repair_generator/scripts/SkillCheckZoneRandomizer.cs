using UnityEngine;

public class SkillCheckZoneRandomizer : MonoBehaviour
{
    public RectTransform bar;
    public RectTransform yellowZone;

    public float minYellowWidth = 60f;
    public float maxYellowWidth = 120f;

    void Start()
    {
        RandomizeZones();
    }

    public void RandomizeZones()
    {
        float barWidth = bar.rect.width;

        float yellowWidth = Random.Range(minYellowWidth, Mathf.Min(maxYellowWidth, barWidth));
        yellowZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, yellowWidth);

        float limit = (barWidth / 2) - (yellowWidth / 2);
        float randomX = Random.Range(-limit, limit);

        yellowZone.anchoredPosition = new Vector2(randomX, yellowZone.anchoredPosition.y);
    }

    public float GetYellowCenter()
    {
        return yellowZone.anchoredPosition.x;
    }

    public float GetYellowHalf()
    {
        return yellowZone.rect.width / 2;
    }
}