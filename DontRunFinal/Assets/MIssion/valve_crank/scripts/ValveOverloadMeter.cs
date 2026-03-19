using UnityEngine;

public class ValveOverloadMeter : MonoBehaviour
{
    public RectTransform meter;

    [Header("Meter Setup")]
    public float startZ = 0f;
    public float maxRotation = 180f;

    [Header("Value")]
    public float value;
    public float maxValue = 25f;

    [Header("Zones")]
    public float cautiousMin = 15f;
    public float overloadMin = 20f;

    [Header("Speed")]
    public float increasePower = 0.02f;
    public float decayRate = 2f;

    [Header("Meter Smooth")]
    public float lerpSpeed = 8f;

    [Header("Overload Behaviour")]
    public float overloadMaxTime = 3f;
    public float restTime = 2f;

    private float overloadTimer = 0f;
    private float restTimer = 0f;

    private bool overloadTriggered = false;
    private float currentRotation;

    private enum Zone
    {
        Safe,
        Cautious,
        Overload
    }

    private Zone currentZone = Zone.Safe;

    public ValveMeter mainMeter;
    public ValveShake shaker;

    void Start()
    {
        currentRotation = startZ;
        meter.localRotation = Quaternion.Euler(0, 0, startZ);
    }

    void Update()
    {
        if (mainMeter != null && mainMeter.IsTaskComplete())
            return;

        if (overloadTriggered)
        {
            restTimer -= Time.deltaTime;

            if (restTimer <= 0)
            {
                overloadTriggered = false;
                value = 0;
                overloadTimer = 0;
                currentZone = Zone.Safe;

                shaker.StopShake();

                Debug.Log("Machine cooled down");
            }
            UpdateMeter();
            return;
        }

        value -= decayRate * Time.deltaTime;
        value = Mathf.Clamp(value, 0, maxValue);

        UpdateZoneState();
        UpdateMeter();
    }

    void UpdateZoneState()
    {
        Zone newZone;

        if (value >= overloadMin)
            newZone = Zone.Overload;
        else if (value >= cautiousMin)
            newZone = Zone.Cautious;
        else
            newZone = Zone.Safe;

        if (newZone != currentZone)
        {
            currentZone = newZone;

            switch (currentZone)
            {
                case Zone.Safe:
                    Debug.Log("Heat normal");
                    shaker.StopShake();
                    break;

                case Zone.Cautious:
                    Debug.Log("Be cautious");
                    shaker.StartCautiousShake();
                    break;

                case Zone.Overload:
                    Debug.Log("OVERLOAD");
                    shaker.StartOverloadShake();
                    break;
            }
        }

        if (currentZone == Zone.Overload)
        {
            overloadTimer += Time.deltaTime;

            if (overloadTimer >= overloadMaxTime)
            {
                TriggerOverload();
            }
        }
        else
        {
            overloadTimer = 0;
        }
    }

    public void AddHeat(float delta)
    {
        if (overloadTriggered) return;

        value += delta * increasePower;
        value = Mathf.Clamp(value, 0, maxValue);
    }

    void TriggerOverload()
    {
        overloadTriggered = true;
        restTimer = restTime;

        Debug.Log("The machine is too hot, rest");

        if (mainMeter != null)
            mainMeter.ResetProgress();

        shaker.StartRestDecay(restTime);
    }

    public bool CanCrank()
    {
        return !overloadTriggered;
    }

    void UpdateMeter()
    {
        float percent = value / maxValue;
        float targetRotation = startZ - (percent * maxRotation);

        currentRotation = Mathf.Lerp(
            currentRotation,
            targetRotation,
            lerpSpeed * Time.deltaTime
        );

        meter.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }
}