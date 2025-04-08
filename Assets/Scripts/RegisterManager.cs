using System;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;
using TMPro;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text feedbackMessage;
    public Button createAccountButton;

    private string connectionString = "Server=localhost;Database=autorevamp_bd;User Id=root;Password=;SslMode=None;";

    void Start()
    {
        createAccountButton.onClick.AddListener(CreateAccount);
    }

    public void CreateAccount()
    {
        string email = emailField.text.Trim();
        string username = usernameField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackMessage.text = "Preenche todos os campos!";
            return;
        }

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Debug.Log("Conexão ao MySQL estabelecida!");

                string checkQuery = "SELECT COUNT(*) FROM users WHERE Email = @Email OR Username = @Username";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    int userExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (userExists > 0)
                    {
                        feedbackMessage.text = "Email ou Username já existem!";
                        return;
                    }
                }

                string salt = GenerateSalt();
                string passwordHash = HashPassword(password, salt);

                string insertQuery = "INSERT INTO users (Email, Username, PasswordHash, Salt) VALUES (@Email, @Username, @PasswordHash, @Salt)";
                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
                {
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.Parameters.AddWithValue("@Username", username);
                    insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    insertCmd.Parameters.AddWithValue("@Salt", salt);
                    insertCmd.ExecuteNonQuery();

                    feedbackMessage.text = "Conta criada com sucesso!";
                    Debug.Log("Nova conta criada: " + username);
                }
            }
            catch (Exception ex)
            {
                feedbackMessage.text = "Erro ao criar conta!";
                Debug.LogError("Erro MySQL: " + ex.Message);
            }
        }
    }

    private string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
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
    usernameField.text = string.Empty;
    passwordField.text = string.Empty;
}
}




