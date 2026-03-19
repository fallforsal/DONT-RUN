using UnityEngine;
using UnityEngine.UI;

public class ErrorIndicator : MonoBehaviour
{
    public int index;

    public Color inactiveColor = Color.black;
    public Color activeColor = Color.red;

    Image img;

    void EnsureImage()
    {
        if (img == null)
            img = GetComponent<Image>();
    }

    public void Activate()
    {
        EnsureImage();

        Debug.Log("Error Indicator " + index + " activated.");
        img.color = activeColor;
    }

    public void ResetIndicator()
    {
        EnsureImage();

        Debug.Log("Error Indicator " + index + " reset.");
        img.color = inactiveColor;
    }
}