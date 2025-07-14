using UnityEngine;

public class CorPrimariaManager : MonoBehaviour
{
    public static CorPrimariaManager Instance;

    public Color corAtualDoCarro = Color.gray;
    public bool usarCorDoCarro = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void InicializarComCorRenderer(Renderer carroRenderer)
    {
        if (carroRenderer != null && carroRenderer.material.HasProperty("_Color"))
        {
            corAtualDoCarro = carroRenderer.material.color;
        }
    }

    public void AplicarCor(Color novaCor)
    {
        GameObject carroAtual = GameObject.FindWithTag("CarroAtual");
        if (carroAtual == null) return;

        // Aplicar a cor ao body do carro
        Transform body = carroAtual.transform.Find("Body");
        if (body != null)
        {
            Renderer renderer = body.GetComponent<Renderer>();
            if (renderer != null && renderer.material.HasProperty("_Color"))
            {
                renderer.material.color = novaCor;
            }
        }

        // Atualizar a cor atual no manager
        corAtualDoCarro = novaCor;

        // Atualizar a cor do spoiler, se estiver em modo "usar cor do carro"
        SpoilerColorToggle toggle = FindObjectOfType<SpoilerColorToggle>();
        if (toggle != null && toggle.usarCorDoCarro)
        {
            toggle.AplicarCorASpoilers(); // <-- usa a versão SEM parâmetros, como pediste
        }
    }

    public string CorAtualHex => "#" + ColorUtility.ToHtmlStringRGB(corAtualDoCarro);
    
    public void AplicarCorHex(string hex)
{
    if (ColorUtility.TryParseHtmlString(hex, out Color cor))
    {
        GameObject carroAtual = GameObject.FindWithTag("CarroAtual");
if (carroAtual != null)
{
    Transform body = carroAtual.transform.Find("Body");
    if (body != null)
    {
        Renderer renderer = body.GetComponent<Renderer>();
        if (renderer != null && renderer.material.HasProperty("_Color"))
            renderer.material.color = cor;
    }
}
        corAtualDoCarro = cor;
    }
    else
    {
        Debug.LogWarning("Cor inválida (hex): " + hex);
    }
}


}



