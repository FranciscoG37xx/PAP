using UnityEngine;

public class SpoilerColorManager : MonoBehaviour
{
    public static SpoilerColorManager Instance;

    public Color corPrimariaAtual = Color.white;
    public Color corSecundariaPadrao = new Color(0.05f, 0.05f, 0.05f);

    public bool usarCorSecundaria = true;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AlternarCor()
    {
        usarCorSecundaria = !usarCorSecundaria;
        AtualizarSpoilersAtivos();
    }

    void AtualizarSpoilersAtivos()
    {
        var spoilers = GameObject.FindObjectsOfType<Renderer>();
        foreach (Renderer r in spoilers)
        {
            if (!r.name.ToLower().Contains("spoiler")) continue;

            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    mat.color = usarCorSecundaria ? corSecundariaPadrao : corPrimariaAtual;
                }
            }
        }
    }
}

