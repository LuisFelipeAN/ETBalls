using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTargetController : MonoBehaviour {
    public int valorPontos;

    private SphereCollider boxCollider;
    private Rigidbody rigidbody;
    private MeshCollider meshCollider;
    private Vector3 position;
    private Quaternion rotation;
    private bool acertado;

    

    // Use this for initialization
    void Start () {
        boxCollider = GetComponent<SphereCollider>();
        meshCollider = GetComponent<MeshCollider>();
        rigidbody = GetComponent<Rigidbody>();
        position = transform.localPosition;
        rotation = transform.localRotation;
        acertado = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!acertado)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.name.StartsWith("PrefabBolaVerde")&&!acertado)
        {
            ScriptEnemyController parentScript = GetComponentInParent<ScriptEnemyController>();
            transform.parent = parentScript.getParent();

            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            boxCollider.enabled = false;
            meshCollider.enabled = true;
            acertado = true;
            DadosJogo.getInstance().Pontos = DadosJogo.getInstance().Pontos + valorPontos;
            parentScript.destroy();
        }
    }
}
