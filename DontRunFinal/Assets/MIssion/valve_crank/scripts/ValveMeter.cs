using System;
using UnityEngine;

public class ValveMeter : MonoBehaviour, ITaskProgress
{
    public RectTransform meter;

    [Header("Meter Setup")]
    public float startZ = -173.508f;
    public float maxRotation = 270f;

    [Header("Progress")]
    public float progress;
    public float maxProgress = 100f;

    [Header("Speed Settings")]
    public float crankPower = 0.03f;
    public float decayRate = 1f;

    [Header("Smoothing")]
    public float lerpSpeed = 8f;

    private bool taskComplete = false;
    private float currentRotation;

    // Required by ITaskProgress
    public event Action OnTaskCompleted;

    void Start()
    {
        ResetProgress();
    }

    void Update()
    {
        if (taskComplete) return;

        progress -= decayRate * Time.deltaTime;
        progress = Mathf.Clamp(progress, 0, maxProgress);

        UpdateMeter();
    }

    public void AddProgress(float delta)
    {
        if (taskComplete) return;

        progress += delta * crankPower;
        progress = Mathf.Clamp(progress, 0, maxProgress);

        UpdateMeter();

        if (progress >= maxProgress)
        {
            taskComplete = true;

            Debug.Log("Task Complete!");

            // Close the task through TaskUI
            OnTaskCompleted?.Invoke();
        }
    }

    public bool IsTaskComplete()
    {
        return taskComplete;
    }

    // Required by ITaskProgress
    public void ResetProgress()
    {
        taskComplete = false;

        progress = 0;

        currentRotation = startZ;

        if (meter != null)
            meter.localRotation = Quaternion.Euler(0, 0, startZ);
    }

    void UpdateMeter()
    {
        float percent = progress / maxProgress;

        float targetRotation = startZ - (percent * maxRotation);

        currentRotation = Mathf.Lerp(
            currentRotation,
            targetRotation,
            lerpSpeed * Time.deltaTime
        );

        meter.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }
}