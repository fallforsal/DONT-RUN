using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float minSpeed = 100f;
    public float maxSpeed = 250f;

    float speed;
    int direction = 1; // start clockwise

    void Start()
    {
        RandomizeSpeed(); // random speed at start
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * speed * direction * Time.deltaTime);
    }

    public float GetAngle()
    {
        return transform.eulerAngles.z;
    }

    public void RandomizeDirection()
    {
        direction = Random.value > 0.5f ? 1 : -1;
    }

    public void RandomizeSpeed()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }
}