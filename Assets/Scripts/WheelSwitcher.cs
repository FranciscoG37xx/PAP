using UnityEngine;

public class WheelSwitcher : MonoBehaviour
{
    public GameObject[] wheelPrefabs;
    public GameObject[] currentWheels;
    private GameObject[] activeWheels = new GameObject[4];

    private int currentIndex = 0;
    private string nomeJanteSelecionada = "";

    [Header("Correção de Rodas")]
    public Vector3[] wheelRotations = new Vector3[4];
    public Vector3 defaultWheelScale = Vector3.one;

    [Header("Offset de Posição")]
    public float offsetEsquerda = -0.05f;
    public float offsetDireita = 0.05f;

    [Header("Material da Jante")]
    public Material materialJante;  // ← Atribuir no inspetor

    public void SwitchToNextWheels()
    {
        currentIndex = (currentIndex + 1) % wheelPrefabs.Length;
        SwitchWheels(currentIndex);
    }

    public void SwitchToPreviousWheels()
    {
        currentIndex = (currentIndex - 1 + wheelPrefabs.Length) % wheelPrefabs.Length;
        SwitchWheels(currentIndex);
    }

    private void SwitchWheels(int index)
    {
        if (wheelPrefabs.Length == 0 || currentWheels.Length != 4 || wheelRotations.Length != 4)
        {
            Debug.LogWarning("Configuração incorreta de rodas.");
            return;
        }

        if (index == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (activeWheels[i] != null)
                    Destroy(activeWheels[i]);

                currentWheels[i].SetActive(true);

                for (int j = 0; j < currentWheels[i].transform.childCount; j++)
                {
                    Transform child = currentWheels[i].transform.GetChild(j);
                    string nameLower = child.name.ToLower();
                    if (!nameLower.Contains("brakedisc") && !nameLower.Contains("rimdark_in"))
                        child.gameObject.SetActive(true);
                }
            }

            nomeJanteSelecionada = "Original";
            Debug.Log("Jante original restaurada.");
            return;
        }

        nomeJanteSelecionada = wheelPrefabs[index].name.Replace("(Clone)", "").Trim();
        Debug.Log("Jante selecionada: " + nomeJanteSelecionada);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < currentWheels[i].transform.childCount; j++)
            {
                Transform child = currentWheels[i].transform.GetChild(j);
                string nameLower = child.name.ToLower();
                if (!nameLower.Contains("brakedisc") && !nameLower.Contains("rimdark_in"))
                    child.gameObject.SetActive(false);
            }

            if (activeWheels[i] != null)
                Destroy(activeWheels[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject newWheel = Instantiate(wheelPrefabs[index]);
            Transform refTransform = currentWheels[i].transform;

            newWheel.transform.SetParent(refTransform.parent);

            Transform rimBright = refTransform.Find("RimBright");
            Vector3 basePosition = (rimBright != null) ? rimBright.position : refTransform.position;

            newWheel.transform.position = basePosition;
            newWheel.transform.rotation = Quaternion.Euler(wheelRotations[i]);
            newWheel.transform.localScale = defaultWheelScale;

            // Corrigir centro visual
            Renderer refRenderer = refTransform.GetComponentInChildren<Renderer>();
            Renderer newRenderer = newWheel.GetComponentInChildren<Renderer>();

            if (refRenderer != null && newRenderer != null)
            {
                Vector3 centroOriginal = refRenderer.bounds.center;
                Vector3 centroNovo = newRenderer.bounds.center;
                Vector3 diferencaCentro = centroOriginal - centroNovo;
                newWheel.transform.position += diferencaCentro;
            }

            // Aplicar offset lateral
            float offsetX = (i == 0 || i == 2) ? offsetEsquerda : offsetDireita;
            Vector3 offset = refTransform.right * offsetX;
            newWheel.transform.position += offset;

            // ✅ Aplica o material às novas jantes
            if (materialJante != null)
            {
                Renderer[] renderers = newWheel.GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers)
                    rend.material = materialJante;
            }

            activeWheels[i] = newWheel;
        }
    }

    public void SetCurrentWheels(GameObject[] newReferenceWheels)
    {
        if (newReferenceWheels.Length == 4)
            currentWheels = newReferenceWheels;
        else
            Debug.LogWarning("São necessárias 4 rodas de referência.");
    }

    public string GetNomeJanteSelecionada()
    {
        return nomeJanteSelecionada;
    }
}



























