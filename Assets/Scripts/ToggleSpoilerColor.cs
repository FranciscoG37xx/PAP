using UnityEngine;

public class SpoilerColorToggle : MonoBehaviour
{
    public GameObject spoilerPai;           // Carro ou objeto que contém os spoilers instanciados
    public Renderer carroRenderer;          // Renderer com a cor principal do carro
    private bool usarCorPrimaria = false;

    private Color ultimaCorUsada = Color.black; // Começa com secundária por padrão

    public void AlternarCorSpoiler()
    {
        if (spoilerPai == null || carroRenderer == null)
        {
            Debug.LogWarning("SpoilerPai ou carroRenderer não está atribuído.");
            return;
        }

        // Alternar a cor
        if (usarCorPrimaria)
            ultimaCorUsada = Color.black; // Cor secundária
        else
            ultimaCorUsada = carroRenderer.material.color; // Cor do carro

        // Aplica a cor a todos os spoilers ativos (ou não)
        AplicarCorASpoilers();

        usarCorPrimaria = !usarCorPrimaria;
    }

    public void AplicarCorASpoilers()
    {
        if (spoilerPai == null) return;

        Renderer[] renderers = spoilerPai.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            if (rend.name.ToLower().Contains("spoiler"))
            {
                foreach (Material mat in rend.materials)
                {
                    if (mat.HasProperty("_Color"))
                        mat.color = ultimaCorUsada;
                }
            }
        }
    }

    //Usa esta função no SpoilerSwitcher após instanciar o novo spoiler
    public Color GetCorAtualDoSpoiler()
    {
        return ultimaCorUsada;
    }
}



