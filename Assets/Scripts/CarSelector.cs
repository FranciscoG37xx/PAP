using UnityEngine;
using TMPro;

public class CarSelector : MonoBehaviour
{
    [System.Serializable]
    public class CarOption
    {
        public string nome;
        public GameObject cenaInstance;
        public float precoBase;
        public GameObject janteOriginalPrefab;
        public GameObject spoilerOriginalPrefab;
        public Vector3 correcaoSpoilerPosicao = Vector3.zero;
        public Vector3 correcaoSpoilerRotacao = Vector3.zero;
        public Material materialCorOriginal;
    }

    [Header("Referências")]
    public CarOption[] carrosDisponiveis;
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI precoTexto;

    public GameObject carroAtual;
    private float precoAtual;
    private int indexAtual = -1;

    // Flags
    private bool pinturaAplicada = false;
    private bool jantesTrocaAplicada = false;
    private bool pinturaJantesAplicada = false;
    private bool spoilerAplicado = false;

    // Custos
    public const float custoPinturaCarro = 1000f;
    public const float custoJanteNova = 500f;
    public const float custoPinturaJante = 150f;
    public const float custoSpoilerNovo = 800f;

    void Start()
    {
        dropdown.ClearOptions();
        dropdown.options.Add(new TMP_Dropdown.OptionData("Escolher carro..."));

        foreach (var c in carrosDisponiveis)
            dropdown.options.Add(new TMP_Dropdown.OptionData(c.nome));

        dropdown.onValueChanged.AddListener(OnCarroSelecionado);

        foreach (var c in carrosDisponiveis)
            if (c.cenaInstance != null)
                c.cenaInstance.SetActive(false);

        carroAtual = null;
        precoAtual = 0;
        AtualizarPrecoUI();
    }

    void OnCarroSelecionado(int index)
    {
        if (carroAtual != null)
            carroAtual.SetActive(false);

        if (index == 0)
        {
            carroAtual = null;
            precoAtual = 0;
            indexAtual = -1;
            AtualizarPrecoUI();
            return;
        }

        indexAtual = index - 1;
        CarOption opcao = carrosDisponiveis[indexAtual];

        carroAtual = opcao.cenaInstance;
        if (carroAtual != null)
            carroAtual.SetActive(true);

        precoAtual = opcao.precoBase;
        AtualizarPrecoUI();

        ResetarModificacoes();
        ConfigurarSistemasDoCarro(carroAtual, indexAtual);
        ResetarModificacoes();
    }

    void ResetarModificacoes()
    {
        pinturaAplicada = false;
        jantesTrocaAplicada = false;
        pinturaJantesAplicada = false;
        spoilerAplicado = false;
    }

    void AtualizarPrecoUI()
    {
        precoTexto.text = carroAtual != null
            ? $"Preço: {precoAtual:0.00}€"
            : "Nenhum carro selecionado";
    }

    void ConfigurarSistemasDoCarro(GameObject carro, int carroIndex)
    {
        if (carro == null) return;

        var body = carro.transform.Find("Body");
        if (body != null)
        {
            var renderer = body.GetComponent<Renderer>();

            // ⬇️ Inicializa o CorPrimariaManager com o renderer do carro atual
            if (CorPrimariaManager.Instance != null)
                CorPrimariaManager.Instance.InicializarComCorRenderer(renderer);

            var colorGrid = FindObjectOfType<ColorGridGenerator>();
            if (colorGrid != null)
                colorGrid.SetCarRenderer(renderer);

                // Atualiza o CorPrimariaManager com a nova cor inicial do carro
CorPrimariaManager.Instance.InicializarComCorRenderer(renderer);

// Aplica a cor atual aos spoilers do carro (se houverem)
var spoilers = carro.GetComponentsInChildren<Renderer>(includeInactive: true);
foreach (var r in spoilers)
{
    if (r.gameObject.name.ToLower().Contains("spoiler"))
    {
        if (r.material.HasProperty("_Color"))
            r.material.color = CorPrimariaManager.Instance.corAtualDoCarro;
    }
}


            var spoilerColorToggle = FindObjectOfType<SpoilerColorToggle>();
            if (spoilerColorToggle != null)
            {
                spoilerColorToggle.spoilerPai = carro;
                spoilerColorToggle.carroRenderer = renderer;
            }
        }

        var spoilerSwitcher = FindObjectOfType<SpoilerSwitcher>();
        if (spoilerSwitcher != null)
        {
            Transform anchor = carro.transform.Find("SpoilerAnchor") ?? carro.transform.Find("Spoiler");
            if (carroIndex < carrosDisponiveis.Length && carrosDisponiveis[carroIndex].spoilerOriginalPrefab != null)
                spoilerSwitcher.SetOriginalSpoiler(carrosDisponiveis[carroIndex].spoilerOriginalPrefab);
            else if (anchor != null)
                spoilerSwitcher.SetOriginalSpoiler(anchor.gameObject);

            spoilerSwitcher.posicaoCorrecao = carrosDisponiveis[carroIndex].correcaoSpoilerPosicao;
            spoilerSwitcher.rotacaoCorrecao = carrosDisponiveis[carroIndex].correcaoSpoilerRotacao;
        }

        var janteManager = FindObjectOfType<WheelSwitcher>();
        if (janteManager != null)
        {
            if (carroIndex < carrosDisponiveis.Length && carrosDisponiveis[carroIndex].janteOriginalPrefab != null)
                janteManager.wheelPrefabs[0] = carrosDisponiveis[carroIndex].janteOriginalPrefab;

            janteManager.SetRodasDoCarro(carroIndex);
            janteManager.ResetarEstado();

            var janteGrid = FindObjectOfType<JanteColorGridGenerator>();
            if (janteGrid != null && janteManager.materialJante != null)
                janteGrid.SetMaterialAlvo(janteManager.materialJante);
        }
    }

    public void AplicarCustoPintura()
    {
        if (!pinturaAplicada)
        {
            precoAtual += custoPinturaCarro;
            pinturaAplicada = true;
            AtualizarPrecoUI();
        }
    }

    public void AplicarCustoTrocaJantes()
    {
        if (!jantesTrocaAplicada)
        {
            precoAtual += custoJanteNova;
            jantesTrocaAplicada = true;
            AtualizarPrecoUI();
        }
    }

    public void AplicarCustoPinturaJantes()
    {
        if (!pinturaJantesAplicada)
        {
            precoAtual += custoPinturaJante;
            pinturaJantesAplicada = true;
            AtualizarPrecoUI();
        }
    }

    public void AplicarCustoSpoiler()
    {
        if (!spoilerAplicado)
        {
            precoAtual += custoSpoilerNovo;
            spoilerAplicado = true;
            AtualizarPrecoUI();
        }
    }

    public GameObject GetCarroAtual() => carroAtual;
    public float GetPrecoAtual() => precoAtual;

    public void SetPreco(float novoPreco)
    {
        precoAtual = novoPreco;
        AtualizarPrecoUI();
    }

    public int GetIndexAtual() => indexAtual;
}















