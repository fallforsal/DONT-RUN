using UnityEngine;
using System;
using System.Collections;

public class WheelController : MonoBehaviour, ITaskProgress
{
    public ArrowController arrow;
    public Wedge[] wedges;
    public ErrorIndicator[] errors;

    public float restTime = 3f;

    int wedgeCount;
    int errorCount = 0;
    bool halted = false;

    // REQUIRED by ITaskProgress
    public event Action OnTaskCompleted;

    void Start()
    {
        wedgeCount = wedges.Length;
        Debug.Log("WheelController initialized with " + wedgeCount + " wedges.");
    }

    void Update()
    {
        if (halted) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryActivate();
        }
    }

    void TryActivate()
    {
        float angle = arrow.GetAngle();
        float wedgeSize = 360f / wedgeCount;

        int index = Mathf.FloorToInt(angle / wedgeSize);
        index = Mathf.Clamp(index, 0, wedgeCount - 1);

        Debug.Log("Arrow angle: " + angle + " -> Wedge: " + index);

        if (wedges[index].IsActive())
        {
            Debug.Log("ERROR: Player hit an already active wedge.");

            wedges[index].Deactivate();
            RegisterError();
        }
        else
        {
            Debug.Log("Activated wedge " + index);
            wedges[index].Activate();

            if (AllWedgesActive())
            {
                TaskCompleted();
                return;
            }
        }

        arrow.RandomizeSpeed();
        arrow.RandomizeDirection();
    }

    bool AllWedgesActive()
    {
        foreach (var wedge in wedges)
        {
            if (!wedge.IsActive())
                return false;
        }
        return true;
    }

    void TaskCompleted()
    {
        halted = true;
        Debug.Log("TASK COMPLETED!");

        // notify TaskUI
        OnTaskCompleted?.Invoke();
    }

    void RegisterError()
    {
        if (errorCount < errors.Length)
        {
            errors[errorCount].Activate();
        }

        errorCount++;

        Debug.Log("Error count: " + errorCount);

        if (errorCount >= errors.Length)
        {
            Debug.Log("SYSTEM HALT triggered.");
            StartCoroutine(HaltSystem());
        }
    }

    IEnumerator HaltSystem()
    {
        halted = true;

        Debug.Log("Resetting wedges and entering cooldown.");

        foreach (var wedge in wedges)
        {
            wedge.Deactivate();
        }

        yield return new WaitForSeconds(restTime);

        foreach (var err in errors)
        {
            err.ResetIndicator();
        }

        errorCount = 0;
        halted = false;

        Debug.Log("System resumed.");
    }

    public void ResetProgress()
    {
        StopAllCoroutines();

        foreach (var wedge in wedges)
        {
            if (wedge != null)
                wedge.Deactivate();
        }

        foreach (var err in errors)
        {
            if (err != null)
                err.ResetIndicator();
        }

        errorCount = 0;
        halted = false;

        Debug.Log("Task progress reset.");
    }
}