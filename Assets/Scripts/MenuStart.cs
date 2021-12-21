using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;

    // sound manager
    private AudioSource menuMusic;

    // options GUI items
    public Slider volumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        menuMusic = gameObject.GetComponent<AudioSource>();

        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 1);
        menuMusic.volume = volumeSlider.value;

        volumeSlider.onValueChanged.AddListener((float val) => {
            PlayerPrefs.SetFloat("masterVolume", val);
            menuMusic.volume = val;
        });

        playButton.onClick.AddListener(LoadFirstScene);
        quitButton.onClick.AddListener(QuitGame);
    }

    void LoadFirstScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    void QuitGame() {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
