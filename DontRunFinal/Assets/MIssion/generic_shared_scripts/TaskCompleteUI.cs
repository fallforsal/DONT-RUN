using UnityEngine;

public class TaskCompleteUI : MonoBehaviour
{
    public GameObject popup;
    public float showTime = 2f;

    public void ShowComplete()
    {
        StartCoroutine(ShowRoutine());
    }

    System.Collections.IEnumerator ShowRoutine()
    {
        popup.SetActive(true);
        yield return new WaitForSeconds(showTime);
        popup.SetActive(false);
    }
}