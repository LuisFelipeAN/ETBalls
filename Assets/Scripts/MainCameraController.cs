using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainCameraController : MonoBehaviour,IObserver {
    public float eyeSpeed=1f;//velocidade de rotação da camera com o mouse
    public float camSpeed=7f;//velocidade de movimentacao lateral da camera
    public float shotImpulse=1000f;//impulso da bola disparada
    public float distanciaMaxParede = 0.5f;//o quao proximo pode chegar da parede

    //informações para a implementacao do nivel atual
    public int level = 1;
    public bool isLastLevel;
    public int pontosProximoNivel;

    public int numMaxBolas = 150;//numero maximo de bolas disponiveis
    public int MaxVida = 100;//vida maxima
    public float maxTime = 120;//tempo maximo

    public float timeMaxInformationCheckpointLoaded = 3f;//tempo maximo que um Text infomativo de level e checkpoint aprece na interface
    public List<int> chekpoints;//vetor de checkpoints contendo os pontos necessarios em cada um
    public int maxCheckpointRestarts = 3;//controla o numero maximo de checkpoints por lvl

    public Rigidbody bullet;//rigidbody da bala
    public Button bntRestartOnCheckpointPrefab;// botao de restart a ser instanciado
    public Button bntMainMenuPrefab;// botao de mai manu a ser instanciado

    private float timeCheckpointLoaded;//controla o tempo corrente que a Text que informa lvl e checkpoits esta sendo visualizada
    private bool onChekpointLoad;//controla se o jogador atingiu um novo checkpoint ou lvl
    private Text timeText;//exibe o tempo atual na interface
    private Canvas canvas;//possui os elementos de interface

    //ambos os botoes a seguir sao exibidos ao necessitar retornar a um checkpoint
    private Button bntRestartOnCheckpoint;//instancia do botao de checkpoint
    private Button bntMainMenu;//instancia do botao de voltar ao menu

    private Text textBolas;//text que informa o numero de bolas
    private Text textPontos;//text que informa o numero de pontos

    private RectTransform mira;//utilizado para mover a mira para a posicao do mause
    private RectTransform RectTransformVida;//utilizado para decrescer o tamanho da barra de vida
    private ScriptSalaController scriptSalaController;//utilizado para informar a sala que esta na hora de iniciar uma "horda" de canhões
    private Camera cam;//necessario para movimenta o jogaodor
    private GameObject sala;//game object da sala
    private Text textInformationCheckpoint;//Text que informa o checkpoint e o lvl alcancado

    private int contCheckpointsRestarts;//conta o numero de retornos a checkpoint
    private float actualTime;//tempo restante da fase
    
    private float mouseH = 0;
    private float mouseV = 0;
    private Quaternion initialOrientation;

    private bool paredeDireita, paredeEsquerda;//utilizado por ray casts para saber se a camera esta proxima de uma parede e nao pode mais se movimentar

    private Vector3 deslocamentoLateral;//necessario para nao ficar alocando vetor toda hora para movimentar a camera apenas para o lado
    private int bolas;//o numero de bolas disparadas
    private float dv;//a variacao do tamanho da barra de vida por cada ponto a menos de vida
    private int actualCheckpoint;//conta o checkpoint atual
    private bool gameOver;

    private List<Chekpoint> chekpointDadosJogo;//lista de checkpoints alcançados

    // Use this for initialization
    private void Awake()
    {
        //procura os game objects
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

        //remove todos os observadores
        DadosJogo.getInstance().clearObservers();

    }
    //struct para salvar os parametros do checkpoint 
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

        //calcula a variacao da barra de vida
        dv = RectTransformVida.sizeDelta.x / MaxVida;

        //inicia os dados do jogo
        DadosJogo.getInstance().getGame().Win = false;
        DadosJogo.getInstance().GameOver = false;
        DadosJogo.getInstance().Bolas = 0;
        DadosJogo.getInstance().Pontos = 0;
        DadosJogo.getInstance().addObserver(this);
        DadosJogo.getInstance().Vida = MaxVida;

        //instancia os botoes de voltar ao menu e retornar ao ultimo checkpoint e esm seguida os desabilita
        bntRestartOnCheckpoint = Object.Instantiate(bntRestartOnCheckpointPrefab);
        bntRestartOnCheckpoint.transform.parent = canvas.transform;
        bntRestartOnCheckpoint.GetComponent<RectTransform>().localPosition= new Vector3(0,0,0);
        bntMainMenu =Object.Instantiate(bntMainMenuPrefab);
        bntMainMenu.transform.parent = canvas.transform;
        bntMainMenu.GetComponent<RectTransform>().localPosition = new Vector3(0,-77,0);
        disableButtons();

        //adiciona os listeners aos botoes de voltar ao menu e retornar ao ultimo checkpoint e esm seguida os desabilita
        bntMainMenu.onClick.AddListener(goToMainManu);
        bntRestartOnCheckpoint.onClick.AddListener(loadLastCheckpoint);


        //o tempo atual é igual ao tempo maximo
        actualTime = maxTime;

        //possiblilita que o level seja informado no text do checkpoint
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

            //controla a rotação ta camera
            mouseH += Input.GetAxis("Mouse X");
            mouseV += Input.GetAxis("Mouse Y");
            Quaternion rotH = Quaternion.AngleAxis(mouseH * eyeSpeed, Vector3.up);
            Quaternion rotV = Quaternion.AngleAxis(mouseV * eyeSpeed, Vector3.left);
            transform.localRotation = initialOrientation * rotH * rotV;

            //dispara uma bola no ray cast dado pelo clique do mause
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

            //movimentase para a esquerda independente da inclinacao
            if ((Input.GetKey(KeyCode.LeftArrow)|| Input.GetKey(KeyCode.A)) && !paredeEsquerda)
            {

                deslocamentoLateral.x = transform.position.x - camSpeed * Time.deltaTime;
                deslocamentoLateral.y = transform.position.y;
                deslocamentoLateral.z = transform.position.z;
                transform.position = deslocamentoLateral;
            }
            //movimentase para a direita independente da inclinacao
            if ((Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.D)) && !paredeDireita)
            {
                deslocamentoLateral.x = transform.position.x + camSpeed * Time.deltaTime;
                deslocamentoLateral.y = transform.position.y;
                deslocamentoLateral.z = transform.position.z;
                transform.position = deslocamentoLateral;
            }


            //controla a cor do text que informa o tempo 
            actualTime -= Time.deltaTime;
            if (actualTime < maxTime / 2&& actualTime > maxTime / 10)//se o tempo corrente for metade do tempo maximo mas mais que 10% coloca em amarelo
            {
                timeText.color = Color.yellow;
            }else if(actualTime<maxTime/10&&actualTime>0){//se o tempo corrente for memor que 10% do tempo maximo coloca em vermelho
                timeText.color = Color.red;
            }
            else if(actualTime < 0)//se o tempo for menor que 0 game over
            {
                DadosJogo.getInstance().GameOver = true;
            }
            timeText.text = Mathf.RoundToInt(actualTime)+"s";//informa o tempo na interface


            if (onChekpointLoad)//se esta em um checkpoint o inicio de level
            {
                if (timeCheckpointLoaded > timeMaxInformationCheckpointLoaded)//se o tempo de informe atual é maior que o tempo maximo de informe //disabilita o imforme e o text
                {
                    textInformationCheckpoint.enabled = false;
                    onChekpointLoad = false;
                }
                timeCheckpointLoaded += Time.deltaTime;
            }
        }
        mira.transform.position = Input.mousePosition;//coloca a imagem da mira presente no canvas na posicao do mouse

    }
    //dispara ray casts para a direita e esquerda da posicao da camera em ordem de identificar se esta nos limites laterais da sala
    private void atualizaRayCasts() {
        paredeDireita=Physics.Raycast(transform.position, Vector3.right, distanciaMaxParede);
        paredeEsquerda = Physics.Raycast(transform.position, Vector3.left, distanciaMaxParede);
    }
   
    //implementacao do mento notify do observer
    public void notify()
    {
        
        if (DadosJogo.getInstance().Pontos >= pontosProximoNivel)//se possui pontos para o proximo nivel
        {
            DadosJogo.getInstance().getGame().Win = true;//avisa a todos que venceu
            Cursor.visible = true;
            if (isLastLevel)//se é o ultimo nivel carreca cenha Win senão carrega a proxima fase
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
        if (gameOver && chekpointDadosJogo.Count > 0&& contCheckpointsRestarts<maxCheckpointRestarts)//se for game over existir um checkpoint e o numero de retornos a checkpoint for menor que o maximo entao exibe botoes de retorno
        {
            textInformationCheckpoint.enabled = false;
            enableButtons();

        }
        else if(gameOver)//se nao for carrega a tela de game over
        {
            Cursor.visible = true;
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
       

        //atualiza os elementos de ui dos dados do jogador
        textBolas.text = "Bolas: " + DadosJogo.getInstance().Bolas+ "/" + numMaxBolas;
        textPontos.text = "Pontos: " + DadosJogo.getInstance().Pontos;
        RectTransformVida.sizeDelta = new Vector2(dv * DadosJogo.getInstance().Vida, RectTransformVida.sizeDelta.y);

        //se alcancou um checkpoint
        if(actualCheckpoint< chekpoints.Count&&DadosJogo.getInstance().Pontos >= chekpoints[actualCheckpoint])
        {
            Debug.Log("Criando Checkpoint");


            Chekpoint c;//cria um checkpoint
            //copia os dados do jogo
            c.Bolas = DadosJogo.getInstance().Bolas;
            c.Pontos= DadosJogo.getInstance().Pontos;
            c.Time = actualTime;
            c.Vida = DadosJogo.getInstance().Vida;
            //adiciona na lista de checkpoints
            chekpointDadosJogo.Add(c);
            actualCheckpoint++;

            //avisa ao usuario na interface
            timeCheckpointLoaded = 0;
            textInformationCheckpoint.text = "Reached Checkpoint " + actualCheckpoint;
            textInformationCheckpoint.enabled = true;
            onChekpointLoad = true;

            //avisa a sala para iniciar a horda
            scriptSalaController.startHordaCheckpoint();
        }
    }

    //dasabilita os botoes de reiniciar no ultimo checkpoint e voltar ao menu
    private void disableButtons()
    {

        bntRestartOnCheckpoint.enabled = false;
        bntRestartOnCheckpoint.image.enabled = false;
        bntRestartOnCheckpoint.GetComponentInChildren<Text>().enabled = false;

        bntMainMenu.enabled = false;
        bntMainMenu.image.enabled = false;
        bntMainMenu.GetComponentInChildren<Text>().enabled = false;

    }
    //habilita os botoes de reiniciar no ultimo checkpoint e voltar ao menu
    private void enableButtons()
    {

        bntRestartOnCheckpoint.enabled = true;
        bntRestartOnCheckpoint.image.enabled = true;
        bntRestartOnCheckpoint.GetComponentInChildren<Text>().enabled = true;

        bntMainMenu.enabled = true;
        bntMainMenu.image.enabled = true;
        bntMainMenu.GetComponentInChildren<Text>().enabled = true;
    }
    //calback do botao de de reiniciar no ultimo checkpoint
    public void loadLastCheckpoint()
    {

        DadosJogo.getInstance().GameOver = false;
        Chekpoint atual = chekpointDadosJogo[actualCheckpoint - 1];//pega o ultimo point
        DadosJogo.getInstance().Bolas = atual.Bolas;
        DadosJogo.getInstance().Pontos = atual.Pontos;
        DadosJogo.getInstance().Vida = atual.Vida;
        actualTime = atual.Time;

        //avisa o usuario qual checkpoint carregou 
        timeCheckpointLoaded = 0;
        textInformationCheckpoint.text = "Loaded Checkpoint " + actualCheckpoint;
        textInformationCheckpoint.enabled = true;
        onChekpointLoad = true;

        contCheckpointsRestarts++;//soma mais um reinicio ao contador de checkpoints

        disableButtons();
       
    }

    //Calback do botao de voltar ao menu 
    public void goToMainManu()
    {
        chekpointDadosJogo.Clear();
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }


}
