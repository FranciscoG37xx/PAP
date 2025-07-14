using System;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;

public class GuardarModificacoesButton : MonoBehaviour
{
    public Button botaoGuardar;

    // Referências aos scripts que guardam os dados do carro atual
    public CarSelector carSelector;
    public CorPrimariaManager corManager;
    public WheelSwitcher wheelSwitcher;
    public SpoilerSwitcher spoilerSwitcher;

    void Start()
    {
        if (botaoGuardar == null)
            botaoGuardar = GetComponent<Button>();

        botaoGuardar.onClick.AddListener(GuardarModificacoes);
    }

    void GuardarModificacoes()
    {
        if (UserSession.UserID <= 0)
        {
            Debug.LogWarning("Utilizador não autenticado. Faça login para guardar modificações.");
            return;
        }

        // Obter os dados atuais do carro
        string carroID = carSelector.ObterCarroIDAtual();
        string corHex = corManager.CorAtualHex; 
        int janteID = wheelSwitcher.JanteAtualID;
        bool pintarJante = wheelSwitcher.JanteEstaPintada;
        int spoilerID = spoilerSwitcher.SpoilerAtualID;
        float precoTotal = carSelector.GetPrecoAtual();

        // Guardar na base de dados
        string connectionString = "Server=localhost;Database=autorevamp_bd;User Id=root;Password=;SslMode=None;";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                string query = @"
                    INSERT INTO modificacoes (user_id, carro_id, cor_hex, jante_id, pintar_jante, spoiler_id, preco_total)
                    VALUES (@user_id, @carro_id, @cor_hex, @jante_id, @pintar_jante, @spoiler_id, @preco_total)
                    ON DUPLICATE KEY UPDATE
                        carro_id = VALUES(carro_id),
                        cor_hex = VALUES(cor_hex),
                        jante_id = VALUES(jante_id),
                        pintar_jante = VALUES(pintar_jante),
                        spoiler_id = VALUES(spoiler_id),
                        preco_total = VALUES(preco_total);
                ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", UserSession.UserID);
                cmd.Parameters.AddWithValue("@carro_id", carroID);
                cmd.Parameters.AddWithValue("@cor_hex", corHex);
                cmd.Parameters.AddWithValue("@jante_id", janteID);
                cmd.Parameters.AddWithValue("@pintar_jante", pintarJante);
                cmd.Parameters.AddWithValue("@spoiler_id", spoilerID);
                cmd.Parameters.AddWithValue("@preco_total", precoTotal);

                int result = cmd.ExecuteNonQuery();
                Debug.Log("Modificações guardadas com sucesso. Registos afetados: " + result);
            }
            catch (Exception ex)
            {
                Debug.LogError("Erro ao guardar modificações: " + ex.Message);
            }
        }
    }
}

