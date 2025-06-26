using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Música de Fundo")]
    public AudioSource musicaDeFundo;
    [Range(0f, 1f)] public float volumeMusica = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional, se quiseres que continue entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicaDeFundo != null)
        {
            musicaDeFundo.loop = true;
            musicaDeFundo.volume = volumeMusica;
            musicaDeFundo.Play();
        }
    }

    public void SetVolumeMusica(float novoVolume)
    {
        volumeMusica = Mathf.Clamp01(novoVolume);

        if (musicaDeFundo != null)
        {
            musicaDeFundo.volume = volumeMusica;
        }
    }

    // Caso queiras pausar/retomar depois
    public void PausarMusica()
    {
        if (musicaDeFundo != null)
            musicaDeFundo.Pause();
    }

    public void RetomarMusica()
    {
        if (musicaDeFundo != null && !musicaDeFundo.isPlaying)
            musicaDeFundo.Play();
    }
}


