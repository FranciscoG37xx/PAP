using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TrocarPainel : MonoBehaviour
{
    public GameObject painelCriarConta;
    public GameObject painelLogin;

    public TMP_InputField emailField1;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField1;

    public TMP_InputField emailField2;
    public TMP_InputField passwordField2;

    public void IrParaLogin()
    {
        painelCriarConta.SetActive(false);
        painelLogin.SetActive(true);
    }

    public void IrParaCriarConta()
    {
        painelCriarConta.SetActive(true);
        painelLogin.SetActive(false);
    }

    public void LimparCampos()
    {
        emailField1.text = string.Empty;
        passwordField1.text = string.Empty;

        emailField2.text = string.Empty;
        usernameField.text = string.Empty;
        passwordField2.text = string.Empty;
    }
}
