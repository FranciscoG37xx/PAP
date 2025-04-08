using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;

public class PasswordResetManager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField codeField;
    public TMP_InputField newPasswordField;
    public TMP_Text feedbackMessage;
    public GameObject emailPanel;
    public GameObject resetPasswordPanel;
    
    private string smtpServer = "smtp.gmail.com";
    private int smtpPort = 587;
    private string senderEmail = "autorevamp123@gmail.com";
    private string senderPassword = "ngpk moho qzfe hdzv";

    private string connectionString = "Server=localhost;Database=autorevamp_bd;User Id=root;Password=;SslMode=None;";
    private string generatedCode;

    public void OpenEmailPanel()
    {
        emailPanel.SetActive(true);
        resetPasswordPanel.SetActive(false);
    }

    public void SendVerificationCode()
    {
        string email = emailField.text.Trim();
        if (string.IsNullOrEmpty(email))
        {
            feedbackMessage.text = "Por favor, insira um email válido.";
            return;
        }

        generatedCode = GenerateVerificationCode();
        
        if (StoreResetCode(email, generatedCode))
        {
            try
            {
                MailMessage mail = new MailMessage(senderEmail, email, "Código de Recuperação", $"O teu código de recuperação é: {generatedCode}");
                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };
                smtp.Send(mail);
                feedbackMessage.text = "Código enviado! Verifica a tua caixa de entrada.";
            }
            catch (Exception ex)
            {
                feedbackMessage.text = "Erro ao enviar email.";
                Debug.LogError("Erro SMTP: " + ex.Message);
            }
        }
        else
        {
            feedbackMessage.text = "Email não encontrado.";
        }
    }

    public void VerifyCode()
    {
        string email = emailField.text.Trim();
        string enteredCode = codeField.text.Trim();
        
        if (string.IsNullOrEmpty(enteredCode))
        {
            feedbackMessage.text = "Insira o código recebido.";
            return;
        }

        if (ValidateResetCode(email, enteredCode))
        {
            feedbackMessage.text = "Código validado! Agora redefine a senha.";
            emailPanel.SetActive(false);
            resetPasswordPanel.SetActive(true);
        }
        else
        {
            feedbackMessage.text = "Código incorreto.";
        }
    }

    public void ResetPassword()
    {
        string email = emailField.text.Trim();
        string newPassword = newPasswordField.text.Trim();

        if (string.IsNullOrEmpty(newPassword))
        {
            feedbackMessage.text = "Insira a nova senha.";
            return;
        }

        string salt = GenerateSalt();
        string hashedPassword = HashPassword(newPassword, salt);

        if (UpdatePassword(email, hashedPassword, salt))
        {
            feedbackMessage.text = "Senha redefinida com sucesso!";
            emailPanel.SetActive(false);
            resetPasswordPanel.SetActive(false);
        }
        else
        {
            feedbackMessage.text = "Erro ao redefinir senha.";
        }
    }

    private bool StoreResetCode(string email, string code)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "UPDATE users SET reset_code = @Code WHERE Email = @Email";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    cmd.Parameters.AddWithValue("@Email", email);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Erro MySQL: " + ex.Message);
                return false;
            }
        }
    }

    private bool ValidateResetCode(string email, string code)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "SELECT reset_code FROM users WHERE Email = @Email";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.Read() && reader["reset_code"].ToString() == code;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Erro MySQL: " + ex.Message);
                return false;
            }
        }
    }

    private bool UpdatePassword(string email, string hashedPassword, string salt)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "UPDATE users SET PasswordHash = @Password, Salt = @Salt, reset_code = NULL WHERE Email = @Email";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@Salt", salt);
                    cmd.Parameters.AddWithValue("@Email", email);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Erro MySQL: " + ex.Message);
                return false;
            }
        }
    }

    private string GenerateVerificationCode()
    {
        System.Random random = new System.Random();
        return random.Next(100000, 999999).ToString();
    }

    private string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        new RNGCryptoServiceProvider().GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private string HashPassword(string password, string salt)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] saltedPassword = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = sha256.ComputeHash(saltedPassword);
            return Convert.ToBase64String(hashBytes);
        }
    }
}


