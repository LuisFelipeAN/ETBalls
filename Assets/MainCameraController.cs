using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCameraController : MonoBehaviour,IObserver {
    public float eyeSpeed=1f;
    public float camSpeed=7f;
    public float shotImpulse=1000f;
    public float distanciaMaxParede = 0.5f;
    public Rigidbody bullet;
    public RectTransform mira;

    public int numMaxBolas=150;
    public  Text textBolas;
    public  Text textPontos;
    public RectTransform RectTransformVida;
    public int MaxVida = 100;
    public Camera cam;

    private float mouseH = 0;
    private float mouseV = 0;
    private Quaternion initialOrientation;
  
    private bool paredeDireita, paredeEsquerda;
    private Vector3 deslocamentoLateral;
    private int bolas;
    private float dv;

    private bool gameOver;
    // Use this for initialization

    void Start () {
        gameOver = false;
        dv = RectTransformVida.sizeDelta.x / MaxVida;

        DadosJogo.getInstance().addObserver(this);
        DadosJogo.getInstance().Vida = MaxVida;

       

        initialOrientation = transform.localRotation;
        deslocamentoLateral = new Vector3();
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (!gameOver)
        {
            atualizaRayCasts();

            mouseH += Input.GetAxis("Mouse X");
            mouseV += Input.GetAxis("Mouse Y");

            Quaternion rotH = Quaternion.AngleAxis(mouseH * eyeSpeed, Vector3.up);
            Quaternion rotV = Quaternion.AngleAxis(mouseV * eyeSpeed, Vector3.left);
            transform.localRotation = initialOrientation * rotH * rotV;

            if (Input.GetMouseButtonDown(0))
            {
                if (DadosJogo.getInstance().Bolas >= numMaxBolas)
                {
                    DadosJogo.getInstance().GameOver = true;
                }
                else
                {
                    Rigidbody b = Object.Instantiate(bullet);
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    b.gameObject.transform.position = transform.position;
                    //Debug.Log(ray.direction + ", " + shotImpulse);
                    b.AddForce(ray.direction * shotImpulse);
                    DadosJogo.getInstance().Bolas = DadosJogo.getInstance().Bolas + 1;
                }
               
            }
            else if (Input.GetMouseButtonDown(1))
            {
                transform.localRotation = initialOrientation;
            }
            if (Input.GetKey(KeyCode.LeftArrow) && !paredeEsquerda)
            {

                deslocamentoLateral.x = transform.position.x - camSpeed * Time.deltaTime;
                deslocamentoLateral.y = transform.position.y;
                deslocamentoLateral.z = transform.position.z;
                transform.position = deslocamentoLateral;
            }
            if (Input.GetKey(KeyCode.RightArrow) && !paredeDireita)
            {
                deslocamentoLateral.x = transform.position.x + camSpeed * Time.deltaTime;
                deslocamentoLateral.y = transform.position.y;
                deslocamentoLateral.z = transform.position.z;
                transform.position = deslocamentoLateral;
            }
            mira.transform.position = Input.mousePosition;
        }
      
        
    }
    private void atualizaRayCasts() {
        paredeDireita=Physics.Raycast(transform.position, Vector3.right, distanciaMaxParede);
        paredeEsquerda = Physics.Raycast(transform.position, Vector3.left, distanciaMaxParede);
    }

    public void notify()
    {
        gameOver = DadosJogo.getInstance().GameOver;
        textBolas.text = "Bolas: " + DadosJogo.getInstance().Bolas+ "/" + numMaxBolas;
        textPontos.text = "Pontos: " + DadosJogo.getInstance().Pontos;
        RectTransformVida.sizeDelta = new Vector2(dv * DadosJogo.getInstance().Vida, RectTransformVida.sizeDelta.y);
    }
    
}
