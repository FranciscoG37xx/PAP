using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorGridGenerator : MonoBehaviour
{
    public GameObject colorButtonPrefab; // O teu prefab do botão de cor
    public Transform gridParent;         // O painel com Grid Layout Group

    public int columns = 18;             // Número de colunas (como na imagem)
    public int rows = 12;                // Número de linhas

    void Start()
    {
        GenerateColorGrid();
    }

    void GenerateColorGrid()
    {
        // Limpa o painel antes
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < rows; y++)
        {
            float hue = (float)y / rows;

            for (int x = 0; x < columns; x++)
            {
                float value = 1f - (float)x / columns; // escurece da esquerda para direita
                Color color = Color.HSVToRGB(hue, 1f, value);

                GameObject colorButton = Instantiate(colorButtonPrefab, gridParent);
                Image img = colorButton.GetComponent<Image>();
                if (img != null) img.color = color;

                // (Opcional) Guardar a cor se quiseres usar depois
                Button btn = colorButton.GetComponent<Button>();
                if (btn != null)
                {
                    Color pickedColor = color;
                    btn.onClick.AddListener(() => OnColorSelected(pickedColor));
                }
            }
        }
    }

    void OnColorSelected(Color selected)
    {
        Debug.Log("Cor selecionada: " + selected);
        // Aqui podes aplicar a cor ao carro, por exemplo
        // carroRenderer.material.color = selected;
    }
}


