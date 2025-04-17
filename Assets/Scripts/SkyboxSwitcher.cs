using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    public Material skyboxDia;
    public Material skyboxNoite;

    public Light luzDirecional;
    public Light luzExtraCarro; // luz opcional extra para iluminar o carro à noite

    private bool isDia = true;

    public void AlternarSkybox()
    {
        isDia = !isDia;

        RenderSettings.skybox = isDia ? skyboxDia : skyboxNoite;

        // alternar a cor/intensidade da luz também
        if (luzDirecional != null)
        {
            luzDirecional.intensity = isDia ? 1.0f : 0.3f;
            luzDirecional.color = isDia ? Color.white : new Color(0.5f, 0.5f, 0.7f);
        }

        // alternar a luz ambiente para evitar carro totalmente escuro
        RenderSettings.ambientLight = isDia
            ? Color.white
            : new Color(0.1f, 0.1f, 0.2f); // tom azulado fraco

        // ativar/desativar luz de apoio no carro
        if (luzExtraCarro != null)
        {
            luzExtraCarro.enabled = !isDia;
        }

        DynamicGI.UpdateEnvironment();
    }
}


