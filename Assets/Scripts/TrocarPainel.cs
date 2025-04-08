using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrocarPainel : MonoBehaviour
{
    public GameObject painelCriarConta;
    public GameObject painelLogin;

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
}
