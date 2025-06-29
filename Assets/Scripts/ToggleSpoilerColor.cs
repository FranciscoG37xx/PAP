using UnityEngine;

public class SpoilerColorToggle : MonoBehaviour
{
    public GameObject spoilerPai;           // Objeto que contém os spoilers
    public Renderer carroRenderer;          // Renderer com a cor principal do carro

    [Header("Som ao pintar spoiler")]
    public AudioSource audioSource;
    public AudioClip somPintar;

    private Color corSecundaria = new Color(0.05f, 0.05f, 0.05f); // Fibra de carbono
    private Color ultimaCorUsada = Color.black; // Guarda a última cor aplicada

    public void AlternarCorSpoiler()
    {
        if (spoilerPai == null || carroRenderer == null)
        {
            Debug.LogWarning("SpoilerPai ou carroRenderer não está atribuído.");
            return;
        }

        Color corPrimaria = carroRenderer.material.color;
        bool algumComCorDoCarro = false;

        // Verifica se algum spoiler está com a cor do carro
        Renderer[] renderers = spoilerPai.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            if (!rend.name.ToLower().Contains("spoiler")) continue;

            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color") && CoresIguais(mat.color, corPrimaria))
                {
                    algumComCorDoCarro = true;
                    break;
                }
            }

            if (algumComCorDoCarro) break;
        }

        // Define a nova cor a aplicar
        ultimaCorUsada = algumComCorDoCarro ? corSecundaria : corPrimaria;

        // Aplica a todos os spoilers
        foreach (Renderer rend in renderers)
        {
            if (!rend.name.ToLower().Contains("spoiler")) continue;

            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = ultimaCorUsada;
            }
        }

        // Toca som se houver fonte e som atribuídos
        if (audioSource != null && somPintar != null)
            if (!audioSource.gameObject.activeInHierarchy)
                audioSource.gameObject.SetActive(true);
        audioSource.PlayOneShot(somPintar);
    }

    public void AplicarCorASpoilers()
    {
        if (spoilerPai == null) return;

        Renderer[] renderers = spoilerPai.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rend in renderers)
        {
            if (!rend.name.ToLower().Contains("spoiler")) continue;

            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = ultimaCorUsada;
            }
        }


    }

    public Color GetCorAtualDoSpoiler()
    {
        return ultimaCorUsada;
    }

    private bool CoresIguais(Color a, Color b, float tolerancia = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerancia &&
               Mathf.Abs(a.g - b.g) < tolerancia &&
               Mathf.Abs(a.b - b.b) < tolerancia;
    }

    public void SetUltimaCorUsada(Color novaCor)
    {
        ultimaCorUsada = novaCor;
    }




}






