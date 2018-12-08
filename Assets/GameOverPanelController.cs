using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanelController : MonoBehaviour {

    private Button mainMenuButton;

    private void Start()
    {

        mainMenuButton= GameObject.Find("MainMenuButton").GetComponent<Button>();
        mainMenuButton.onClick.AddListener(loadMainMenu);
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
