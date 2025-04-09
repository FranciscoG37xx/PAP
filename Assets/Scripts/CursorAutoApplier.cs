using UnityEngine;
using UnityEngine.UI;

public class CursorAutoApplier : MonoBehaviour
{
    public Texture2D pointerCursor;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        ApplyCursorToAllButtons();
    }

    public void ApplyCursorToAllButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true); // <-- 'true' inclui botões desativados
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.GetComponent<PointerCursorChanger>() == null)
            {
                PointerCursorChanger changer = btn.gameObject.AddComponent<PointerCursorChanger>();
                changer.customCursor = pointerCursor;
                changer.cursorMode = cursorMode;
            }
        }
    }
}


