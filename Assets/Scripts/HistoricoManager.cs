using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HistoricoManager : MonoBehaviour
{
    public static HistoricoManager Instance { get; private set; }

    [Header("Referências UI")]
    [Tooltip("Painel que será ativado/desativado ao abrir/fechar o histórico")]
    public GameObject historicoPanel;

    [Tooltip("Transform Content do ScrollView onde aparecem os itens de histórico")]
    public Transform content;

    [Tooltip("Prefab de um item de histórico (com Text e Button)")]
    public GameObject historicoItemPrefab;

    // Lista interna que guarda as customizações
    private List<CustomizacaoData> historicoGuardado = new List<CustomizacaoData>();

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (historicoPanel != null)
            historicoPanel.SetActive(false);
    }

    public void AlternarHistorico()
    {
        bool ativo = historicoPanel.activeSelf;
        historicoPanel.SetActive(!ativo);

        if (!ativo)
            AtualizarListaHistorico();
    }

    public void FecharHistorico()
    {
        if (historicoPanel != null)
            historicoPanel.SetActive(false);
    }

    public void GuardarHistorico(CustomizacaoData data)
    {
        historicoGuardado.Add(data);
        Debug.Log("Histórico guardado.");
    }

    void AtualizarListaHistorico()
    {
        foreach (Transform filho in content)
            Destroy(filho.gameObject);

        foreach (CustomizacaoData item in historicoGuardado)
        {
            GameObject novoItem = Instantiate(historicoItemPrefab, content);

            TMPro.TextMeshProUGUI texto = novoItem.GetComponentInChildren<TMPro.TextMeshProUGUI>();
if (texto != null)
    texto.text = item.nomeExibicao;

            Button botao = novoItem.GetComponentInChildren<Button>();
            if (botao != null)
            {
                CustomizacaoData copia = item;
                botao.onClick.AddListener(() => RestaurarCustomizacao(copia));
            }
        }
    }

    void RestaurarCustomizacao(CustomizacaoData data)
    {
        Debug.Log("Restaurar histórico.");
        AplicarCustomizacaoDoHistorico(data);
    }

    public void AplicarCustomizacaoDoHistorico(CustomizacaoData data)
    {
        CarSelector carSelector = FindObjectOfType<CarSelector>();
        if (carSelector == null || carSelector.GetCarroAtual() == null) return;

        GameObject carro = carSelector.GetCarroAtual();

        // Aplicar cor do carro
        if (!string.IsNullOrEmpty(data.corHex))
        {
            if (ColorUtility.TryParseHtmlString(data.corHex, out Color cor))
            {
                Transform body = carro.transform.Find("Body");
                if (body != null)
                {
                    Renderer renderer = body.GetComponent<Renderer>();
                    if (renderer != null)
                        renderer.material.color = cor;

                    if (CorPrimariaManager.Instance != null)
                        CorPrimariaManager.Instance.corAtualDoCarro = cor;
                }
            }
        }

        // Aplicar jante e cor da jante
        WheelSwitcher wheelSwitcher = FindObjectOfType<WheelSwitcher>();
        if (wheelSwitcher != null)
        {
            wheelSwitcher.SwitchWheels(data.idJante);

            if (!string.IsNullOrEmpty(data.corJanteHex) &&
                ColorUtility.TryParseHtmlString(data.corJanteHex, out Color corJante))
            {
                JanteColorGridGenerator janteColor = FindObjectOfType<JanteColorGridGenerator>();
                if (janteColor != null)
                {
                    janteColor.ResetarPreco(); // Evita aplicar custo novamente
                    janteColor.OnColorSelected(corJante);
                }
            }
        }

        // Aplicar spoiler e cor do spoiler
        SpoilerSwitcher spoilerSwitcher = FindObjectOfType<SpoilerSwitcher>();
        if (spoilerSwitcher != null)
        {
            spoilerSwitcher.AplicarSpoiler(data.idSpoiler);

            if (!string.IsNullOrEmpty(data.corSpoilerHex) &&
                ColorUtility.TryParseHtmlString(data.corSpoilerHex, out Color corSpoiler))
            {
                SpoilerColorManager manager = FindObjectOfType<SpoilerColorManager>();
                if (manager != null)
                {
                    // Verifica se é necessário alternar a cor
                    bool corSalvaEhSecundaria = CoresIguais(corSpoiler, manager.corSecundariaPadrao);
                    if (manager.usarCorSecundaria != corSalvaEhSecundaria)
                    {
                        manager.AlternarCor();
                    }
                }
            }
        }

        // Aplicar preço do histórico
        carSelector.AplicarPrecoDoHistorico(data);
    }

    // Função auxiliar para comparar cores com tolerância
    bool CoresIguais(Color a, Color b, float tolerancia = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerancia &&
               Mathf.Abs(a.g - b.g) < tolerancia &&
               Mathf.Abs(a.b - b.b) < tolerancia;
    }

    // Geração automática da descrição
    string GerarDescricao(CustomizacaoData item)
    {
        if (item.idJante >= 0)
            return "Jante trocada";
        if (item.idSpoiler >= 0)
            return "Spoiler trocado";
        if (!string.IsNullOrEmpty(item.corHex))
            return "Carro pintado";
        if (!string.IsNullOrEmpty(item.corJanteHex))
            return "Cor da jante alterada";
        if (!string.IsNullOrEmpty(item.corSpoilerHex))
            return "Cor do spoiler alterada";

        return "Customização aplicada";
    }

    // Opcional: para adicionar manualmente ao histórico via script
    public void AdicionarAoHistorico(string texto)
    {
        CustomizacaoData novaEntrada = new CustomizacaoData(texto);
        GuardarHistorico(novaEntrada);
        AtualizarListaHistorico(); // Se quiser atualizar em tempo real
    }
}





