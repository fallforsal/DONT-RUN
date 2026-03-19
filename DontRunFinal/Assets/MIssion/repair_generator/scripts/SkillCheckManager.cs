using UnityEngine;

public class SkillCheckManager : MonoBehaviour
{
    public MovingTriangle triangle;
    public SkillCheckZoneRandomizer zones;
    public ProgressManager progress;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Evaluate();
        }
    }

    void Evaluate()
    {
        float x = triangle.GetX();

        float yellowMin = zones.GetYellowCenter() - zones.GetYellowHalf();
        float yellowMax = zones.GetYellowCenter() + zones.GetYellowHalf();

        if (x >= yellowMin && x <= yellowMax)
        {
            Debug.Log("SUCCESS");

            progress.Success();

            triangle.RandomizeSpeed();
            zones.RandomizeZones();
        }
        else
        {
            Debug.Log("FAIL");

            progress.Fail();

            triangle.RandomizeSpeed();
            zones.RandomizeZones();
        }
    }
}