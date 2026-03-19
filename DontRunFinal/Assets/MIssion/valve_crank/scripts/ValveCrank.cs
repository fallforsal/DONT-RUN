using UnityEngine;
using UnityEngine.EventSystems;

public class ValveCrank : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform crank;
    private float lastAngle;

    public ValveMeter meter;
    public ValveOverloadMeter overloadMeter;


    [Header("Overload Reverse")]
    public float reverseSpeed = 200f;

    void Awake()
    {
        crank = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Auto reverse if machine overheats
        if (!overloadMeter.CanCrank())
        {
            crank.Rotate(0, 0, reverseSpeed * Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 center = RectTransformUtility.WorldToScreenPoint(
            eventData.pressEventCamera,
            crank.position
        );

        Vector2 dir = eventData.position - center;

        lastAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!overloadMeter.CanCrank())
            return;

        Vector2 center = RectTransformUtility.WorldToScreenPoint(
            eventData.pressEventCamera,
            crank.position
        );

        Vector2 dir = eventData.position - center;

        float currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float delta = Mathf.DeltaAngle(lastAngle, currentAngle);

        // flip direction so clockwise = positive
        delta = -delta;

        crank.Rotate(0, 0, -delta);

        if (delta > 0)
        {
            meter.AddProgress(delta);
            overloadMeter.AddHeat(delta);
        }

        lastAngle = currentAngle;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Not needed but required by interface
    }
}