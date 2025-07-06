using UnityEngine;

public class LogoutAndExit : MonoBehaviour
{
    public void LogoutAndQuit()
    {
        // Apagar dados de sessão guardados localmente
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("isLoggedIn");
        PlayerPrefs.Save();

        // Fechar a aplicação
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Para quando estás a testar no editor
#else
        Application.Quit(); // Para builds (.exe)
#endif
    }
}
