using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    public void Logout()
    {
        // Limpar sessão do utilizador
        UserSession.Username = null;
        UserSession.UserID = 0;
        UserSession.Email = null;

        // Voltar para a cena de login 
        SceneManager.LoadScene("LoginScene");
    }
}

