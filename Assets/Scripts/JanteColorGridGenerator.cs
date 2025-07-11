using UnityEngine;
using UnityEngine.UI;

public class JanteColorGridGenerator : MonoBehaviour
{
    public GameObject colorButtonPrefab;
    public Transform gridParent;
    public Material janteMaterialAlvo;
    public AudioSource audioSource;
    public AudioClip somSelecao;
    public Color[] realisticColors;

    private bool precoPinturaJanteAplicado = false;

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (Color color in realisticColors)
        {
            GameObject btn = Instantiate(colorButtonPrefab, gridParent);
            Image img = btn.GetComponent<Image>();
            if (img != null)
                img.color = color;

            Button button = btn.GetComponent<Button>();
            if (button != null)
            {
                Color capturedColor = color; // Importante para evitar referência errada na lambda
                button.onClick.AddListener(() => OnColorSelected(capturedColor));
            }
        }
    }

    public void OnColorSelected(Color selected)
    {
        Debug.Log("Cor de jante selecionada: " + selected);

        if (janteMaterialAlvo != null)
        {
            // Garantir que não estamos a modificar uma instância compartilhada
            janteMaterialAlvo = new Material(janteMaterialAlvo);
            janteMaterialAlvo.color = selected;
        }

        CarSelector carSelector = FindObjectOfType<CarSelector>();
        GameObject carroAtual = carSelector?.GetCarroAtual();

        if (carroAtual != null)
        {
            foreach (Transform child in carroAtual.GetComponentsInChildren<Transform>(true))
            {
                string lower = child.name.ToLower();
                if (!lower.Contains("rim") || lower.Contains("rimdark_in"))
                    continue;

                foreach (Renderer rend in child.GetComponentsInChildren<Renderer>(true))
                {
                    Material mat = rend.material; // Evita CS0131
                    if (mat.HasProperty("_Color"))
                        mat.color = selected;
                }
            }
        }

        WheelSwitcher wheelSwitcher = FindObjectOfType<WheelSwitcher>();
        if (wheelSwitcher != null)
            wheelSwitcher.corJanteSelecionada = selected;

        if (!precoPinturaJanteAplicado)
        {
            carSelector?.AplicarCustoPinturaJantes();
            precoPinturaJanteAplicado = true;
        }

        if (audioSource != null && somSelecao != null)
        {
            if (!audioSource.gameObject.activeInHierarchy)
                audioSource.gameObject.SetActive(true);

            audioSource.PlayOneShot(somSelecao);
        }
    }

    public void SetMaterialAlvo(Material novoMaterial)
    {
        janteMaterialAlvo = novoMaterial;
    }

    public void ResetarPreco()
    {
        precoPinturaJanteAplicado = false;
    }
}




