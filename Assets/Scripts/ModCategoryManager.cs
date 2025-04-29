using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModCategorySelector : MonoBehaviour
{
    public GameObject[] categoryPanels;     // Os painéis de categorias (ex: pintura, jantes, etc.)
    public Button leftArrow;
    public Button rightArrow;
    public GameObject panelMods;            // ← Referência ao Panel_Mods (menu principal das mods)

    private int currentIndex = 0;

    void Start()
    {
        HighlightSelected();

        leftArrow.onClick.AddListener(Previous);
        rightArrow.onClick.AddListener(Next);

        // Adicionar listeners de clique nos botões dentro dos painéis
        foreach (GameObject panel in categoryPanels)
        {
            Button panelButton = panel.GetComponentInChildren<Button>();
            if (panelButton != null)
            {
                GameObject capturedPanel = panel;
                panelButton.onClick.AddListener(() => OnPanelClicked(capturedPanel));
            }

            
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Previous();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Next();

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) &&
            !EventSystem.current.currentSelectedGameObject)
        {
            SelectCurrent();
        }
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
                border.gameObject.SetActive(i == currentIndex);
        }
    }

    void SelectCurrent()
    {
        Debug.Log("Selecionaste com ENTER: " + categoryPanels[currentIndex].name);
        AbrirCategoria(categoryPanels[currentIndex]);
        
    }

    void OnPanelClicked(GameObject clickedPanel)
    {
        Debug.Log("Selecionaste com CLIQUE: " + clickedPanel.name);

        // Atualiza o índice atual
        for (int i = 0; i < categoryPanels.Length; i++)
        {
            if (categoryPanels[i] == clickedPanel)
            {
                currentIndex = i;
                HighlightSelected();
                break;
            }
        }

        AbrirCategoria(clickedPanel);
    }

    void AbrirCategoria(GameObject painelSelecionado)
    {
        Debug.Log("Abrindo categoria: " + painelSelecionado.name);

        // Desativa o menu principal de mods
        if (panelMods != null)
            panelMods.SetActive(false);

        // Desativa todos os painéis de categoria
        foreach (GameObject painel in categoryPanels)
        {
            painel.SetActive(false);
        }

        // Ativa só o painel da categoria selecionada
        painelSelecionado.SetActive(true);
    }
}





