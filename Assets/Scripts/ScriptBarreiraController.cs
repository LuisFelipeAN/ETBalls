using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBarreiraController : MonoBehaviour,IObserver {
    public float velocidade;
    public float distanciaMaxParede = 0.5f;
    private float velocidadeAtual;
    private MeshCollider meshCollider;

    private bool paredeDireita, paredeEsquerda;
    private Vector3 centerLeft, centerRight;
    private float size;
    private bool gameOver;

    // Use this for initialization
    void Start () {
        DadosJogo.getInstance().getGame().addObserver(this);
        meshCollider = GetComponent<MeshCollider>();
        size = Mathf.Max(GetComponent<MeshRenderer>().bounds.size.x, GetComponent<MeshRenderer>().bounds.size.y, GetComponent<MeshRenderer>().bounds.size.z);
        centerLeft = new Vector3();
        centerRight = new Vector3();
        velocidadeAtual = velocidade;
        gameOver = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameOver)
        {
            atualizaRayCasts();
            if (paredeDireita || paredeEsquerda)
            {
                velocidadeAtual = -velocidadeAtual;
                transform.Translate(velocidadeAtual * Time.deltaTime, 0, 0, Space.World);
            }
            else
            {
                transform.Translate(velocidadeAtual * Time.deltaTime, 0, 0, Space.World);
            }
        }
      
	}
    private void atualizaRayCasts()
    {
        if (meshCollider.enabled)
        {
            meshCollider.enabled = false;

            centerRight = transform.position;
            centerRight.x -= size;

            centerLeft = transform.position;
            centerLeft.x += size;

            paredeDireita = Physics.Raycast(centerLeft, Vector3.left, distanciaMaxParede);
            paredeEsquerda = Physics.Raycast(centerRight, Vector3.right, distanciaMaxParede);

            meshCollider.enabled = true;
        }
        else
        {
            centerRight = transform.position;
            centerRight.x -= size;

            centerLeft = transform.position;
            centerLeft.x += size;

            paredeDireita = Physics.Raycast(centerLeft, Vector3.left, distanciaMaxParede);
            paredeEsquerda = Physics.Raycast(centerRight, Vector3.right, distanciaMaxParede);
        }
      
    }

    public void notify()
    {
        gameOver = DadosJogo.getInstance().GameOver;
    }
}
