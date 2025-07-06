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

    private GameObject carro => FindObjectOfType<CarSelector>()?.carroAtual;
    private Vector3 carroPosicaoOriginal;
    [SerializeField] private Vector3 deslocamentoPintura = new Vector3(2f, 0f, 0f);

    [Header("Câmara")]
    public Transform cameraTransform;
    public Transform janteFocusPoint;
    public float cameraZoomSpeed = 3f;
    public Vector3 cameraOffset = new Vector3(0f, 0.3f, -0.6f);

    private Vector3 cameraPosOriginal;
    private Quaternion cameraRotOriginal;

    void Start()
    {
        if (carro != null)
            carroPosicaoOriginal = carro.transform.position;

        if (cameraTransform != null)
        {
            cameraPosOriginal = cameraTransform.position;
            cameraRotOriginal = cameraTransform.rotation;
        }

        HighlightSelected();

        leftArrow.onClick.AddListener(Previous);
        rightArrow.onClick.AddListener(Next);

        for (int i = 0; i < categoryPanels.Length; i++)
        {
            int index = i;
            Button[] buttons = categoryPanels[i].GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
                btn.onClick.AddListener(() => OnPanelClicked(index));

            EventTrigger trigger = categoryPanels[i].GetComponent<EventTrigger>();
            if (trigger == null) trigger = categoryPanels[i].AddComponent<EventTrigger>();

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener((data) => OnPanelClicked(index));
            trigger.triggers.Add(entry);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Previous();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Next();

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            bool algumaCategoriaAtiva = false;
            foreach (GameObject panel in optionPanels)
            {
                if (panel.activeSelf)
                {
                    algumaCategoriaAtiva = true;
                    break;
                }
            }

            if (!algumaCategoriaAtiva)
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
        AbrirCategoria(currentIndex);
    }

    void OnPanelClicked(int index)
    {
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

        string nome = categoryPanels[index].name.ToLower();
        currentIndex = index;

        HighlightSelected();

        // Desativa todos os painéis
        foreach (GameObject panel in optionPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        // Ativa o painel correspondente
        GameObject painelOpcao = optionPanels[index];
        if (painelOpcao != null)
            painelOpcao.SetActive(true);

        // Só esconde panelMods se não for a troca de carro
        if (panelMods != null && !nome.Contains("carro"))
            panelMods.SetActive(false);

        if (nome.Contains("pintura"))
        {
            var colorGrid = FindObjectOfType<ColorGridGenerator>();
            if (colorGrid != null) colorGrid.AtivarColorGrid();

            if (carro != null)
            {
                carroPosicaoOriginal = carro.transform.position;
                if (moverCarroCoroutine != null) StopCoroutine(moverCarroCoroutine);
                moverCarroCoroutine = StartCoroutine(MoverCarroSuavemente(carroPosicaoOriginal + deslocamentoPintura));
            }
        }
        else if (nome.Contains("jantes"))
        {
            var janteGrid = FindObjectOfType<JanteColorGridGenerator>();
            if (janteGrid != null) janteGrid.gameObject.SetActive(true);

            if (cameraTransform != null && janteFocusPoint != null)
            {
                StopAllCoroutines();
                StartCoroutine(FocarCameraNaJante(janteFocusPoint.position + cameraOffset));
            }
        }
        else
        {
            ResetCameraPosition();
        }
    }

    // ---------- Fechar Categorias ----------
    public void FecharPainelDePintura()
    {
        FecharPainelPorNome("pintura");

        var colorGrid = FindObjectOfType<ColorGridGenerator>();
        if (colorGrid != null) colorGrid.gameObject.SetActive(false);

        if (carro != null)
        {
            if (moverCarroCoroutine != null) StopCoroutine(moverCarroCoroutine);
            moverCarroCoroutine = StartCoroutine(MoverCarroSuavemente(carroPosicaoOriginal));
        }

        HighlightSelected();
    }

    public void FecharPainelDeJantes()
    {
        FecharPainelPorNome("jantes");

        var janteGrid = FindObjectOfType<JanteColorGridGenerator>();
        if (janteGrid != null) janteGrid.gameObject.SetActive(false);

        ResetCameraPosition();
        HighlightSelected();
    }

    public void FecharPainelDeSpoiler()
    {
        FecharPainelPorNome("spoiler");
        ResetCameraPosition();
        HighlightSelected();
    }

    public void FecharPainelDeCarro()
    {
        FecharPainelPorNome("carro");
        ResetCameraPosition();
        HighlightSelected();
    }

    void FecharPainelPorNome(string nome)
    {
        foreach (GameObject panel in optionPanels)
        {
            if (panel != null && panel.name.ToLower().Contains(nome.ToLower()))
            {
                panel.SetActive(false);
                break;
            }
        }

        if (panelMods != null)
            panelMods.SetActive(true);
    }

    void ResetCameraPosition()
    {
        if (cameraTransform != null)
        {
            StopAllCoroutines();
            StartCoroutine(MoverCameraSuavemente(cameraPosOriginal, cameraRotOriginal));
        }
    }

    // ---------- Coroutines ----------
    IEnumerator MoverCarroSuavemente(Vector3 destino, float duracao = 0.6f)
    {
        Vector3 origem = carro.transform.position;
        float tempo = 0f;

        while (tempo < duracao)
        {
            carro.transform.position = Vector3.Lerp(origem, destino, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        carro.transform.position = destino;
    }

    IEnumerator FocarCameraNaJante(Vector3 destino)
    {
        Vector3 origem = cameraTransform.position;
        Quaternion rotOrigem = cameraTransform.rotation;
        Quaternion rotFinal = Quaternion.LookRotation(janteFocusPoint.position - destino);

        float duracao = 0.6f;
        float tempo = 0f;

        while (tempo < duracao)
        {
            cameraTransform.position = Vector3.Lerp(origem, destino, tempo / duracao);
            cameraTransform.rotation = Quaternion.Slerp(rotOrigem, rotFinal, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = destino;
        cameraTransform.rotation = rotFinal;
    }

    IEnumerator MoverCameraSuavemente(Vector3 destino, Quaternion rotFinal)
    {
        Vector3 origem = cameraTransform.position;
        Quaternion rotOrigem = cameraTransform.rotation;

        float duracao = 0.6f;
        float tempo = 0f;

        while (tempo < duracao)
        {
            cameraTransform.position = Vector3.Lerp(origem, destino, tempo / duracao);
            cameraTransform.rotation = Quaternion.Slerp(rotOrigem, rotFinal, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = destino;
        cameraTransform.rotation = rotFinal;
    }
}












