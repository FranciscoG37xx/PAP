using UnityEngine;

[System.Serializable]
public class CustomizacaoData
{
    public string nomeExibicao;
    public int idJante;
    public string corHex;
    public int idSpoiler;
    public string corJanteHex;
    public string corSpoilerHex;

    // Construtor completo (snapshot)
    public CustomizacaoData(string nome, int jante, string cor, int spoiler, string corJante, string corSpoiler)
    {
        nomeExibicao = nome;
        idJante = jante;
        corHex = cor;
        idSpoiler = spoiler;
        corJanteHex = corJante;
        corSpoilerHex = corSpoiler;
    }

    // Construtor simplificado (apenas nome)
    public CustomizacaoData(string nome)
    {
        nomeExibicao = nome;
        idJante = -1;
        corHex = null;
        idSpoiler = -1;
        corJanteHex = null;
        corSpoilerHex = null;
    }
}

