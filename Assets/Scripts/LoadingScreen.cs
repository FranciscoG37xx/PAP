using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public float minLoadingTime = 10f; // segundos mínimos que a loading vai ficar ativa

    void Start()
    {
        StartCoroutine(LoadMainSceneAsync());
    }

    IEnumerator LoadMainSceneAsync()
    {
        float startTime = Time.time;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // Se o carregamento estiver quase completo (90%) e já passaram os segundos mínimos,
            // então ativa a cena principal
            if (asyncLoad.progress >= 0.9f && Time.time - startTime >= minLoadingTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}

