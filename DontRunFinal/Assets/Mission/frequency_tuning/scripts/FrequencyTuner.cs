using System;
using UnityEngine;
using UnityEngine.UI;

public class FrequencyTuner : MonoBehaviour, ITaskProgress
{
    public RectTransform swipeBar;
    public RectTransform limitBar;
    public RectTransform correctZone;

    public Text codeText;

    public int totalDigits = 6;
    public float revealDelay = 0.4f;
    public bool randomizeSwipeStart = true;

    private string fullCode;
    private int revealedDigits = 0;

    private float revealTimer = 0f;
    private bool inWindow = false;

    // ITaskProgress event
    public event Action OnTaskCompleted;

    void Start()
    {
        // Original behaviour: generate a fresh code and reset everything
        ResetProgress();
    }

    void Update()
    {
        inWindow = CheckWindow();
        RevealDigits();
    }

    void GenerateCode()
    {
        fullCode = "";

        for (int i = 0; i < totalDigits; i++)
        {
            fullCode += UnityEngine.Random.Range(0, 10).ToString();
        }
    }

    void RandomizeCorrectZone()
    {
        float limitWidth = limitBar.rect.width;
        float zoneWidth = correctZone.rect.width;

        float minX = -limitWidth / 2 + zoneWidth / 2;
        float maxX = limitWidth / 2 - zoneWidth / 2;

        float randomX = UnityEngine.Random.Range(minX, maxX);

        Vector2 pos = correctZone.anchoredPosition;
        pos.x = randomX;

        correctZone.anchoredPosition = pos;
    }

    void RandomizeSwipeBar()
    {
        if (swipeBar == null || limitBar == null) return;

        float limitWidth = limitBar.rect.width;
        float barWidth = swipeBar.rect.width;

        float minX = -limitWidth / 2 + barWidth / 2;
        float maxX =  limitWidth / 2 - barWidth / 2;

        float randomX = UnityEngine.Random.Range(minX, maxX);

        Vector2 pos = swipeBar.anchoredPosition;
        pos.x = randomX;
        swipeBar.anchoredPosition = pos;
    }

    void UpdateDisplay()
    {
        // Show revealed digits followed by hidden underscores
        string visible = fullCode.Substring(0, revealedDigits);
        string hidden = new string('_', totalDigits - revealedDigits);

        codeText.text = visible + hidden;
    }

    bool CheckWindow()
    {
        Rect swipeRect = GetWorldRect(swipeBar);
        Rect zoneRect = GetWorldRect(correctZone);

        return swipeRect.Overlaps(zoneRect);
    }

    Rect GetWorldRect(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        return new Rect(corners[0].x, corners[0].y, width, height);
    }

    void RevealDigits()
    {
        if (!inWindow || revealedDigits >= totalDigits)
        {
            revealTimer = 0;
            return;
        }

        revealTimer += Time.deltaTime;

        if (revealTimer >= revealDelay)
        {
            revealTimer = 0;

            revealedDigits++;
            UpdateDisplay();

            if (revealedDigits >= totalDigits)
            {
                Debug.Log("Frequency Tuned!");

                // notify any listeners that this task is complete
                OnTaskCompleted?.Invoke();
            }
        }
    }

    public string GetCode()
    {
        return fullCode;
    }

    public bool IsCodeFullyRevealed()
    {
        return revealedDigits >= totalDigits;
    }

    // ITaskProgress implementation
    public void ResetProgress()
    {
        revealedDigits = 0;
        revealTimer = 0f;

        GenerateCode();
        RandomizeCorrectZone();

        if (randomizeSwipeStart)
            RandomizeSwipeBar();

        UpdateDisplay();
    }
}