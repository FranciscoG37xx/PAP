using UnityEngine;
using UnityEngine.UI;

public class ColorGridGenerator : MonoBehaviour
{
    public GameObject colorButtonPrefab;
    public Transform gridParent;

    public Texture2D pointerCursor;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("Referências")]
    public Renderer carroRenderer;

    public MeshRenderer[] porscheRenderers;

    public AudioSource audioSource;
    public AudioClip pintarSom;

    public int columns = 10;

    private bool gridGerado = false;

    private Color corSecundaria = new Color(0.05f, 0.05f, 0.05f); // Cor escura padrão para o spoiler

    private Color[][] colorRows = new Color[][]
    {
        new Color[] { Color.white, new Color(0.7f, 0.7f, 0.7f), Color.black },
        new Color[] { new Color(1f, 0.9f, 0.8f), new Color(1f, 0.5f, 0f), new Color(0.4f, 0.2f, 0f) },
        new Color[] { new Color(1f, 1f, 0.8f), new Color(1f, 0.9f, 0.1f), new Color(0.4f, 0.3f, 0f) },
        new Color[] { new Color(0.9f, 1f, 0.8f), new Color(0.6f, 0.8f, 0.1f), new Color(0.2f, 0.3f, 0f) },
        new Color[] { new Color(0.8f, 1f, 0.8f), new Color(0.2f, 0.7f, 0.2f), new Color(0f, 0.3f, 0f) },
        new Color[] { new Color(0.8f, 1f, 1f), new Color(0.1f, 0.8f, 0.6f), new Color(0f, 0.3f, 0.2f) },
        new Color[] { new Color(0.7f, 1f, 1f), new Color(0f, 0.6f, 0.8f), new Color(0f, 0.2f, 0.3f) },
        new Color[] { new Color(0.7f, 0.9f, 1f), new Color(0.1f, 0.4f, 1f), new Color(0f, 0f, 0.4f) },
        new Color[] { new Color(0.8f, 0.8f, 1f), new Color(0.4f, 0.2f, 0.8f), new Color(0.2f, 0f, 0.4f) },
        new Color[] { new Color(0.9f, 0.8f, 1f), new Color(0.7f, 0.3f, 1f), new Color(0.4f, 0f, 0.5f) },
        new Color[] { new Color(1f, 0.7f, 1f), new Color(0.8f, 0.1f, 0.5f), new Color(0.3f, 0f, 0.2f) },
        new Color[] { new Color(1f, 0.8f, 0.9f), new Color(1f, 0.4f, 0.6f), new Color(0.4f, 0f, 0.1f) },
        new Color[] { new Color(1f, 0.85f, 0.85f), new Color(0.9f, 0.2f, 0.2f), new Color(0.3f, 0f, 0f) }
    };

