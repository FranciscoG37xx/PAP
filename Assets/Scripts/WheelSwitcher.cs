using UnityEngine;

public class WheelSwitcher : MonoBehaviour
{
    public GameObject[] wheelPrefabs;           // Prefabs disponíveis
    public GameObject[] currentWheels;          // Referência das rodas atuais
    private GameObject[] activeWheels = new GameObject[4];

    private int currentIndex = 0;

    [Header("Correção de Rodas")]
    public Vector3[] wheelRotations = new Vector3[4]; // Rotação manual para cada roda
    public Vector3 defaultWheelScale = Vector3.one;   // ← ESCALA fixa a usar (ajustável no inspetor)

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

        // Restaurar rodas originais caso se volte à posição 0
        if (index == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (activeWheels[i] != null)
                    Destroy(activeWheels[i]);

                currentWheels[i].SetActive(true); // Reativa as rodas originais
            }

            return;
        }

        // Remove rodas anteriores
        for (int i = 0; i < 4; i++)
        {
            if (activeWheels[i] != null)
                Destroy(activeWheels[i]);

            currentWheels[i].SetActive(false); // Esconde originais
        }

        // Instancia as novas rodas
        for (int i = 0; i < 4; i++)
        {
            GameObject newWheel = Instantiate(wheelPrefabs[index]);

            Transform refTransform = currentWheels[i].transform;
            newWheel.transform.SetParent(refTransform.parent);
            newWheel.transform.position = refTransform.position;
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
}
















