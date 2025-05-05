using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModCategorySelector : MonoBehaviour
{
    public GameObject[] categoryPanels; // Panel_Pintura, Panel_Capôs, etc. (clicáveis)
    public GameObject[] optionPanels;   // Panel_Pintura_Options, Panel_Capôs_Options, etc. (conteúdo)
    public Button leftArrow;
    public Button rightArrow;
    public GameObject panelMods;        // Panel_Mods principal

    private int currentIndex = 0;

    void Start()
    {
        HighlightSelected();

        leftArrow.onClick.AddListener(Previous);
        rightArrow.onClick.AddListener(Next);

        for (int i = 0; i < categoryPanels.Length; i++)
        {
            int index = i;
            GameObject panel = categoryPanels[i];

            // Liga botões filhos
            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                btn.onClick.AddListener(() => OnPanelClicked(index));
            }

            // Liga clique direto no painel
            EventTrigger trigger = panel.GetComponent<EventTrigger>();
            if (trigger == null) trigger = panel.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { OnPanelClicked(index); });
            trigger.triggers.Add(entry);
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
        AbrirCategoria(currentIndex);
    }

    void OnPanelClicked(int index)
    {
        Debug.Log("Selecionaste com CLIQUE: " + categoryPanels[index].name);
        currentIndex = index;
        HighlightSelected();
        AbrirCategoria(index);
    }

    void AbrirCategoria(int index)
    {
        if (index < 0 || index >= categoryPanels.Length || index >= optionPanels.Length)
        {
            Debug.LogWarning("Índice inválido ao abrir categoria.");
            return;
        }

        GameObject painelSelecionado = categoryPanels[index];
        GameObject painelOpcao = optionPanels[index];

        Debug.Log("Abrindo categoria: " + painelSelecionado.name);

        // Oculta o menu principal de mods
        if (panelMods != null)
            panelMods.SetActive(false);

        // Desativa todos os painéis de opções
        foreach (GameObject option in optionPanels)
        {
            if (option != null)
                option.SetActive(false);
        }

        // Ativa apenas o painel de opções correspondente
        if (painelOpcao != null)
            painelOpcao.SetActive(true);

        // Se for categoria de pintura, ativa o ColorGrid
        if (painelSelecionado.name.ToLower().Contains("pintura") || painelSelecionado.name.ToLower().Contains("cor"))
        {
            var colorGrid = FindObjectOfType<ColorGridGenerator>();
            if (colorGrid != null)
            {
                colorGrid.gameObject.SetActive(true);
                colorGrid.GenerateColorGrid();
            }
        }
        else
        {
            // Se não for pintura, certifica-se que o ColorGrid está escondido
            var colorGrid = FindObjectOfType<ColorGridGenerator>();
            if (colorGrid != null)
                colorGrid.gameObject.SetActive(false);
        }
    }
}







