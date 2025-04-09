using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModCategorySelector : MonoBehaviour
{
    public GameObject[] categoryPanels; // Todos os painéis (visíveis ao mesmo tempo)
    public Button leftArrow;
    public Button rightArrow;

    private int currentIndex = 0;

    void Start()
    {
        HighlightSelected();
        leftArrow.onClick.AddListener(Previous);
        rightArrow.onClick.AddListener(Next);
    }

    void Update()
    {
        // Garante que [Enter] só executa ação se nenhum botão estiver "focused"
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) &&
            !EventSystem.current.currentSelectedGameObject)
        {
            SelectCurrent();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Previous();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Next();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            SelectCurrent();
    }

    void Previous()
    {
        currentIndex = (currentIndex - 1 + categoryPanels.Length) % categoryPanels.Length;
        HighlightSelected();
    }

    void Next()
    {
        currentIndex = (currentIndex + 1) % categoryPanels.Length;
        HighlightSelected();
    }

    void HighlightSelected()
    {
        for (int i = 0; i < categoryPanels.Length; i++)
        {
            Transform border = categoryPanels[i].transform.Find("Border");
            if (border != null)
                border.gameObject.SetActive(i == currentIndex); // mostra só o contorno do selecionado
        }
    }

    void SelectCurrent()
    {
        Debug.Log("Selecionaste: " + categoryPanels[currentIndex].name);
        // Aqui podes abrir o menu da categoria ou outro painel com mods
    }
}


