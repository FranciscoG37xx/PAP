using UnityEngine;

public class WheelSwitcher : MonoBehaviour
{
    public GameObject[] wheelPrefabs;             // Prefabs de jantes disponíveis
    public GameObject[] currentWheels;            // As jantes atuais do carro, usadas como referência
    private GameObject[] activeWheels = new GameObject[4]; // Jantes atualmente ativas na cena

    private int currentIndex = 0;

    // Chamada pelas setas (direita)
    public void SwitchToNextWheels()
    {
        currentIndex = (currentIndex + 1) % wheelPrefabs.Length;
        SwitchWheels(currentIndex);
    }

    // Chamada pelas setas (esquerda)
    public void SwitchToPreviousWheels()
    {
        currentIndex = (currentIndex - 1 + wheelPrefabs.Length) % wheelPrefabs.Length;
        SwitchWheels(currentIndex);
    }

    private void SwitchWheels(int index)
    {
        // Validação
        if (wheelPrefabs.Length == 0 || currentWheels.Length != 4)
        {
            Debug.LogWarning("Certifica-te que tens 4 rodas atuais e prefabs definidos.");
            return;
        }

        // Destrói as rodas anteriores
        for (int i = 0; i < activeWheels.Length; i++)
        {
            if (activeWheels[i] != null)
                Destroy(activeWheels[i]);
        }

        // Instancia as novas rodas com base na posição, rotação e escala das atuais
        for (int i = 0; i < 4; i++)
        {
            GameObject newWheel = Instantiate(wheelPrefabs[index]);

            // Copiar posição, rotação e escala da roda atual
            newWheel.transform.position = currentWheels[i].transform.position;
            newWheel.transform.rotation = currentWheels[i].transform.rotation;
            newWheel.transform.localScale = currentWheels[i].transform.localScale;

            // Torna a nova roda filha da original (opcional)
            newWheel.transform.SetParent(currentWheels[i].transform.parent);

            activeWheels[i] = newWheel;
        }
    }

    // Atualizar rodas de referência ao mudar de carro
    public void SetCurrentWheels(GameObject[] newReferenceWheels)
    {
        if (newReferenceWheels.Length == 4)
        {
            currentWheels = newReferenceWheels;
        }
        else
        {
            Debug.LogWarning("As rodas de referência precisam de exatamente 4 objetos.");
        }
    }
}

