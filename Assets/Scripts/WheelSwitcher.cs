using UnityEngine;

public class WheelSwitcher : MonoBehaviour
{
    public GameObject[] wheelPrefabs;
    [SerializeField] private GameObject[] rodasAudi = new GameObject[4];
    [SerializeField] private GameObject[] rodasPorsche = new GameObject[4];

    private GameObject[] currentWheels = new GameObject[4];
    private GameObject[] activeWheels = new GameObject[4];
    private int currentIndex = 0;
    private string nomeJanteSelecionada = "";
    public Color corJanteSelecionada = Color.white;
    public Vector3[] wheelRotations = new Vector3[4];
    public Vector3 escalaJanteAudi = Vector3.one;
    public Vector3 escalaJantePorsche = Vector3.one;
    private Vector3 escalaAtual = Vector3.one;
    public float offsetEsquerda = -0.05f;
    public float offsetDireita = 0.05f;
    public float[] ajusteProfundidadePorJanteAudi;
    public float[] ajusteProfundidadePorJantePorsche;
    private float[] profundidadesAtuais = null;
    public Material materialJante;
    public Renderer jantePintavel;
    public Color corPinturaAtual = Color.white;

    // Controle de preço por troca de jante
    private bool precoAplicadoJante = false;

    public void SetRodasDoCarro(int carroIndex)
    {
        switch (carroIndex)
        {
            case 0:
                currentWheels = rodasAudi;
                escalaAtual = escalaJanteAudi;
                profundidadesAtuais = ajusteProfundidadePorJanteAudi;
                break;

            case 1:
                currentWheels = rodasPorsche;
                escalaAtual = escalaJantePorsche;
                profundidadesAtuais = ajusteProfundidadePorJantePorsche;

                // Oculta jantes do Audi se ainda estiverem ativas
                GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in todos)
                {
                    if (obj.name.ToLower().Contains("_jante_rim"))
                        obj.SetActive(false);
                }
                break;

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

    public void SwitchWheels(int index)
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
                {
                    Destroy(activeWheels[i]);
                    activeWheels[i] = null;
                }

                if (currentWheels[i] != null)
                {
                    if (currentWheels == rodasPorsche)
                    {
                        // Para o Porsche, desativa o objeto pai (a roda inteira)
                        currentWheels[i].SetActive(true);
                    }
                    else
                    {
                        // Para o Audi, mantém o pai ativo e ativa todos os filhos (exceto brakedisc)
                        currentWheels[i].SetActive(true);
                        foreach (Transform child in currentWheels[i].transform)
                        {
                            if (!child.name.ToLower().Contains("brakedisc"))
                                child.gameObject.SetActive(true);
                        }
                    }
                }
            }

            nomeJanteSelecionada = "Original";
            precoAplicadoJante = false;
            Debug.Log("Jante original restaurada.");
        }
        else
        {
            nomeJanteSelecionada = wheelPrefabs[index].name.Replace("(Clone)", "").Trim();
            Debug.Log("Jante selecionada: " + nomeJanteSelecionada);

            for (int i = 0; i < 4; i++)
            {
                if (currentWheels[i] != null)
                {
                    if (currentWheels == rodasPorsche)
                    {
                        // Para Porsche, desativa o objeto pai inteiro
                        currentWheels[i].SetActive(false);
                    }
                    else
                    {
                        // Para Audi, desativa apenas os filhos (exceto brakedisc)
                        foreach (Transform child in currentWheels[i].transform)
                        {
                            if (!child.name.ToLower().Contains("brakedisc"))
                                child.gameObject.SetActive(false);
                        }
                    }
                }

                if (activeWheels[i] != null)
                {
                    Destroy(activeWheels[i]);
                    activeWheels[i] = null;
                }
            }

            float profundidade = (profundidadesAtuais != null && index < profundidadesAtuais.Length)
                                ? profundidadesAtuais[index] : 0f;

            for (int i = 0; i < 4; i++)
            {
                GameObject newWheel = Instantiate(wheelPrefabs[index]);

                //Duplicar materiais para evitar herança de cor
                foreach (Renderer rend in newWheel.GetComponentsInChildren<Renderer>(true))
                {
                    Material[] mats = rend.materials;
                    for (int j = 0; j < mats.Length; j++)
                        mats[j] = new Material(mats[j]);
                    rend.materials = mats;
                }

                Transform refT = currentWheels[i].transform;

                newWheel.transform.SetParent(refT.parent);

                Transform rimBright = refT.Find("RimBright");
                Vector3 basePos = rimBright != null ? rimBright.position : refT.position;
                newWheel.transform.position = basePos;
                newWheel.transform.rotation = Quaternion.Euler(wheelRotations[i]);
                newWheel.transform.localScale = escalaAtual;

                Renderer refR = refT.GetComponentInChildren<Renderer>();
                Renderer newR = newWheel.GetComponentInChildren<Renderer>();
                if (refR != null && newR != null)
                {
                    newWheel.transform.position += refR.bounds.center - newR.bounds.center;
                }

                Vector3 offset = refT.right * ((i % 2 == 0) ? offsetEsquerda : offsetDireita);
                offset += refT.right * ((i % 2 == 0) ? -profundidade : profundidade);
                newWheel.transform.position += offset;

                if (materialJante != null)
                {
                    foreach (Renderer rend in newWheel.GetComponentsInChildren<Renderer>())
                    {
                        rend.material = materialJante;
                        if (rend.material.HasProperty("_Color"))
                            rend.material.color = corJanteSelecionada;
                    }
                }

                activeWheels[i] = newWheel;
            }

            AplicarPrecoJante();
        }

        if (HistoricoManager.Instance != null && currentIndex != 0)
        {
            HistoricoManager.Instance.GuardarHistorico(new CustomizacaoData(
                "Jante trocada",
                index, // o ID da jante
                null,
                -1,
                null,
                null
            ));
        }

        currentIndex = index;

    }


    private void AplicarPrecoJante()
    {
        if (!precoAplicadoJante && currentIndex != 0)
        {
            FindObjectOfType<CarSelector>()?.AplicarCustoTrocaJantes();
            precoAplicadoJante = true;
        }
        else if (currentIndex == 0)
        {
            precoAplicadoJante = false;
        }
    }

    public string GetNomeJanteSelecionada() => nomeJanteSelecionada;

    public void ResetarEstado()
    {
        currentIndex = 0;
        SwitchWheels(0);
        precoAplicadoJante = false;
    }

    public void ResetarCorDasJantes(Color cor)
    {
        foreach (GameObject roda in currentWheels)
        {
            if (roda == null) continue;

            Renderer[] renderers = roda.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer rend in renderers)
            {
                if (!rend.name.ToLower().Contains("rim")) continue;

                foreach (Material mat in rend.materials)
                {
                    if (mat.HasProperty("_Color"))
                        mat.color = cor;
                }
            }
        }
    }

    public void AplicarJante(int id)
    {
        if (id >= 0 && id < wheelPrefabs.Length)
        {
            SwitchWheels(id); // este método já existe e troca a jante
        }
        else
        {
            Debug.LogWarning("ID de jante inválido ao restaurar histórico: " + id);
        }
    }

    public int JanteAtualID => currentIndex;
    public bool JanteEstaPintada => corJanteSelecionada != Color.white;

public void PintarJante()
{
    if (jantePintavel != null)
    {
        jantePintavel.material.color = corPinturaAtual;
        corJanteSelecionada = corPinturaAtual; // Atualiza o estado da cor pintada
    }
    else
    {
        Debug.LogWarning("Objeto de jante pintável não definido.");
    }
}



}







































