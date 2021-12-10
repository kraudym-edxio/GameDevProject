using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("coinCount", 0);
        PlayerPrefs.SetInt("kills", 0);

        playButton.onClick.AddListener(LoadFirstScene);
        quitButton.onClick.AddListener(Application.Quit);
    }

    void LoadFirstScene() {
        SceneManager.LoadScene("Level_1");
    }
}
