using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController2>().Caught();
        }
        else if (other.gameObject.CompareTag("EnemyAgent"))
        {
            other.gameObject.GetComponent<EnemyAgentController>().Caught();
        }
    }
}
