using UnityEngine;

[System.Serializable]
public class CustomizacaoData
{
    public string nomeExibicao;
    public int idJante;
    public string corHex;
    public int idSpoiler;

    public CustomizacaoData(string nome, int jante, string cor, int spoiler)
    {
        nomeExibicao = nome;
        idJante = jante;
        corHex = cor;
        idSpoiler = spoiler;
    }
}
