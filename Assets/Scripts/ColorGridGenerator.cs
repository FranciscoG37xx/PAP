using UnityEngine;
using UnityEngine.UI;

public class ColorGridGenerator : MonoBehaviour
{
    public GameObject colorButtonPrefab;
    public Transform gridParent;

    public Texture2D pointerCursor;
    public CursorMode cursorMode = CursorMode.Auto;

    public Renderer carroRenderer; // Assign o Renderer do carro
    public AudioSource audioSource; // O componente que vai tocar o som
    public AudioClip pintarSom;    // O som a tocar quando escolhe uma cor

    public int columns = 10;

    // Cada linha representa uma gama de cor: [claro, médio (principal), escuro]
    private Color[][] colorRows = new Color[][]
    {
        new Color[] { Color.white, new Color(0.7f, 0.7f, 0.7f), Color.black },                     // 1. Escala de cinzentos
        new Color[] { new Color(1f, 0.9f, 0.8f), new Color(1f, 0.5f, 0f), new Color(0.4f, 0.2f, 0f) },   // 2. Laranja
        new Color[] { new Color(1f, 1f, 0.8f), new Color(1f, 0.9f, 0.1f), new Color(0.4f, 0.3f, 0f) },   // 3. Amarelo
        new Color[] { new Color(0.9f, 1f, 0.8f), new Color(0.6f, 0.8f, 0.1f), new Color(0.2f, 0.3f, 0f) }, // 4. Verde-limão
        new Color[] { new Color(0.8f, 1f, 0.8f), new Color(0.2f, 0.7f, 0.2f), new Color(0f, 0.3f, 0f) },   // 5. Verde
        new Color[] { new Color(0.8f, 1f, 1f), new Color(0.1f, 0.8f, 0.6f), new Color(0f, 0.3f, 0.2f) },   // 6. Verde-água
        new Color[] { new Color(0.7f, 1f, 1f), new Color(0f, 0.6f, 0.8f), new Color(0f, 0.2f, 0.3f) },     // 7. Ciano
        new Color[] { new Color(0.7f, 0.9f, 1f), new Color(0.1f, 0.4f, 1f), new Color(0f, 0f, 0.4f) },     // 8. Azul
        new Color[] { new Color(0.8f, 0.8f, 1f), new Color(0.4f, 0.2f, 0.8f), new Color(0.2f, 0f, 0.4f) }, // 9. Azul-roxo
        new Color[] { new Color(0.9f, 0.8f, 1f), new Color(0.7f, 0.3f, 1f), new Color(0.4f, 0f, 0.5f) },   // 10. Roxo
        new Color[] { new Color(1f, 0.7f, 1f), new Color(0.8f, 0.1f, 0.5f), new Color(0.3f, 0f, 0.2f) },   // 11. Magenta
        new Color[] { new Color(1f, 0.8f, 0.9f), new Color(1f, 0.4f, 0.6f), new Color(0.4f, 0f, 0.1f) },   // 12. Rosa
        new Color[] { new Color(1f, 0.85f, 0.85f), new Color(0.9f, 0.2f, 0.2f), new Color(0.3f, 0f, 0f) }  // 13. Vermelho
    };

    void Start()
    {
        GenerateColorGrid();
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
                Color color;

                if (t < 0.5f)
                    color = Color.Lerp(bright, baseColor, t * 2);
                else
                    color = Color.Lerp(baseColor, dark, (t - 0.5f) * 2);

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

        if (carroRenderer != null)
        {
            Material mat = carroRenderer.material;
            mat.color = selected;

            // Aumenta o brilho/gloss e metalicidade
            mat.SetFloat("_Metallic", 0.5f);    
            mat.SetFloat("_Glossiness", 0.8f);  // Quanto mais alto, mais polido
        }

        if (audioSource != null && pintarSom != null)
        {
            audioSource.PlayOneShot(pintarSom);
        }
    }
}











