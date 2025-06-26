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

void OnColorSelected(Color selected)
{
    Debug.Log("Cor selecionada: " + selected);

    // Pintar o carro
    if (carroRenderer != null)
    {
        Material mat = carroRenderer.material;
        mat.color = selected;
        mat.SetFloat("_Metallic", 0.5f);
        mat.SetFloat("_Glossiness", 0.8f);
    }

    // Guardar a nova cor no manager
    if (CorPrimariaManager.Instance != null)
        CorPrimariaManager.Instance.corAtualDoCarro = selected;

    // Pintar todos os spoilers (ativos e inativos)
    GameObject[] todos = FindObjectsOfType<GameObject>(true);
    foreach (GameObject obj in todos)
    {
        if (!obj.name.ToLower().Contains("spoiler")) continue;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = selected;
            }
        }
    }

    // Atualiza a cor ativa do SpoilerColorToggle (se existir na cena)
    var toggle = FindObjectOfType<SpoilerColorToggle>();
    if (toggle != null)
    {
        toggle.SetUltimaCorUsada(selected);
    }

    // Som
    if (audioSource != null && pintarSom != null)
            if (!audioSource.gameObject.activeInHierarchy)
                audioSource.gameObject.SetActive(true);
            audioSource.PlayOneShot(pintarSom);
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













