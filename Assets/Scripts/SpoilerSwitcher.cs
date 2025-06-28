using UnityEngine;

public class SpoilerSwitcher : MonoBehaviour
{
    public GameObject[] spoilerPrefabs;
    public GameObject originalSpoiler;
    private GameObject activeSpoiler;
    private int currentIndex = 0;
    private bool precoAplicadoSpoiler = false;

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

        bool usarSec = true;
        if (activeSpoiler != null)
        {
            foreach (Renderer rend in activeSpoiler.GetComponentsInChildren<Renderer>(true))
                foreach (var mat in rend.materials)
                    if (mat.HasProperty("_Color") && mat.color != new Color(0.05f, 0.05f, 0.05f))
                        usarSec = false;
        }

        if (activeSpoiler != null) Destroy(activeSpoiler);

        GameObject newSpoiler = Instantiate(spoilerPrefabs[index],
            originalSpoiler.transform.position,
            originalSpoiler.transform.rotation,
            originalSpoiler.transform.parent);

        activeSpoiler = newSpoiler;

        if (!usarSec)
        {
            foreach (Renderer rend in newSpoiler.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_Color"))
                        mat.color = CorPrimariaManager.Instance?.corAtualDoCarro ?? Color.white;
                    if (mat.HasProperty("_Metallic"))
                        mat.SetFloat("_Metallic", 0.5f);
                    if (mat.HasProperty("_Glossiness"))
                        mat.SetFloat("_Glossiness", 0.8f);
                }
            }
        }

        FindObjectOfType<SpoilerColorToggle>()?.AplicarCorASpoilers();

        AplicarPrecoSpoiler();
    }

    private void AplicarPrecoSpoiler()
    {
        if (!precoAplicadoSpoiler && currentIndex != 0)
        {
            FindObjectOfType<CarSelector>()?.AplicarCustoSpoiler();
            precoAplicadoSpoiler = true;
        }
        else if (currentIndex == 0) precoAplicadoSpoiler = false;
    }

    public void ClearSpoiler()
    {
        if (activeSpoiler != null) Destroy(activeSpoiler);
    }

    public void SetOriginalSpoiler(GameObject novo)
    {
        originalSpoiler = novo;
    }

    public void ResetarEstado()
    {
        ClearSpoiler();
        currentIndex = 0;
        precoAplicadoSpoiler = false;
    }
}




