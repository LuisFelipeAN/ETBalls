using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBolasController : MonoBehaviour {
    public float timeToDisappear;
    private float time;
    // Use this for initialization
    void Start () {
        time = 0;

    }
	
	// Update is called once per frame
	void Update () {
        if (time > timeToDisappear)
        {
            Destroy(gameObject);
        }
        time += Time.deltaTime;
	}
}
