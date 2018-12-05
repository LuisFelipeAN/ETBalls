using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptEnemyController : MonoBehaviour {
    public Transform player;
    
    public float aberturaMaxima=90;
    public float rotationSpeed = 10;
    private float aberturaAtual;
    private float initialRotation;
    private float speedR;
    public float maxIncinacao = -40f;
    float inclinacao;
    private Vector3Int cellPosition;

    public Vector3Int CellPosition
    {
        set { cellPosition = value; }
    }


    public Transform getParent()
    {
        return transform.parent;
    }
    public void destroy()
    {
        ScriptSalaController.desocuparMatriz(cellPosition.x, cellPosition.y, cellPosition.z);
        Destroy(gameObject);
    }
    // Use this for initialization
    void Start () {

        transform.LookAt(player);
        inclinacao=RandomGenerator.Instance.getRamdom(10 * Mathf.FloorToInt(maxIncinacao), 0) /10;

        initialRotation = transform.rotation.eulerAngles.y;

        transform.rotation = Quaternion.Euler(inclinacao, initialRotation, 0);

        aberturaAtual = 0;
        
        if(Random.Range(0, 100) < 50)
        {
            speedR = rotationSpeed;
        }
        else
        {
            speedR = -rotationSpeed;
        }
        

    }
	
	// Update is called once per frame
	void Update () {
        if (aberturaAtual > aberturaMaxima/2)
        {

            speedR = -speedR;
        }else if (aberturaAtual < -aberturaMaxima / 2)
        {
            speedR = -speedR;
        }
        aberturaAtual += Time.deltaTime * speedR;

        transform.rotation = Quaternion.Euler(inclinacao, initialRotation+aberturaAtual, 0);
    }
}
