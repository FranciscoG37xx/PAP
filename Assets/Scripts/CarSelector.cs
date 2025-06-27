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
        public GameObject janteOriginalPrefab;
    }

    public CarOption[] carrosDisponiveis;
    public Transform spawnPoint;
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI precoTexto;

    private GameObject carroAtual;
    private float precoAtual;

    void Start()
    {
        dropdown.ClearOptions();
        dropdown.options.Add(new TMP_Dropdown.OptionData("Escolher carro..."));
        foreach (var c in carrosDisponiveis)
            dropdown.options.Add(new TMP_Dropdown.OptionData(c.nome));

        dropdown.onValueChanged.AddListener(OnCarroSelecionado);
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

        FixarSombrasETagTransparente(carroAtual);


        AtualizarPrecoUI();
        ConfigurarSistemasDoCarro(carroAtual, index - 1);
    }

    void AtualizarPrecoUI()
    {
        precoTexto.text = (carroAtual != null) ? $"Preço: {precoAtual:0.00}€" : "Nenhum carro selecionado";
    }

    void ConfigurarSistemasDoCarro(GameObject carro, int carroIndex)
    {
        // Pintura
        var body = carro.transform.Find("Body");
        var colorGrid = FindObjectOfType<ColorGridGenerator>();
        if (body && colorGrid)
            colorGrid.SetCarRenderer(body.GetComponent<Renderer>());

        // Spoiler
        var spoilerSwitcher = FindObjectOfType<SpoilerSwitcher>();
        if (spoilerSwitcher != null)
        {
            var anchor = carro.transform.Find("SpoilerAnchor");
            if (anchor)
                spoilerSwitcher.SetOriginalSpoiler(anchor.gameObject);
            else
            {
                var spoiler = carro.transform.Find("Spoiler");
                if (spoiler)
                    spoilerSwitcher.SetOriginalSpoiler(spoiler.gameObject);
            }
            spoilerSwitcher.ResetarEstado();
        }

        // SpoilerColorToggle
        var spoilerColorToggle = FindObjectOfType<SpoilerColorToggle>();
        if (spoilerColorToggle != null)
        {
            spoilerColorToggle.spoilerPai = carro;
            if (body != null)
                spoilerColorToggle.carroRenderer = body.GetComponent<Renderer>();
        }

        // Jantes
        var janteManager = FindObjectOfType<WheelSwitcher>();
        if (janteManager != null)
        {
            janteManager.SetRodasDoCarro(carroIndex);

            // Define a jante original no index 0 do wheelPrefabs
            if (carroIndex >= 0 && carroIndex < carrosDisponiveis.Length)
            {
                GameObject janteOriginal = carrosDisponiveis[carroIndex].janteOriginalPrefab;
                if (janteOriginal != null && janteManager.wheelPrefabs.Length > 0)
                {
                    janteManager.wheelPrefabs[0] = janteOriginal;
                }
            }

            janteManager.ResetarEstado();
        }
    }

    public GameObject GetCarroAtual() => carroAtual;
    public float GetPrecoAtual() => precoAtual;
    public void SetPreco(float novoPreco)
    {
        precoAtual = novoPreco;
        AtualizarPrecoUI();
    }
    
    void FixarSombrasETagTransparente(GameObject carro)
{
    Renderer[] renderers = carro.GetComponentsInChildren<Renderer>(true);
    foreach (var rend in renderers)
    {
        if (rend.material != null && rend.material.shader != null)
        {
            string shaderName = rend.material.shader.name.ToLower();
            if (shaderName.Contains("transparent") || rend.name.ToLower().Contains("glass") || rend.name.ToLower().Contains("window"))
            {
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.material.renderQueue = 3000; // Força render transparente
            }
        }
    }
}

}




