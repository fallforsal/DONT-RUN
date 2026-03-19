using UnityEngine;

public class MovingTriangle : MonoBehaviour
{
    public RectTransform bar;

    public float minSpeed = 350f;
    public float maxSpeed = 600f;


    float speed;
    public RectTransform tip;
    RectTransform rect;
    float leftLimit;
    float rightLimit;
    int direction = 1;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        float barHalf = bar.rect.width / 2;
        float triangleHalf = rect.rect.width / 2;

        leftLimit = -barHalf + triangleHalf;
        rightLimit = barHalf - triangleHalf;

        RandomizeSpeed();
    }

    void Update()
    {
        rect.anchoredPosition += new Vector2(speed * direction * Time.deltaTime, 0);

        if (rect.anchoredPosition.x >= rightLimit)
        {
            direction = -1;
            RandomizeSpeed();
        }

        if (rect.anchoredPosition.x <= leftLimit)
        {
            direction = 1;
            RandomizeSpeed();
        }
    }

    public float GetX()
    {
        return tip.anchoredPosition.x + rect.anchoredPosition.x;
    }

    public void RandomizeSpeed()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }
}