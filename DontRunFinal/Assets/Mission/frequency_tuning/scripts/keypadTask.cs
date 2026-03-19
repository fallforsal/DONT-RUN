using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeypadTask : MonoBehaviour, ITaskProgress
{
    public Text _inputCode;

    public FrequencyTuner tuner;

    public float _codeResetTimeInSeconds = 0.5f;
    // Maximum digits the keypad will accept for a code.
    // This will be synced to the tuner's code length on enable.
    public int _maxInputDigits = 6;

    private bool _isResetting = false;

    private string _inputBuffer = "";
    private string _correctCode = "";

    // ITaskProgress event
    public event Action OnTaskCompleted;

    void OnEnable()
    {
        _inputBuffer = "";
        _inputCode.text = "";

        // Sync digit count & cache code for this keypad session.
        // Note: we still refresh the code again on Submit() in case
        // this keypad was enabled before the tuner initialized.
        if (tuner != null)
        {
            _maxInputDigits = Mathf.Max(1, tuner.totalDigits);
            _correctCode = tuner.GetCode();
        }
    }

    public void ButtonClick(int number)
    {
        if (_isResetting) return;

        // Lazily cache the code if for some reason it wasn't set on enable
        if (string.IsNullOrEmpty(_correctCode) && tuner != null)
            _correctCode = tuner.GetCode();

        // Prevent entering more than the maximum allowed digits
        if (_inputBuffer.Length >= _maxInputDigits)
            return;

        _inputBuffer += number.ToString();
        _inputCode.text = _inputBuffer;
    }

    public void Submit()
    {
        if (_isResetting) return;

        if (string.IsNullOrEmpty(_inputBuffer)) return;

        // Always refresh right before comparing to avoid stale cached codes
        // (e.g., if this keypad enabled before the tuner generated its code,
        // or if the tuner was reset externally while the keypad UI remained open).
        if (tuner != null)
        {
            _maxInputDigits = Mathf.Max(1, tuner.totalDigits);
            _correctCode = tuner.GetCode();
        }

        // Compare input to the effective correct code (optionally truncated to max digits)
        string effectiveCode = _correctCode;
        if (!string.IsNullOrEmpty(effectiveCode) && effectiveCode.Length > _maxInputDigits)
            effectiveCode = effectiveCode.Substring(0, _maxInputDigits);

        if (_inputBuffer == effectiveCode)
        {
            HandleCorrect();
        }
        else
        {
            HandleFailed();
        }
    }

    public void DeleteOne()
    {
        if (_isResetting) return;

        if (_inputBuffer.Length == 0) return;

        _inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 1);
        _inputCode.text = _inputBuffer;
    }

    private void HandleCorrect()
    {
        Debug.Log("Correct code entered!");

        _inputCode.text = "Correct";

        // Reset the tuner visuals (number & correct zone) for the next time this task is opened
        if (tuner != null)
            tuner.ResetProgress();

        // Notify listeners (Interactable/TaskUI) that this task is complete
        OnTaskCompleted?.Invoke();

        // Do NOT run the local reset coroutine here because this GameObject
        // may be deactivated immediately by the interaction system.
        // The task system will call ResetProgress() the next time this task opens.
    }

    private void HandleFailed()
    {
        Debug.Log("Incorrect code entered!");

        _inputCode.text = "Failed";

        StartCoroutine(ResetCode());
    }

    private IEnumerator ResetCode()
    {
        _isResetting = true;

        yield return new WaitForSeconds(_codeResetTimeInSeconds);

        _inputBuffer = "";
        _inputCode.text = "";

        _isResetting = false;
    }

    // ITaskProgress implementation
    public void ResetProgress()
    {
        // Reset local keypad state
        _inputBuffer = "";
        _inputCode.text = "";
        _isResetting = false;

        // Also reset the tuner (number, swipe bar & correct zone placement)
        // when the player cancels/escapes this task.
        if (tuner != null)
        {
            tuner.ResetProgress();
            _correctCode = tuner.GetCode();
        }
    }

}