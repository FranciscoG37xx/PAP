using UnityEngine;
using System.Collections;
using System.IO;
using SFB;

public class ShareManager : MonoBehaviour
{
    public GameObject panelMods; // esconder antes do screenshot

    public void ShareScreenshot()
    {
        StartCoroutine(TakeScreenshotAndSave());
    }

    IEnumerator TakeScreenshotAndSave()
    {
        panelMods.SetActive(false); // esconder UI
        yield return new WaitForEndOfFrame();

        // Tira o screenshot
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        panelMods.SetActive(true); // voltar a mostrar UI

        // Abre janela para escolher onde guardar
        var extensions = new[] {
            new ExtensionFilter("Imagem PNG", "png")
        };

        string path = StandaloneFileBrowser.SaveFilePanel("Guardar Screenshot", "", "CarroCustomizado", extensions);

        if (!string.IsNullOrEmpty(path))
        {
            // Garante extensão .png
            if (!path.EndsWith(".png")) path += ".png";

            byte[] pngData = screenshot.EncodeToPNG();
            File.WriteAllBytes(path, pngData);

            Debug.Log("Screenshot guardado em: " + path);
        }
        else
        {
            Debug.Log("Utilizador cancelou.");
        }

        // Libertar memória
        Destroy(screenshot);
    }
}



