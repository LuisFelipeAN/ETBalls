using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour {
    public float eyeSpeed=1f;
    public float camSpeed=7f;
    public float shotImpulse=1000f;
    public Rigidbody bullet;

    private float mouseH = 0;
    private float mouseV = 0;
    public Camera cam;
    private Quaternion initialOrientation;
    public float distanciaMaxParede = 0.5f;
    private bool paredeDireita, paredeEsquerda;
    private Vector3 deslocamentoLateral;
    // Use this for initialization
    void Start () {
        initialOrientation = transform.localRotation;
        deslocamentoLateral = new Vector3();
        //Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {

        atualizaRayCasts();

        mouseH += Input.GetAxis("Mouse X");
        mouseV += Input.GetAxis("Mouse Y");

        Quaternion rotH = Quaternion.AngleAxis(mouseH * eyeSpeed, Vector3.up);
        Quaternion rotV = Quaternion.AngleAxis(mouseV * eyeSpeed, Vector3.left);
        transform.localRotation = initialOrientation * rotH * rotV;

        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody b = Object.Instantiate(bullet);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            b.gameObject.transform.position = transform.position;
            Debug.Log(ray.direction + ", " + shotImpulse);
            b.AddForce(ray.direction * shotImpulse);
        }else if(Input.GetMouseButtonDown(1)){
            transform.localRotation = initialOrientation;
        }
        if (Input.GetKey(KeyCode.LeftArrow)&&!paredeEsquerda)
        {

            deslocamentoLateral.x = transform.position.x - camSpeed * Time.deltaTime;
            deslocamentoLateral.y = transform.position.y;
            deslocamentoLateral.z = transform.position.z;
            transform.position = deslocamentoLateral;
        }
        if (Input.GetKey(KeyCode.RightArrow)&&!paredeDireita)
        {
            deslocamentoLateral.x = transform.position.x + camSpeed * Time.deltaTime;
            deslocamentoLateral.y = transform.position.y;
            deslocamentoLateral.z = transform.position.z;
            transform.position = deslocamentoLateral;
        }
        
    }
    private void atualizaRayCasts() {
        paredeDireita=Physics.Raycast(transform.position, Vector3.right, distanciaMaxParede);
        paredeEsquerda = Physics.Raycast(transform.position, Vector3.left, distanciaMaxParede);
    }
    
}
