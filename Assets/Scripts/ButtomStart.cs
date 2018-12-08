using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtomStart : MonoBehaviour {

    public void startFase1()
    {
        SceneManager.LoadScene("Fase1", LoadSceneMode.Additive);
    }
}
