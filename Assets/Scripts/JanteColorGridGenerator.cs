using UnityEngine;
using UnityEngine.UI;

public class JanteColorGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject colorButtonPrefab;
    public Transform gridParent;
    public Material janteMaterialAlvo; // ← Material a aplicar nas jantes

    [Header("Som opcional")]
    public AudioSource audioSource;
    public AudioClip somSelecao;

    [Header("Configuração")]
    public Color[] realisticColors = new Color[]
    {
        new Color(0.9f, 0.9f, 0.9f),    // Prateado claro
        new Color(0.6f, 0.6f, 0.6f),    // Prateado escuro
        new Color(0.3f, 0.3f, 0.3f),    // Antracite
        new Color(0.1f, 0.1f, 0.1f),    // Preto
        new Color(0.8f, 0.7f, 0.5f),    // Bronze
        new Color(0.6f, 0.5f, 0.3f),    // Ouro queimado
        new Color(0.9f, 0.8f, 0.7f),    // Champanhe
    };

    void Start()
    {
        GenerateGrid(); // ← Gera uma vez e depois fica desativado no painel até ser mostrado
    }

    public void GenerateGrid()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (Color color in realisticColors)
        {
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
    }

    private void OnColorSelected(Color selected)
    {
        Debug.Log("Cor de jante selecionada: " + selected);

        if (janteMaterialAlvo != null)
        {
            janteMaterialAlvo.color = selected;
        }

        if (audioSource != null && somSelecao != null)
        {
            audioSource.PlayOneShot(somSelecao);
        }
    }
}
