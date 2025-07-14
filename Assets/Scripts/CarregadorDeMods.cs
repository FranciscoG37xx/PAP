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

        string connectionString = "Server=localhost;Database=autorevamp_bd;User Id=root;Password=;SslMode=None;";
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM modificacoes WHERE user_id = @user_id LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", UserSession.UserID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string carroID = reader.GetString("carro_id");
                        string corHex = reader.GetString("cor_hex");
                        int janteID = reader.GetInt32("jante_id");
                        bool pintarJante = reader.GetBoolean("pintar_jante");
                        int spoilerID = reader.GetInt32("spoiler_id");
                        float precoTotal = reader.GetFloat("preco_total");

                        // Aplicar dados
                        carSelector.SelecionarCarroPorID(carroID); // Tem de existir esse método


                        corManager.AplicarCorHex(corHex);
                        wheelSwitcher.SwitchWheels(janteID);
                        if (pintarJante)
                        {
                            wheelSwitcher.PintarJante(); // se tiver método separado
                        }
                        spoilerSwitcher.AplicarSpoiler(spoilerID);
                        carSelector.DefinirPreco(precoTotal); // método para definir preço manualmente
                    }
                    else
                    {
                        Debug.Log("Nenhuma modificação encontrada para este utilizador.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Erro ao carregar modificações: " + ex.Message);
            }
            
            yield return new WaitForSeconds(0.2f); // Esperar para o carro carregar corretamente
        }
    }
}

