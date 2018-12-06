using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCannonController : MonoBehaviour,IObserver {
    public GameObject bolaPrefab;

    public float shootInpulse;
    public float timeShootInterval;
    public float timeDelayToStartShoot = 1f;
    private float timeDelay;
    private float time;

    private GameObject alvo;
    private GameObject bola;
    private bool gameOver;
    private Vector3 posicaoInicialBola;

    public void notify()
    {
        gameOver = DadosJogo.getInstance().GameOver;
    }

    // Use this for initialization
    void Start () {
        DadosJogo.getInstance().getGame().addObserver(this);

        posicaoInicialBola = new Vector3(0, 0, 1);
        timeDelay = 0;
        time = 0;
        gameOver = false;
    }

	// Update is called once per frame
	void Update () {
        if (!gameOver)
        {
            if (timeDelay < timeDelayToStartShoot)
            {
                timeDelay += Time.deltaTime;
            }
            else
            {
                if (time > timeShootInterval)
                {
                    bola = Object.Instantiate(bolaPrefab);
                    bola.transform.position = transform.TransformPoint(posicaoInicialBola);
                    bola.GetComponent<Rigidbody>().AddForce(transform.forward * shootInpulse);
                    time = 0;
                }
                time += Time.deltaTime;
            }
        }
	} 
}
