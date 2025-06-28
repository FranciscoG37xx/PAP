using UnityEngine;
using TMPro;

public class CarSelector : MonoBehaviour
{
    [System.Serializable]
    public class CarOption
    {
        public string nome;
        public GameObject cenaInstance;       // Objeto já presente na cena
        public float precoBase;
        public GameObject janteOriginalPrefab;
    }

    [Header("Referências")]
    public CarOption[] carrosDisponiveis;
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI precoTexto;

    private GameObject carroAtual;
    private float precoAtual;
    private int indexAtual = -1;

    // Flags de modificações aplicadas
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

        // Desativa todos os carros no início
        foreach (var c in carrosDisponiveis)
            if (c.cenaInstance != null)
                c.cenaInstance.SetActive(false);

        carroAtual = null;
        precoAtual = 0;
        AtualizarPrecoUI();
    }

    void OnCarroSelecionado(int index)
    {
        // Desativar carro atual
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
            var colorGrid = FindObjectOfType<ColorGridGenerator>();
            if (colorGrid != null)
                colorGrid.SetCarRenderer(renderer);

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
            if (anchor != null)
                spoilerSwitcher.SetOriginalSpoiler(anchor.gameObject);

            spoilerSwitcher.ResetarEstado();
        }

        var janteManager = FindObjectOfType<WheelSwitcher>();
        if (janteManager != null)
        {
            // Atribuir jante original no índice 0
            if (carroIndex < carrosDisponiveis.Length && carrosDisponiveis[carroIndex].janteOriginalPrefab != null)
                janteManager.wheelPrefabs[0] = carrosDisponiveis[carroIndex].janteOriginalPrefab;

            janteManager.SetRodasDoCarro(carroIndex);
            janteManager.ResetarEstado();

            var janteGrid = FindObjectOfType<JanteColorGridGenerator>();
            if (janteGrid != null && janteManager.materialJante != null)
                janteGrid.SetMaterialAlvo(janteManager.materialJante);
        }
    }

    // Métodos para aplicar custo apenas uma vez por modificação
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

    // Acesso externo
    public GameObject GetCarroAtual() => carroAtual;
    public float GetPrecoAtual() => precoAtual;

    public void SetPreco(float novoPreco)
    {
        precoAtual = novoPreco;
        AtualizarPrecoUI();
    }

    public int GetIndexAtual() => indexAtual;
}














