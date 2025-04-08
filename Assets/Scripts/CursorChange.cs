using UnityEngine;
using UnityEngine.EventSystems;

public class InputFieldCursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D customCursor; // Cursor personalizado
    public CursorMode cursorMode = CursorMode.Auto; // Modo do cursor
    private Vector2 hotSpot; // Ponto ativo do cursor

    // Inicializa o hotspot no início
    void Start()
    {
        if (customCursor != null)
        {
            // Define o ponto ativo como o centro da textura
            hotSpot = new Vector2(customCursor.width / 2, customCursor.height / 2);
        }
        else
        {
            Debug.LogError("Nenhuma textura foi atribuída ao campo customCursor!");
        }
    }

    // Ativado quando o cursor entra no campo de texto
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (customCursor != null)
        {
            Cursor.SetCursor(customCursor, hotSpot, cursorMode);
        }
    }

    // Ativado quando o cursor sai do campo de texto
    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}




