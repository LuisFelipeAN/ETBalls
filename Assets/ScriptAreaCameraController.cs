using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptAreaCameraController : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    public Color corAoSerAtingido;
    private Color initialColor;
    public int danoPorAcerto=5;
    public float timeResetarAcerto = 0.5f;
    private float time;
    private bool acertado;
    
    private float dv;

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
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
