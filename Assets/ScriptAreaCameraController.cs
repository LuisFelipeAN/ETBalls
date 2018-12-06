using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptAreaCameraController : MonoBehaviour,IObserver {
    public RectTransform RectTransformVida;
    private SpriteRenderer spriteRenderer;
    public Color corAoSerAtingido;
    private Color initialColor;
    public int MaxVida=100;
    public int danoPorAcerto=5;
    public float timeResetarAcerto = 0.5f;
    private float time;
    private bool acertado;
    
    private float dv;


    public void notify()
    {
        RectTransformVida.sizeDelta = new Vector2(dv * DadosJogo.getInstance().Vida, RectTransformVida.sizeDelta.y);
    }

    // Use this for initialization
    void Start () {
        dv = RectTransformVida.sizeDelta.x / MaxVida;
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
        DadosJogo.getInstance().addObserver(this);
        DadosJogo.getInstance().Vida = MaxVida;
    }
	
	// Update is called once per frame
	void Update () {
        if (acertado)
        {
            if (time > timeResetarAcerto)
            {
                time = 0;
                acertado = false;
                spriteRenderer.color = initialColor;
            }
            time += Time.deltaTime;
        }
      

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("PrefabBolaVermelha"))
        {
            DadosJogo.getInstance().Vida = DadosJogo.getInstance().Vida - danoPorAcerto;
            spriteRenderer.color = corAoSerAtingido;
            acertado = true;
            time = 0;
        }
    }
}
