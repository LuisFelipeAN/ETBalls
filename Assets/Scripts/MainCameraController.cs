using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainCameraController : MonoBehaviour,IObserver {
    public float eyeSpeed=1f;
    public float camSpeed=7f;
    public float shotImpulse=1000f;
    public float distanciaMaxParede = 0.5f;


    public int level = 1;
    public bool isLastLevel;
    public int pontosProximoNivel;

    public int numMaxBolas = 150;
    public int MaxVida = 100;
    public float maxTime = 120;

    public float timeMaxInformationCheckpointLoaded = 3f;
    public List<int> chekpoints;
    public int maxCheckpointRestarts = 3;

    public Rigidbody bullet;
    public Button bntRestartOnCheckpointPrefab;
    public Button bntMainMenuPrefab;

    private float timeCheckpointLoaded;
    private bool onChekpointLoad;
    private Text timeText;
    private Canvas canvas;
    private Button bntRestartOnCheckpoint;
    private Button bntMainMenu;
    private Text textBolas;
    private Text textPontos;
    private RectTransform mira;
    private RectTransform RectTransformVida;
    private ScriptSalaController scriptSalaController;
    private Camera cam;
    private GameObject sala;
    private Text textInformationCheckpoint;

    private int contCheckpointsRestarts;
    private float actualTime;
    
    private float mouseH = 0;
    private float mouseV = 0;
    private Quaternion initialOrientation;
    private bool paredeDireita, paredeEsquerda;
    private Vector3 deslocamentoLateral;
    private int bolas;
    private float dv;
    private int actualCheckpoint;
    private bool gameOver;

    private List<Chekpoint> chekpointDadosJogo;
    // Use this for initialization
    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        cam = GetComponent<Camera>();
        textPontos = GameObject.Find("Pontos").GetComponent<Text>();
        textBolas = GameObject.Find("Bolas").GetComponent<Text>();
        RectTransformVida = GameObject.Find("Vida").GetComponent<RectTransform>();
        mira = GameObject.Find("Mira").GetComponent<RectTransform>();
        sala = GameObject.Find("PrefabSala");
        scriptSalaController = sala.GetComponent<ScriptSalaController>();
        textInformationCheckpoint = GameObject.Find("InformCheckpoint").GetComponent<Text>();
        chekpointDadosJogo = new List<Chekpoint>();
        timeText = GameObject.Find("TimeText").GetComponent<Text>();
        DadosJogo.getInstance().clearObservers();

    }
    struct Chekpoint
    {
        public int Vida;
        public int Bolas;
        public int Pontos;
        public float Time;
    }

    void Start () {
        gameOver = false;
        contCheckpointsRestarts = 0;
        actualCheckpoint = 0;
        dv = RectTransformVida.sizeDelta.x / MaxVida;
        DadosJogo.getInstance().getGame().Win = false;
        DadosJogo.getInstance().GameOver = false;
        DadosJogo.getInstance().Bolas = 0;
        DadosJogo.getInstance().Pontos = 0;
        DadosJogo.getInstance().addObserver(this);
        DadosJogo.getInstance().Vida = MaxVida;
        textInformationCheckpoint.enabled = false;


        bntRestartOnCheckpoint = Object.Instantiate(bntRestartOnCheckpointPrefab);
       
        bntRestartOnCheckpoint.transform.parent = canvas.transform;
        bntRestartOnCheckpoint.GetComponent<RectTransform>().localPosition= new Vector3(0,0,0);
     
        bntMainMenu =Object.Instantiate(bntMainMenuPrefab);
        bntMainMenu.transform.parent = canvas.transform;
        bntMainMenu.GetComponent<RectTransform>().localPosition = new Vector3(0,-77,0);

        disableButtons();

        bntMainMenu.onClick.AddListener(goToMainManu);
        bntRestartOnCheckpoint.onClick.AddListener(loadLastCheckpoint);


        
        actualTime = maxTime;

        timeCheckpointLoaded = 0;
        textInformationCheckpoint.text = "Level " + level;
        textInformationCheckpoint.enabled = true;
        onChekpointLoad = true;

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

            actualTime -= Time.deltaTime;
            if (actualTime < maxTime / 2&& actualTime > maxTime / 10)
            {
                timeText.color = Color.yellow;
            }else if(actualTime<maxTime/10&&actualTime>0){
                timeText.color = Color.red;
            }
            else if(actualTime < 0)
            {
                DadosJogo.getInstance().GameOver = true;
            }
            timeText.text = Mathf.RoundToInt(actualTime)+"s";

            if (onChekpointLoad)
            {
                if (timeCheckpointLoaded > timeMaxInformationCheckpointLoaded)
                {
                    textInformationCheckpoint.enabled = false;
                    onChekpointLoad = false;
                }
                timeCheckpointLoaded += Time.deltaTime;
            }
        }
        mira.transform.position = Input.mousePosition;

    }
    private void atualizaRayCasts() {
        paredeDireita=Physics.Raycast(transform.position, Vector3.right, distanciaMaxParede);
        paredeEsquerda = Physics.Raycast(transform.position, Vector3.left, distanciaMaxParede);
    }
   
    public void notify()
    {
        
        if (DadosJogo.getInstance().Pontos >= pontosProximoNivel)
        {
            DadosJogo.getInstance().getGame().Win = true;
            Cursor.visible = true;
            if (isLastLevel)
            {
                SceneManager.LoadScene("Win", LoadSceneMode.Single);
            }
            else
            {
                int novoLevel = level + 1;
                SceneManager.LoadScene("Fase" + novoLevel, LoadSceneMode.Single);
            }
        }

        gameOver = DadosJogo.getInstance().GameOver;
        if (gameOver && chekpointDadosJogo.Count > 0&& contCheckpointsRestarts<maxCheckpointRestarts)
        {
            textInformationCheckpoint.enabled = false;
            enableButtons();

        }
        else if(gameOver)
        {
            Cursor.visible = true;
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
       
        textBolas.text = "Bolas: " + DadosJogo.getInstance().Bolas+ "/" + numMaxBolas;
        textPontos.text = "Pontos: " + DadosJogo.getInstance().Pontos;
        RectTransformVida.sizeDelta = new Vector2(dv * DadosJogo.getInstance().Vida, RectTransformVida.sizeDelta.y);

        if(actualCheckpoint< chekpoints.Count&&DadosJogo.getInstance().Pontos >= chekpoints[actualCheckpoint])
        {
            Debug.Log("Criando Checkpoint");

            Chekpoint c;
            c.Bolas = DadosJogo.getInstance().Bolas;
            c.Pontos= DadosJogo.getInstance().Pontos;
            c.Time = actualTime;
            c.Vida = DadosJogo.getInstance().Vida;
            chekpointDadosJogo.Add(c);
            actualCheckpoint++;

            timeCheckpointLoaded = 0;
            textInformationCheckpoint.text = "Reached Checkpoint " + actualCheckpoint;
            textInformationCheckpoint.enabled = true;
            onChekpointLoad = true;

            scriptSalaController.startHordaCheckpoint();
        }
    }
    private void disableButtons()
    {

        bntRestartOnCheckpoint.enabled = false;
        bntRestartOnCheckpoint.image.enabled = false;
        bntRestartOnCheckpoint.GetComponentInChildren<Text>().enabled = false;

        bntMainMenu.enabled = false;
        bntMainMenu.image.enabled = false;
        bntMainMenu.GetComponentInChildren<Text>().enabled = false;

    }

    private void enableButtons()
    {

        bntRestartOnCheckpoint.enabled = true;
        bntRestartOnCheckpoint.image.enabled = true;
        bntRestartOnCheckpoint.GetComponentInChildren<Text>().enabled = true;

        bntMainMenu.enabled = true;
        bntMainMenu.image.enabled = true;
        bntMainMenu.GetComponentInChildren<Text>().enabled = true;
    }
    public void loadLastCheckpoint()
    {

        DadosJogo.getInstance().GameOver = false;
        Chekpoint atual = chekpointDadosJogo[actualCheckpoint - 1];
        DadosJogo.getInstance().Bolas = atual.Bolas;
        DadosJogo.getInstance().Pontos = atual.Pontos;
        DadosJogo.getInstance().Vida = atual.Vida;
        actualTime = atual.Time;

        timeCheckpointLoaded = 0;
        textInformationCheckpoint.text = "Loaded Checkpoint " + actualCheckpoint;
        textInformationCheckpoint.enabled = true;
        onChekpointLoad = true;

        contCheckpointsRestarts++;

        disableButtons();
       
    }

    public void goToMainManu()
    {
        chekpointDadosJogo.Clear();
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }


}
