using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{

    // sound manager
    private AudioSource menuMusic;

    // options GUI items
    public GameObject optionsGUI;
    // Start is called before the first frame update
    void Start()
    {
        menuMusic = gameObject.GetComponent<AudioSource>();
        var volumeSlider = optionsGUI.transform.Find("VolumeControl").Find("VolumeSlider").GetComponent<Slider>();

        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 1);
        menuMusic.volume = volumeSlider.value;

        volumeSlider.onValueChanged.AddListener((float val) => {
            PlayerPrefs.SetFloat("masterVolume", val);
            menuMusic.volume = val;
        });

    }
}
