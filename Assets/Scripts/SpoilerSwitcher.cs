using UnityEngine;

public class SpoilerSwitcher : MonoBehaviour
{
    public GameObject[] spoilerPrefabs;         // Prefabs disponíveis
    public Transform spoilerAnchor;             // Onde os spoilers serão colocados
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
        if (spoilerPrefabs.Length == 0 || spoilerAnchor == null)
        {
            Debug.LogWarning("Spoilers ou âncora não atribuídos.");
            return;
        }

        if (activeSpoiler != null)
            Destroy(activeSpoiler);

        GameObject newSpoiler = Instantiate(spoilerPrefabs[index], spoilerAnchor.position, spoilerAnchor.rotation, spoilerAnchor);
        activeSpoiler = newSpoiler;
    }

    public void ClearSpoiler()
    {
        if (activeSpoiler != null)
            Destroy(activeSpoiler);
    }
}

