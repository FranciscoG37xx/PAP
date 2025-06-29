using System;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_Text feedbackMessage1;
    public Button loginButton;

    private string connectionString = "Server=localhost;Database=autorevamp_bd;User Id=root;Password=;SslMode=None;";

    void Start()
    {
        loginButton.onClick.AddListener(AttemptLogin);
    }

    public void AttemptLogin()
    {
        Debug.Log($"Tentando login no GameObject: {this.gameObject.name}");
        if (feedbackMessage1 == null)
        {
            Debug.LogError("feedbackMessage está null! Verifica no Inspector.");
            return;
        }
        string email = emailField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            feedbackMessage1.text = "Preenche todos os campos!";
            return;
        }

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Debug.Log("Conexão ao MySQL estabelecida!");

                string query = @"
                    SELECT UserID, Username, PasswordHash, Salt 
                    FROM users 
                    WHERE Email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            string username = reader.GetString(1);
                            string storedHash = reader.GetString(2);
                            string storedSalt = reader.GetString(3);
                            string enteredHash = HashPassword(password, storedSalt);

                            if (storedHash == enteredHash)
                            {
                                reader.Close(); // fechar reader antes de novo comando

                                // Atualizar LastLogin
                                string updateQuery = "UPDATE users SET LastLogin = NOW() WHERE UserID = @UserID";
                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection))
                                {
                                    updateCmd.Parameters.AddWithValue("@UserID", userId);
                                    updateCmd.ExecuteNonQuery();
                                }

                                // Guardar os dados na sessão
                                UserSession.Username = username;
                                UserSession.UserID = userId;
                                UserSession.Email = email;

                                feedbackMessage1.text = "Login bem-sucedido!";
                                Debug.Log($"Login: {username} ({email})");

                                SceneManager.LoadScene("LoadingScene");
                            }
                            else
                            {
                                feedbackMessage1.text = "Senha incorreta!";
                            }
                        }
                        else
                        {
                            feedbackMessage1.text = "Email não encontrado!";
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                feedbackMessage1.text = "Erro ao conectar ao MySQL!";
                Debug.LogError("Erro MySQL: " + ex.Message);
            }
        }
    }

    private string HashPassword(string password, string salt)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string saltedPassword = password + salt;
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public void LimparCampos()
    {
        emailField.text = string.Empty;
        passwordField.text = string.Empty;
    }
}






