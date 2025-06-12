using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ModCategorySelector : MonoBehaviour
{
    public GameObject[] categoryPanels;
    public GameObject[] optionPanels;
    public Button leftArrow;
    public Button rightArrow;
    public GameObject panelMods;

    private Coroutine moverCarroCoroutine;
    private int currentIndex = 0;

    [SerializeField] private GameObject carro;
    private Vector3 carroPosicaoOriginal;
    [SerializeField] private Vector3 deslocamentoPintura = new Vector3(2f, 0f, 0f);

    void Start()
    {
        if (carro != null)
        {
            carroPosicaoOriginal = carro.transform.position;
            Debug.Log("Posição original do carro: " + carroPosicaoOriginal);
        }

        HighlightSelected();

        leftArrow.onClick.AddListener(Previous);
        rightArrow.onClick.AddListener(Next);

        for (int i = 0; i < categoryPanels.Length; i++)
        {
            int index = i;
            GameObject panel = categoryPanels[i];

            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                btn.onClick.AddListener(() => OnPanelClicked(index));
            }

            EventTrigger trigger = panel.GetComponent<EventTrigger>();
            if (trigger == null) trigger = panel.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
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

        if (panelMods != null)
            panelMods.SetActive(false);

        foreach (GameObject option in optionPanels)
        {
            if (option != null)
                option.SetActive(false);
        }

        if (painelOpcao != null)
            painelOpcao.SetActive(true);

        bool isPintura = painelSelecionado.name.ToLower().Contains("pintura") || painelSelecionado.name.ToLower().Contains("cor");

        var colorGrid = FindObjectOfType<ColorGridGenerator>();

        if (colorGrid != null)
        {
            if (isPintura)
                colorGrid.AtivarColorGrid();
            else
                colorGrid.gameObject.SetActive(false);
        }
        if (carro != null)
        {
            Vector3 destino = isPintura ? carroPosicaoOriginal + deslocamentoPintura : carroPosicaoOriginal;
            if (moverCarroCoroutine != null) StopCoroutine(moverCarroCoroutine);
            moverCarroCoroutine = StartCoroutine(MoverCarroSuavemente(destino, 0.6f));
        }
        
        if (painelSelecionado.name.ToLower().Contains("jantes"))
       {
        var grid = FindObjectOfType<JanteColorGridGenerator>();
         if (grid != null)
         grid.gameObject.SetActive(true);
       }

    }

    public void FecharPainelDePintura()
    {
        GameObject painelPintura = null;
        foreach (GameObject painel in optionPanels)
        {
            if (painel != null && painel.name.ToLower().Contains("pintura"))
            {
                painel.SetActive(false);
                painelPintura = painel;
                break;
            }
        }

        if (panelMods != null)
            panelMods.SetActive(true);

        var colorGrid = FindObjectOfType<ColorGridGenerator>();
        if (colorGrid != null)
            colorGrid.gameObject.SetActive(false);

        if (carro != null)
        {
            if (moverCarroCoroutine != null) StopCoroutine(moverCarroCoroutine);
            moverCarroCoroutine = StartCoroutine(MoverCarroSuavemente(carroPosicaoOriginal, 0.6f));
        }
    }

    IEnumerator MoverCarroSuavemente(Vector3 destino, float duracao = 0.5f)
    {
        Vector3 origem = carro.transform.position;
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracao)
        {
            carro.transform.position = Vector3.Lerp(origem, destino, tempoDecorrido / duracao);
            tempoDecorrido += Time.deltaTime;
            yield return null;
        }

        carro.transform.position = destino;
    }
}








