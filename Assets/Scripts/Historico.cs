using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HistoricoManager : MonoBehaviour
{
    public GameObject panelHistorico;
    public Transform contentParent; // O Content do Scroll View
    public GameObject historicoItemPrefab;

    private List<string> historico = new List<string>();

    public void AdicionarAoHistorico(string descricao)
    {
        historico.Add(descricao);
    }

    public void MostrarHistorico()
    {
        panelHistorico.SetActive(true);

        // Limpar anteriores
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Preencher UI com histórico
        foreach (string entrada in historico)
        {
            GameObject item = Instantiate(historicoItemPrefab, contentParent);
            item.GetComponentInChildren<TMP_Text>().text = entrada;
        }
    }

    public void FecharHistorico()
    {
        panelHistorico.SetActive(false);
    }
}

