using UnityEngine;

public class WheelSwitcher : MonoBehaviour
{
    public GameObject[] wheelPrefabs;

    [Header("Rodas de cada carro")]
    [SerializeField] private GameObject[] rodasAudi = new GameObject[4];
    [SerializeField] private GameObject[] rodasPorsche = new GameObject[4];

    private GameObject[] currentWheels = new GameObject[4];
    private GameObject[] activeWheels = new GameObject[4];

    private int currentIndex = 0;
    private string nomeJanteSelecionada = "";
    public Color corJanteSelecionada = Color.white;

    [Header("Correção de Rodas")]
    public Vector3[] wheelRotations = new Vector3[4];
    public Vector3 defaultWheelScale = Vector3.one;

    [Header("Offset de Posição")]
    public float offsetEsquerda = -0.05f;
    public float offsetDireita = 0.05f;

    [Header("Ajuste de profundidade por jante")]
    public float[] ajusteProfundidadePorJante = new float[4];

    [Header("Material da Jante")]
    public Material materialJante;

    public void SetRodasDoCarro(int carroIndex)
    {
        switch (carroIndex)
        {
            case 0: currentWheels = rodasAudi; break;
            case 1: currentWheels = rodasPorsche; break;
            default:
                Debug.LogWarning("Carro inválido ou rodas não definidas.");
                return;
        }

        ResetarEstado();
    }

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

                foreach (Transform child in currentWheels[i].transform)
                {
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
            foreach (Transform child in currentWheels[i].transform)
            {
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

            Renderer refRenderer = refTransform.GetComponentInChildren<Renderer>();
            Renderer newRenderer = newWheel.GetComponentInChildren<Renderer>();
            if (refRenderer != null && newRenderer != null)
            {
                Vector3 centroOriginal = refRenderer.bounds.center;
                Vector3 centroNovo = newRenderer.bounds.center;
                Vector3 diferencaCentro = centroOriginal - centroNovo;
                newWheel.transform.position += diferencaCentro;
            }

            float offsetX = (i == 0 || i == 2) ? offsetEsquerda : offsetDireita;
            Vector3 offset = refTransform.right * offsetX;

            if (ajusteProfundidadePorJante.Length == wheelPrefabs.Length)
            {
                float profundidade = ajusteProfundidadePorJante[index];
                bool isEsquerda = (i == 0 || i == 2);
                float profundidadeFinal = isEsquerda ? -profundidade : profundidade;
                offset += refTransform.right * profundidadeFinal;
            }

            newWheel.transform.position += offset;

            if (materialJante != null)
            {
                Renderer[] renderers = newWheel.GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers)
                {
                    rend.material = materialJante;
                    if (rend.material.HasProperty("_Color"))
                        rend.material.color = corJanteSelecionada;
                }
            }

            activeWheels[i] = newWheel;
        }
    }

    public string GetNomeJanteSelecionada() => nomeJanteSelecionada;

    public void ResetarEstado()
    {
        currentIndex = 0;
        SwitchWheels(0);
    }
}
































