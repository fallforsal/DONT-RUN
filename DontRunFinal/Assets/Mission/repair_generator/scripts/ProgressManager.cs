using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour, ITaskProgress
{
    public Image fillImage;

    public event Action OnTaskCompleted;

    public float minSuccessGain = 0.05f;
    public float maxSuccessGain = 0.12f;
    public float minFailLoss = 0.01f;
    public float decayPerSecond = 0.01f;

    float lastSuccessGain = 0f;
    bool completed = false;

    void Update()
    {
        if (!completed)
            Decay();
    }

    void Decay()
    {
        fillImage.fillAmount -= decayPerSecond * Time.deltaTime;
        fillImage.fillAmount = Mathf.Clamp01(fillImage.fillAmount);
    }

    public void Success()
    {
        if (completed) return;

        float gain = UnityEngine.Random.Range(minSuccessGain, maxSuccessGain);
        lastSuccessGain = gain;

        fillImage.fillAmount += gain;
        fillImage.fillAmount = Mathf.Clamp01(fillImage.fillAmount);

        CheckCompletion();
    }

    public void Fail()
    {
        if (completed || lastSuccessGain <= 0) return;

        float loss = UnityEngine.Random.Range(minFailLoss, lastSuccessGain * 0.8f);

        fillImage.fillAmount -= loss;
        fillImage.fillAmount = Mathf.Clamp01(fillImage.fillAmount);
    }

    void CheckCompletion()
    {
        if (fillImage.fillAmount >= 1f && !completed)
        {
            completed = true;

            Debug.Log("TASK COMPLETED");

            OnTaskCompleted?.Invoke();
        }
    }

    public void ResetProgress()
    {
        fillImage.fillAmount = 0f;
        lastSuccessGain = 0f;
        completed = false;
    }
}