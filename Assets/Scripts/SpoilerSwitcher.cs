using UnityEngine;

public class SpoilerSwitcher : MonoBehaviour
{
    [Header("Configuração de Spoilers")]
    public GameObject[] spoilerPrefabs;         // Prefabs disponíveis para troca
    public GameObject originalSpoiler;          // Spoiler original do carro atual (serve de referência)
    private GameObject activeSpoiler;           // Spoiler atualmente ativo

    private int currentIndex = 0;

    public void SwitchToNextSpoiler()
    {
        currentIndex = (currentIndex + 1) % spoilerPrefabs.Length;
        SwitchSpoiler(currentIndex);
    }

    public void SwitchToPreviousSpoiler()
    {
        currentIndex = (currentIndex - 1 + spoilerPrefabs.Length) % spoilerPrefabs.Length;
        SwitchSpoiler(currentIndex);
    }

    private void SwitchSpoiler(int index)
{
    if (spoilerPrefabs.Length == 0 || originalSpoiler == null)
    {
        Debug.LogWarning("Spoilers ou referência original não atribuídos.");
        return;
    }

    if (activeSpoiler != null)
        Destroy(activeSpoiler);

    // Instanciar na posição/rotação/escala do original
    GameObject newSpoiler = Instantiate(spoilerPrefabs[index]);
    
    newSpoiler.transform.position = originalSpoiler.transform.position;
    newSpoiler.transform.rotation = originalSpoiler.transform.rotation;
    newSpoiler.transform.localScale = originalSpoiler.transform.localScale;

    // Mesmo parent do spoiler original
    newSpoiler.transform.SetParent(originalSpoiler.transform.parent);

    activeSpoiler = newSpoiler;
    FindObjectOfType<SpoilerColorToggle>()?.AplicarCorASpoilers();
    // Debug
    string nomeCarro = originalSpoiler.transform.root.name.ToLower();
    Debug.Log("Nome do carro raiz: " + nomeCarro);
    Debug.Log("Spoiler index: " + index);

    // Ajustes personalizados
    if (nomeCarro.Contains("audi_a7")) // substitui pelo nome real
    {
        if (index == 2)
        {
            Debug.Log("Aplicando ajuste para spoiler 1 (frente)");
            newSpoiler.transform.localPosition += new Vector3(0f, 0f, -0.05f);
        }
        else if (index == 4)
        {
            Debug.Log("Aplicando ajuste para spoiler 2 (cima)");
            newSpoiler.transform.localPosition += new Vector3(0f, 0.01f, 0.07f);
        }
    }
}


    public void ClearSpoiler()
    {
        if (activeSpoiler != null)
            Destroy(activeSpoiler);
    }

    public void SetOriginalSpoiler(GameObject novoSpoiler)
    {
        originalSpoiler = novoSpoiler;
    }

    public void ResetarEstado()
    {
        ClearSpoiler();
        currentIndex = 0;
    }
}



