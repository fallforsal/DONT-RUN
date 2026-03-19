using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeBarDrag : MonoBehaviour, IDragHandler
{
    public RectTransform swipeBar;
    public RectTransform limitBar;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = swipeBar.anchoredPosition;

        pos.x += eventData.delta.x;

        float halfLimit = limitBar.rect.width / 2;
        float halfBar = swipeBar.rect.width / 2;

        pos.x = Mathf.Clamp(pos.x, -halfLimit + halfBar, halfLimit - halfBar);

        swipeBar.anchoredPosition = pos;
    }
}