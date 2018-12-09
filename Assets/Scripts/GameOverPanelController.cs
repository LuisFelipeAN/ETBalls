using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanelController : MonoBehaviour {

    private Button mainMenuButton;//botao start no menu principal

    private void Start()
    {
        mainMenuButton= GameObject.Find("MainMenuButton").GetComponent<Button>();
        mainMenuButton.onClick.AddListener(loadMainMenu);//registra o callback para uma funcao que carrega a fase 1
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