void Start()
{
    if (carroRenderer != null && CorPrimariaManager.Instance != null)
    {
        CorPrimariaManager.Instance.InicializarComCorRenderer(carroRenderer);
    }
}

    public void AtivarColorGrid()
    {
        if (!gridGerado)
        {
            GenerateColorGrid();
            gridGerado = true;
        }

        gameObject.SetActive(true);
    }

    public void GenerateColorGrid()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (Color[] row in colorRows)
        {
            Color bright = row[0];
            Color baseColor = row[1];
            Color dark = row[2];

            for (int x = 0; x < columns; x++)
            {
                float t = x / (float)(columns - 1);
                Color color = (t < 0.5f)
                    ? Color.Lerp(bright, baseColor, t * 2)
                    : Color.Lerp(baseColor, dark, (t - 0.5f) * 2);

                GameObject btn = Instantiate(colorButtonPrefab, gridParent);
                Image img = btn.GetComponent<Image>();
                if (img != null) img.color = color;

                Button b = btn.GetComponent<Button>();
                if (b != null)
                {
                    Color picked = color;
                    b.onClick.AddListener(() => OnColorSelected(picked));
                }
            }

            PointerCursorChanger changer = colorButtonPrefab.AddComponent<PointerCursorChanger>();
            changer.customCursor = pointerCursor;
            changer.cursorMode = CursorMode.Auto;
        }
    }

    public void OnColorSelected(Color selected)
    {
        Debug.Log("Cor selecionada: " + selected);


        string carroAtual = FindObjectOfType<CarSelector>()?.carroAtual?.name;

        Color corAnterior = Color.clear;

        if (carroAtual != null && carroAtual.ToLower().Contains("porsche") && porscheRenderers != null)
        {
            foreach (MeshRenderer rend in porscheRenderers)
            {
                foreach (Material mat in rend.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        corAnterior = mat.color;
                        mat.color = selected;
                        mat.SetFloat("_Metallic", 0.5f);
                        mat.SetFloat("_Glossiness", 0.8f);
                    }
                }
            }

            // Como Porsche não tem carroRenderer, criamos temporariamente um "fake" para o spoiler
            // Ou melhor, chamamos um método que pinta spoilers baseado no root do primeiro Porsche renderer
            if (porscheRenderers.Length > 0)
            {
                var root = porscheRenderers[0].transform.root;
                AplicarCorAoSpoilerSeNecessarioPeloRoot(root, selected);
            }
        }
        else if (carroRenderer != null) // Audi ou outros carros
        {
            Material mat = carroRenderer.material;
            corAnterior = mat.color;

            mat.color = selected;
            mat.SetFloat("_Metallic", 0.5f);
            mat.SetFloat("_Glossiness", 0.8f);

            AplicarCorAoSpoilerSeNecessario(corAnterior, selected);
        }

        // Atualiza no manager
        if (CorPrimariaManager.Instance != null)
            CorPrimariaManager.Instance.corAtualDoCarro = selected;

        // Aplica custo da pintura
        FindObjectOfType<CarSelector>()?.AplicarCustoPintura();

        // Atualiza o toggle do spoiler (se existir)
        var toggle = FindObjectOfType<SpoilerColorToggle>();
        if (toggle != null)
        {
            toggle.SetUltimaCorUsada(selected);
        }

        // Som
        if (audioSource != null && pintarSom != null)
        {
            if (!audioSource.enabled)
                audioSource.enabled = true;

            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(pintarSom);
        }
    
    if (HistoricoManager.Instance != null)
{
    string corHex = "#" + ColorUtility.ToHtmlStringRGB(selected);

    HistoricoManager.Instance.GuardarHistorico(new CustomizacaoData(
        "Pintura do carro aplicada",
        -1, // id da jante (não foi alterada)
        corHex,
        -1, // id do spoiler (não foi alterado)
        null, // cor da jante
        null  // cor do spoiler
    ));
}

}

// Método auxiliar para Porsche, aplicando cor ao spoiler a partir do root do primeiro renderer Porsche
void AplicarCorAoSpoilerSeNecessarioPeloRoot(Transform root, Color novaCor)
{
    Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);

    foreach (Renderer r in renderers)
    {
        if (!r.name.ToLower().Contains("spoiler")) continue;

        foreach (Material mat in r.materials)
        {
            if (mat.HasProperty("_Color") && mat.color != corSecundaria)
            {
                mat.color = novaCor;
            }
        }
    }
}







    void AplicarCorAoSpoilerSeNecessario(Color corAnterior, Color novaCor)
    {
        if (carroRenderer == null) return;

        Transform carro = carroRenderer.transform.root;
        Renderer[] renderers = carro.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            if (!r.name.ToLower().Contains("spoiler")) continue;

            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_Color") && mat.color != corSecundaria)
                {
                    mat.color = novaCor;
                }
            }
        }
    }

    public void SetCarRenderer(Renderer novoRenderer)
    {
        carroRenderer = novoRenderer;
    }
}













