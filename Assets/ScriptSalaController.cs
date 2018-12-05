using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptSalaController : MonoBehaviour {
    public GameObject canonPrefab;
    public GameObject enemyPrefab;
    public Transform player;

    public float timeSpanw=3;
    private float time;

    private float minX, maxX;
    private float minY, maxY;
    private float minZ, maxZ;
    private int altura, comprimento, largura;
    
    
    private static Cell[][][] mat;
    private static int numInimigosAtivos;
    private static int maxInimigosAtivosPeriodicamente=7;
    public static void desocuparMatriz(int x,int y,int z)
    {
        numInimigosAtivos--;
        mat[x][y][z].ocupada = false;
    }

    struct Cell
    {
        public bool ocupada;
        public Vector3 position;
    }


    // Use this for initialization
    void Start () {

        Vector3 cannonSize = canonPrefab.GetComponent<MeshRenderer>().bounds.size;

        float maxSizeCannon = Mathf.Max(cannonSize.x, cannonSize.y, cannonSize.z);

        minX = transform.position.x - transform.localScale.x  + maxSizeCannon / 2;
        maxX = transform.position.x + transform.localScale.x - maxSizeCannon / 2;

        Debug.Log(minX + "," + maxX);
        minY = transform.position.y - transform.localScale.z / 2 + maxSizeCannon / 2;
        maxY = transform.position.y + transform.localScale.z / 2 - maxSizeCannon / 2;

        minZ = transform.position.z;
        maxZ = transform.position.z + transform.localScale.y / 2 - maxSizeCannon / 2;

       
        largura = Mathf.RoundToInt((maxX - minX) / maxSizeCannon);
        altura = Mathf.RoundToInt((maxY - minY) / maxSizeCannon);
        comprimento = Mathf.RoundToInt((maxZ - minZ) / maxSizeCannon);

        Debug.Log("Largura: " + largura);
        Debug.Log("altura: " + altura);
        Debug.Log("comprimento: " + comprimento);


        mat = new Cell[largura][][];

        Vector3 d = new Vector3((maxX - minX) / largura, (maxY - minY) / altura, (maxZ - minZ) / comprimento);

        for (int x = 0; x < largura; x++)
        {
            mat[x] = new Cell[altura][];
            for (int y = 0; y < altura; y++)
            {
                mat[x][y] = new Cell[comprimento];
                for (int z = 0; z < comprimento; z++)
                {
                    //mat[x][y][z] = new Cell();
                    mat[x][y][z].ocupada = false;
                    mat[x][y][z].position = new Vector3(minX + x * d.x + d.x / 2, minY + y * d.y + d.y / 2, minZ + z * d.z + d.z / 2);
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (time > timeSpanw)
        {
            if (numInimigosAtivos < maxInimigosAtivosPeriodicamente)
            {
                int x, y, z;
                x = RandomGenerator.Instance.getRamdom(0, largura);
                y = RandomGenerator.Instance.getRamdom(0, altura);
                z = RandomGenerator.Instance.getRamdom(0, comprimento);
   
                Debug.Log(x + ", " + y + ", " + z);
                int tentativas = 0;
                while (mat[x][y][z].ocupada && tentativas < 100)
                {
                    x = RandomGenerator.Instance.getRamdom(0, largura);
                    y = RandomGenerator.Instance.getRamdom(0, altura);
                    z = RandomGenerator.Instance.getRamdom(0, comprimento);
                    tentativas++;
                }
                if (!mat[x][y][z].ocupada)
                {
                    mat[x][y][z].ocupada = true;
                    Vector3 positionCanon = mat[x][y][z].position;

                    GameObject enemy = Object.Instantiate(enemyPrefab);
                    enemy.transform.position = positionCanon;
                    ScriptEnemyController scriptEnemyController = enemy.GetComponent<ScriptEnemyController>();
                    scriptEnemyController.player = player;
                    scriptEnemyController.CellPosition = new Vector3Int(x, y, z);
                    numInimigosAtivos++;
                }
            }
            time = 0;
        }
        time += Time.deltaTime;

    }
}
