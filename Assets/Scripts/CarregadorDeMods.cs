using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;

public class CarregadorDeModificacoes : MonoBehaviour
{
    public CarSelector carSelector;
    public CorPrimariaManager corManager;
    public WheelSwitcher wheelSwitcher;
    public SpoilerSwitcher spoilerSwitcher;

    void Start()
    {
        StartCoroutine(CarregarModificacoes());
    }

    IEnumerator CarregarModificacoes()
    {
        if (UserSession.UserID <= 0)
        {
            Debug.LogWarning("Utilizador não autenticado.");
            yield break;
        }

        // Dados a carregar
        string carroID = null;
        string corHex = null;
        int janteID = 0;
        bool pintarJante = false;
        int spoilerID = 0;
        float precoTotal = 0f;
        bool encontrou = false;

        // Parte 1: carregar da base de dados
        string connectionString = "Server=localhost;Database=autorevamp_bd;User Id=root;Password=;SslMode=None;";
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM modificacoes WHERE user_id = @user_id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", UserSession.UserID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        carroID = reader.GetString("carro_id");
                        corHex = reader.GetString("cor_hex");
                        janteID = reader.GetInt32("jante_id");
                        pintarJante = reader.GetBoolean("pintar_jante");
                        spoilerID = reader.GetInt32("spoiler_id");
                        precoTotal = reader.GetFloat("preco_total");
                        encontrou = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Erro ao carregar modificações: " + ex.Message);
            }
        }

        // Parte 2: aplicar se encontrou modificações
        if (encontrou)
        {
            carSelector.SelecionarCarroPorID(carroID);
            yield return new WaitUntil(() => carSelector.CarregamentoConcluido());

            corManager.AplicarCorHex(corHex);
            wheelSwitcher.SwitchWheels(janteID);
            yield return null;

            if (pintarJante && wheelSwitcher.JanteEstaPronta())
            {
                wheelSwitcher.PintarJante();
            }

            spoilerSwitcher.AplicarSpoiler(spoilerID);
            carSelector.DefinirPreco(precoTotal);
        }
        else
        {
            Debug.Log("Nenhuma modificação encontrada para este utilizador.");
        }
    }
}


