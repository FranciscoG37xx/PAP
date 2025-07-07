using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HistoricoManager : MonoBehaviour
{
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
        if (historicoPanel != null)
            historicoPanel.SetActive(false);
    }

    // Método chamado ao clicar em "Histórico"
    public void AbrirHistorico()
    {
        if (historicoPanel != null)
        {
            historicoPanel.SetActive(true);
            AtualizarListaHistorico();
        }
    }

    // Fecha o painel de histórico
    public void FecharHistorico()
    {
        if (historicoPanel != null)
            historicoPanel.SetActive(false);
    }

    // Guarda os dados atuais no histórico
    public void GuardarHistorico(CustomizacaoData data)
    {
        historicoGuardado.Add(data);
        Debug.Log("Histórico guardado: " + data.nomeExibicao);
    }

    // Atualiza a UI do histórico
    void AtualizarListaHistorico()
    {
        // Limpar os itens antigos
        foreach (Transform filho in content)
            Destroy(filho.gameObject);

        // Criar novos itens com base na lista
        foreach (CustomizacaoData item in historicoGuardado)
        {
            GameObject novoItem = Instantiate(historicoItemPrefab, content);

            // Definir texto
            Text texto = novoItem.GetComponentInChildren<Text>();
            if (texto != null)
                texto.text = item.nomeExibicao;

            // Definir ação do botão
            Button botao = novoItem.GetComponentInChildren<Button>();
            if (botao != null)
            {
                CustomizacaoData copia = item; // evitar referência errada no delegate
                botao.onClick.AddListener(() => RestaurarCustomizacao(copia));
            }
        }
    }

    // Restaura uma customização guardada no histórico
    void RestaurarCustomizacao(CustomizacaoData data)
    {
        Debug.Log("Restaurar histórico: " + data.nomeExibicao);

        // Chamar métodos responsáveis por aplicar os dados guardados
        // Estes métodos devem ser criados nos respetivos scripts:

        // Aplica a jante guardada (ex: WheelSwitcher)
        // FindObjectOfType<WheelSwitcher>()?.AplicarJante(data.idJante);

        // Aplica a cor principal (ex: CorPrimariaManager)
        // FindObjectOfType<CorPrimariaManager>()?.AplicarCor(data.corHex);

        // Aplica o spoiler (ex: SpoilerSwitcher)
        // FindObjectOfType<SpoilerSwitcher>()?.AplicarSpoiler(data.idSpoiler);

        // Atualiza o preço com base na customização restaurada (ex: CarSelector ou PrecoManager)
        // FindObjectOfType<CarSelector>()?.AplicarPrecoDoHistorico(data);
    }
}


