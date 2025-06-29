using UnityEngine;
using UnityEngine.UI;

public class EffectsVolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource effectsSource;

    void Start()
    {
        if (volumeSlider != null && effectsSource != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 1f);
            effectsSource.volume = volumeSlider.value;

            volumeSlider.onValueChanged.AddListener(delegate {
                effectsSource.volume = volumeSlider.value;
                PlayerPrefs.SetFloat("EffectsVolume", volumeSlider.value);
            });
        }
    }
}

