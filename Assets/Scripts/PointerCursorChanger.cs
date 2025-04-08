using UnityEngine;
using UnityEngine.EventSystems;

public class PointerCursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D customCursor;
    public CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot;

    void Start()
    {
        if (customCursor != null)
        {
            hotSpot = new Vector2(customCursor.width / 2, customCursor.height / 2);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (customCursor != null)
        {
            Cursor.SetCursor(customCursor, hotSpot, cursorMode);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}

