using System;

public interface ITaskProgress
{
    event Action OnTaskCompleted;
    void ResetProgress();
}