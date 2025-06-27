using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarSelector : MonoBehaviour
{
    [System.Serializable]
    public class CarOption
    {
        public string nome;
        public GameObject prefab;
        public float precoBase;
    }

    public CarOption[] carrosDisponiveis;
    public Transform spawnPoint;
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI precoTexto;

    private GameObject carroAtual;
    private float precoAtual;

    void Start()
    {
        // Adicionar opção vazia como primeira
        dropdown.ClearOptions();
        dropdown.options.Add(new TMP_Dropdown.OptionData("Escolher carro..."));

        foreach (var c in carrosDisponiveis)
            dropdown.options.Add(new TMP_Dropdown.OptionData(c.nome));

        dropdown.onValueChanged.AddListener(OnCarroSelecionado);

        // Nenhum carro selecionado inicialmente
        AtualizarPrecoUI();
    }

    void OnCarroSelecionado(int index)
    {
        if (index == 0)
        {
            if (carroAtual != null)
            {
                Destroy(carroAtual);
                carroAtual = null;
            }

            precoAtual = 0;
            AtualizarPrecoUI();
            return;
        }

        if (carroAtual != null)
            Destroy(carroAtual);

        var carroInfo = carrosDisponiveis[index - 1];
        carroAtual = Instantiate(carroInfo.prefab, spawnPoint.position, spawnPoint.rotation);
        precoAtual = carroInfo.precoBase;

        AtualizarPrecoUI();

        // 🔧 Atualiza sistemas dependentes do carro atual
        ConfigurarSistemasDoCarro(carroAtual);
    }

    void AtualizarPrecoUI()
    {
        if (precoTexto != null)
        {
            precoTexto.text = (carroAtual != null)
                ? "Preço: " + precoAtual.ToString("0.00") + "€"
                : "Nenhum carro selecionado";
        }
    }

    public GameObject GetCarroAtual() => carroAtual;

    public float GetPrecoAtual() => precoAtual;

    public void SetPreco(float novoPreco)
    {
        precoAtual = novoPreco;
        AtualizarPrecoUI();
    }

    void ConfigurarSistemasDoCarro(GameObject carro)
    {
        //Pintura
        var colorGrid = FindObjectOfType<ColorGridGenerator>();
        if (colorGrid != null)
        {
            Renderer body = carro.transform.Find("Body")?.GetComponent<Renderer>();
            if (body != null)
                colorGrid.SetCarRenderer(body);
        }

        //Jantes
        var janteManager = FindObjectOfType<WheelSwitcher>();
        if (janteManager != null)
           // janteManager.SetReferenciasDoCarro(carro); // tu já tinhas este método
            janteManager.ResetarEstado();

        //Spoiler
        var spoilerSwitcher = FindObjectOfType<SpoilerSwitcher>();
        if (spoilerSwitcher != null)
        {
            // Se tiver anchor, usa. Senão, usa o original
            var anchor = carro.transform.Find("SpoilerAnchor");
            if (anchor != null)
                spoilerSwitcher.SetOriginalSpoiler(anchor.gameObject);
            else
            {
                var spoiler = carro.transform.Find("Spoiler")?.gameObject;
                if (spoiler != null)
                    spoilerSwitcher.SetOriginalSpoiler(spoiler);
            }
        }

        //SpoilerColorToggle
        var spoilerColorToggle = FindObjectOfType<SpoilerColorToggle>();
        if (spoilerColorToggle != null)
        {
            spoilerColorToggle.spoilerPai = carro;
            Renderer body = carro.transform.Find("Body")?.GetComponent<Renderer>();
            if (body != null)
                spoilerColorToggle.carroRenderer = body;
        }
    }
}


