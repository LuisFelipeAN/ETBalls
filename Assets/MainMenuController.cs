using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public Button startButton;

    private void Start()
    {
   
        startButton.onClick.AddListener(loadSeneFase1);
    }

    public void loadSeneFase1()
    {
        SceneManager.LoadScene("Fase1", LoadSceneMode.Single);
    }
}
