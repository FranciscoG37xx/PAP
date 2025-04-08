using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserInfoDisplay : MonoBehaviour
{
    public TMP_Text usernameDisplay;

    void Start()
    {
        if (usernameDisplay != null && !string.IsNullOrEmpty(UserSession.Username))
        {
            usernameDisplay.text = UserSession.Username;
        }
        else
        {
            usernameDisplay.text = "Utilizador";
        }
    }
}

