using UnityEngine;

public class WheelSwitcher : MonoBehaviour
{
    public GameObject[] wheelPrefabs;      // Prefabs das jantes disponíveis
    public GameObject[] currentWheels;     // Jantes atuais do carro, referência de posição
    private GameObject[] activeWheels = new GameObject[4];

    private int currentIndex = 0;

    // Referência opcional para ajustar rotação e escala padrão das jantes
    public Vector3 defaultWheelRotation = Vector3.zero;
    public Vector3 defaultWheelScale = Vector3.one;

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
        if (wheelPrefabs.Length == 0 || currentWheels.Length != 4)
        {
            Debug.LogWarning("Faltam prefabs ou referência de rodas!");
            return;
        }

        // Apagar rodas ativas antigas
        for (int i = 0; i < activeWheels.Length; i++)
        {
            if (activeWheels[i] != null)
                Destroy(activeWheels[i]);
        }

        // Instanciar novas jantes nas posições corretas
        for (int i = 0; i < 4; i++)
        {
            GameObject newWheel = Instantiate(wheelPrefabs[index]);

            Transform refWheel = currentWheels[i].transform;

            newWheel.transform.SetParent(refWheel.parent);
            newWheel.transform.position = refWheel.position;
            newWheel.transform.rotation = Quaternion.Euler(defaultWheelRotation);
            newWheel.transform.localScale = defaultWheelScale;

            activeWheels[i] = newWheel;

            // Opcional: desativar a roda original visível
            currentWheels[i].SetActive(false);
        }
    }

    public void SetCurrentWheels(GameObject[] newReferenceWheels)
    {
        if (newReferenceWheels.Length == 4)
        {
            currentWheels = newReferenceWheels;
        }
        else
        {
            Debug.LogWarning("Precisam ser exatamente 4 rodas de referência.");
        }
    }
}




