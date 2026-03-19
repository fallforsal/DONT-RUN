using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireTask : MonoBehaviour, ITaskProgress
{
    public List<Color> _wireColors = new List<Color>();
    public List<Wire> _leftWires = new List<Wire>();
    public List<Wire> _rightWires = new List<Wire>();

    public Wire CurrentDraggedWire;
    public Wire CurrentHoveredWire;

    public bool IsTaskCompleted = false;

    // REQUIRED BY ITaskProgress
    public event Action OnTaskCompleted;

    private List<Color> _availableColors;
    private List<int> _availableLeftWireIndex;
    private List<int> _availableRightWireIndex;

    private void Start()
    {
        _availableColors = new List<Color>(_wireColors);
        _availableLeftWireIndex = new List<int>();
        _availableRightWireIndex = new List<int>();

        for (int i = 0; i < _leftWires.Count; i++)
        {
            _availableLeftWireIndex.Add(i);
        }

        for (int i = 0; i < _rightWires.Count; i++)
        {
            _availableRightWireIndex.Add(i);
        }

        while (_availableColors.Count > 0 &&
               _availableLeftWireIndex.Count > 0 &&
               _availableRightWireIndex.Count > 0)
        {

            Color pickedColor =
             _availableColors[UnityEngine.Random.Range(0, _availableColors.Count)];

            int pickedLeftWireIndex = UnityEngine.Random.Range(0,
                                      _availableLeftWireIndex.Count);

            int pickedRightWireIndex = UnityEngine.Random.Range(0,
                                      _availableRightWireIndex.Count);

            _leftWires[_availableLeftWireIndex[pickedLeftWireIndex]]
                                              .SetColor(pickedColor);

            _rightWires[_availableRightWireIndex[pickedRightWireIndex]]
                                              .SetColor(pickedColor);

            _availableColors.Remove(pickedColor);
            _availableLeftWireIndex.RemoveAt(pickedLeftWireIndex);
            _availableRightWireIndex.RemoveAt(pickedRightWireIndex);
        }

        StartCoroutine(CheckTaskCompletion());
    }

    // ADDITION: restart coroutine whenever the task object becomes active again
    void OnEnable()
    {
        if (!IsTaskCompleted)
        {
            StopAllCoroutines();
            StartCoroutine(CheckTaskCompletion());
        }
    }

    private IEnumerator CheckTaskCompletion()
    {
        while (!IsTaskCompleted)
        {

            int successfulWires = 0;

            for (int i = 0; i < _rightWires.Count; i++)
            {
                if (_rightWires[i].IsSuccess) { successfulWires++; }
            }

            if (successfulWires >= _rightWires.Count)
            {
                IsTaskCompleted = true;

                Debug.Log("TASK COMPLETED");

                // NOTIFY TaskUI
                OnTaskCompleted?.Invoke();
            }
            else
            {
                Debug.Log("TASK INCOMPLETED");
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    // REQUIRED BY ITaskProgress
    public void ResetProgress()
    {
        IsTaskCompleted = false;

        CurrentDraggedWire = null;
        CurrentHoveredWire = null;

        foreach (var wire in _leftWires)
            wire.ResetWire();

        foreach (var wire in _rightWires)
            wire.ResetWire();

        StopAllCoroutines();
        StartCoroutine(CheckTaskCompletion());
    }
}