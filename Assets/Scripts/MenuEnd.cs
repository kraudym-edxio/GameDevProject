using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuEnd : MonoBehaviour
{
    public Button mainMenuButton;

    public TextMeshProUGUI coinCountText;
    public TextMeshProUGUI killText;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(BackToMainMenu);
        coinCountText.text = $"{PlayerPrefs.GetInt("coinCount", 0)}";
        killText.text = $"{PlayerPrefs.GetInt("kills", 0)}";
    }

    void BackToMainMenu() {
        SceneManager.LoadScene("StartMenu");
    }
}
