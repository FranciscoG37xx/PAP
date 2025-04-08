using UnityEngine;
using UnityEngine.UI;

public class CursorAutoApplier : MonoBehaviour
{
    public Texture2D pointerCursor; // A textura da mãozinha
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        if (pointerCursor == null)
        {
            Debug.LogError("Nenhum cursor de mãozinha atribuído no CursorAutoApplier!");
            return;
        }

        // Vai buscar todos os botões na cena
        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (Button btn in allButtons)
        {
            // Verifica se o botão já tem o script para evitar duplicados
            if (btn.gameObject.GetComponent<PointerCursorChanger>() == null)
            {
                PointerCursorChanger changer = btn.gameObject.AddComponent<PointerCursorChanger>();
                changer.customCursor = pointerCursor;
                changer.cursorMode = cursorMode;
            }
        }
    }
}

