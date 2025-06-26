using UnityEngine;

public class SpoilerSwitcher : MonoBehaviour
{
    [Header("Configuração de Spoilers")]
    public GameObject[] spoilerPrefabs;
    public GameObject originalSpoiler;
    private GameObject activeSpoiler;
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

        GameObject newSpoiler = Instantiate(spoilerPrefabs[index]);
        newSpoiler.transform.position = originalSpoiler.transform.position;
        newSpoiler.transform.rotation = originalSpoiler.transform.rotation;
        newSpoiler.transform.localScale = originalSpoiler.transform.localScale;
        newSpoiler.transform.SetParent(originalSpoiler.transform.parent);

        activeSpoiler = newSpoiler;

        // Ajustes personalizados por carro
        string nomeCarro = originalSpoiler.transform.root.name.ToLower();
        if (nomeCarro.Contains("audi_a7"))
        {
            if (index == 2)
                newSpoiler.transform.localPosition += new Vector3(0f, 0f, -0.05f);
            else if (index == 4)
                newSpoiler.transform.localPosition += new Vector3(0f, 0.01f, 0.07f);
        }

        // Aplica a cor atual ao novo spoiler
        if (CorPrimariaManager.Instance != null)
        {
            Color cor = CorPrimariaManager.Instance.corAtualDoCarro;

            Renderer[] renderers = newSpoiler.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_Color"))
                        mat.color = cor;
                }
            }
        }

        // Aplica cor a todos os spoilers
        FindObjectOfType<SpoilerColorToggle>()?.AplicarCorASpoilers();
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




