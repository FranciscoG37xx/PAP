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

        // Se voltar à jante original (posição 0)
        if (index == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (activeWheels[i] != null)
                    Destroy(activeWheels[i]);

                currentWheels[i].SetActive(true);

                // Ativa apenas os filhos (jantes, disco, etc.)
                for (int j = 0; j < currentWheels[i].transform.childCount; j++)
                    currentWheels[i].transform.GetChild(j).gameObject.SetActive(true);
            }

            nomeJanteSelecionada = "Original";
            Debug.Log("Jante original restaurada.");
            return;
        }

        // Atualiza o nome da jante
        nomeJanteSelecionada = wheelPrefabs[index].name.Replace("(Clone)", "").Trim();
        Debug.Log("Jante selecionada: " + nomeJanteSelecionada);

        // Esconde filhos das rodas atuais (mantém a borracha)
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < currentWheels[i].transform.childCount; j++)
                currentWheels[i].transform.GetChild(j).gameObject.SetActive(false);

            if (activeWheels[i] != null)
                Destroy(activeWheels[i]);
        }

        // Instancia jantes novas
        for (int i = 0; i < 4; i++)
        {
            GameObject newWheel = Instantiate(wheelPrefabs[index]);
            Transform refTransform = currentWheels[i].transform;

            newWheel.transform.SetParent(refTransform.parent);

            // Offset lateral conforme lado
            float offsetX = (i == 0 || i == 2) ? offsetEsquerda : offsetDireita;
            Vector3 offset = refTransform.TransformDirection(new Vector3(offsetX, 0f, 0f));

            newWheel.transform.position = refTransform.position + offset;
            newWheel.transform.rotation = Quaternion.Euler(wheelRotations[i]);
            newWheel.transform.localScale = defaultWheelScale;

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






















