using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModCategorySelector : MonoBehaviour
{
    public GameObject[] categoryPanels;   // Os painéis (categorias)
    public Button leftArrow;
    public Button rightArrow;

    private int currentIndex = 0;

    void Start()
    {
        HighlightSelected();

        leftArrow.onClick.AddListener(Previous);
        rightArrow.onClick.AddListener(Next);

        // Adicionar listeners de clique nos botões dentro dos painéis
        foreach (GameObject panel in categoryPanels)
        {
            Button panelButton = panel.GetComponentInChildren<Button>(); // ← procura o botão dentro do painel
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

    void AbrirCategoria(GameObject painel)
    {
        Debug.Log("Abrindo categoria: " + painel.name);

        // Aqui fazer o que quiser com a categoria selecionada
        // Exemplo: painel.transform.Find("SubMenu").gameObject.SetActive(true);
    }
}




