using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Mixer Field")]
    [Tooltip("Drag in the audio mixer to access the values.")]
    [SerializeField] AudioMixer mixer;

    [Tooltip("The type of volume that this slider changes.")]
    [SerializeField] public volume_types volume_choice;

    [Header("Slider Fields")]
    [Tooltip("Drag in the slider that will modify the volume.")]
    [SerializeField] Slider volume_slider;

    public enum volume_types {
        master,
        music,
        fx
    }

    string volume_type;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        switch (volume_choice) {
            case volume_types.master:
                volume_type = "master_volume";
                break;
            case volume_types.music:
                volume_type = "music_volume";
                break;
            case volume_types.fx:
                volume_type = "fx_volume";
                break;
        }

        if (!PlayerPrefs.HasKey(volume_type)) {
            PlayerPrefs.SetFloat(volume_type, 1);
        } else {
            load();
        }
    }

    public void change_volume() {
        // The slider scales from 0 to 1 linearly.
        // The audio mixer control scales from -80 to 0 logarithimcally.
        float volume_level = 0f;
        if (volume_slider.value == 0) {
            volume_level = -80f;
        } else {
            volume_level = Mathf.Log(volume_slider.value, 10) * 80f;
        }
        mixer.SetFloat(volume_type, volume_level);
        save();
    }

    private void load() {
        // Remember to make this load function an inverse of the change volume function!
        mixer.GetFloat(volume_type, out float volume);
        volume_slider.value = Mathf.Pow(10, (volume / 80f));
    }

    private void save() {
        PlayerPrefs.SetFloat(volume_type, volume_slider.value);
    }
}
