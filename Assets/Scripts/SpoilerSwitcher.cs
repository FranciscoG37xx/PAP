using UnityEngine;

public class SpoilerSwitcher : MonoBehaviour
{
    public GameObject[] spoilerPrefabs;
    public GameObject originalSpoiler;
    private GameObject activeSpoiler;
    private int currentIndex = 0;
    private bool precoAplicadoSpoiler = false;

    [Header("Correções específicas por carro")]
    public Vector3 posicaoCorrecao = Vector3.zero;
    public Vector3 rotacaoCorrecao = Vector3.zero;

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

        // Verifica se a cor foi alterada antes (para aplicar ou não a cor atual)
        bool usarCorSecundaria = true;
        if (activeSpoiler != null)
        {
            foreach (Renderer rend in activeSpoiler.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_Color") && mat.color != new Color(0.05f, 0.05f, 0.05f))
                    {
                        usarCorSecundaria = false;
                        break;
                    }
                }
            }
        }

        if (activeSpoiler != null) Destroy(activeSpoiler);

        GameObject newSpoiler = Instantiate(spoilerPrefabs[index]);
        newSpoiler.transform.SetParent(originalSpoiler.transform.parent);



        // Instanciar os materiais para evitar partilha
        foreach (Renderer rend in newSpoiler.GetComponentsInChildren<Renderer>(true))
        {
            Material[] mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
                mats[i] = new Material(mats[i]);
            rend.materials = mats;
        }

        // Aplicar posição e rotação com correção
        newSpoiler.transform.localPosition = originalSpoiler.transform.localPosition + posicaoCorrecao;
        newSpoiler.transform.localRotation = originalSpoiler.transform.localRotation * Quaternion.Euler(rotacaoCorrecao);

        activeSpoiler = newSpoiler;

        // Se o spoiler anterior tinha cor secundária, aplicar a cor atual
        if (!usarCorSecundaria && CorPrimariaManager.Instance != null)
        {
            foreach (Renderer rend in newSpoiler.GetComponentsInChildren<Renderer>(true))
            {
                foreach (Material mat in rend.materials)
                {
                    if (mat.HasProperty("_Color"))
                        mat.color = CorPrimariaManager.Instance.corAtualDoCarro;
                    if (mat.HasProperty("_Metallic"))
                        mat.SetFloat("_Metallic", 0.5f);
                    if (mat.HasProperty("_Glossiness"))
                        mat.SetFloat("_Glossiness", 0.8f);
                }
            }
        }

        // Aplicar novamente a cor se for necessário
        FindObjectOfType<SpoilerColorToggle>()?.AplicarCorASpoilers();

        AplicarPrecoSpoiler();

        // Ativar spoiler original apenas se o índice for 0
        originalSpoiler.SetActive(index == 0);
    }

    private void AplicarPrecoSpoiler()
    {
        if (!precoAplicadoSpoiler && currentIndex != 0)
        {
            FindObjectOfType<CarSelector>()?.AplicarCustoSpoiler();
            precoAplicadoSpoiler = true;
        }
        else if (currentIndex == 0)
        {
            precoAplicadoSpoiler = false;
        }
    }

    public void ClearSpoiler()
    {
        if (activeSpoiler != null) Destroy(activeSpoiler);
    }

    public void SetOriginalSpoiler(GameObject novo)
    {
        originalSpoiler = novo;
    }
    
    public void AplicarSpoiler(int id)
{
    if (id >= 0 && id < spoilerPrefabs.Length)
    {
        SwitchSpoiler(id); // método já existente para trocar o spoiler
    }
    else
    {
        Debug.LogWarning("ID de spoiler inválido ao restaurar histórico: " + id);
    }
}

}






