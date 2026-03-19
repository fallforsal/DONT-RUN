using UnityEngine;
using UnityEngine.UI;

public class Wedge : MonoBehaviour
{
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.yellow;

    Image img;
    bool active = false;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    void EnsureImage()
    {
        if (img == null)
            img = GetComponent<Image>();
    }

    public void Activate()
    {
        EnsureImage();
        active = true;
        img.color = activeColor;
    }

    public void Deactivate()
    {
        EnsureImage();
        active = false;
        img.color = inactiveColor;
    }

    public bool IsActive()
    {
        return active;
    }
}