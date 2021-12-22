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
