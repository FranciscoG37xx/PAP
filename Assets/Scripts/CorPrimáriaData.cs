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

    

}



