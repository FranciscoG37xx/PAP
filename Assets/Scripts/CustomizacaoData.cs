using UnityEngine;

[System.Serializable]
public class CustomizacaoData
{
    public string nomeExibicao;
    public int idJante;
    public string corHex;
    public int idSpoiler;

    public string corJanteHex; // Novo campo para guardar a cor da jante
    public string corSpoilerHex;


    public CustomizacaoData(string nome, int jante, string cor, int spoiler, string corJante, string corSpoiler)
    {
        nomeExibicao = nome;
        idJante = jante;
        corHex = cor;
        idSpoiler = spoiler;
        corJanteHex = corJante;
        corSpoilerHex = corSpoiler;
    }
}
