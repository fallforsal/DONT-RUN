using UnityEngine;

public class SpriteOutline : MonoBehaviour
{
    SpriteRenderer sr;
    Material mat;

    public float outlineSize = 4f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // create unique material instance for this object
        mat = new Material(sr.material);
        sr.material = mat;

        DisableOutline();
    }

    public void EnableOutline()
    {
        mat.SetFloat("_Outline", 4f);
        mat.SetFloat("_OutlineSize", outlineSize);
        mat.SetColor("_OutlineColor", Color.yellow);

        Debug.Log("Outline enabled");
    }

    public void DisableOutline()
    {
        mat.SetFloat("_Outline", 0f);
    }
}