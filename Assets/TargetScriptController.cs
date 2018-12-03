using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScriptController : MonoBehaviour {
    private SphereCollider boxCollider;
    private Rigidbody rigidbody;
    private Vector3 position;
    private Quaternion rotation;
    private bool acertado;

    // Use this for initialization
    void Start () {
        boxCollider = GetComponent<SphereCollider>();
        rigidbody = GetComponent<Rigidbody>();
        position = transform.position;
        rotation = transform.rotation;
        acertado = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!acertado)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.name.StartsWith("PrefabBolaVerde"))
        {
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            boxCollider.enabled = false;
            acertado = true;
        }
    }
}
