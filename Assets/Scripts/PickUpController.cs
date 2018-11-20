using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController2>().IncrementNumItems();
            Destroy(this.gameObject);
        }
        else if (other.gameObject.CompareTag("EnemyAgent"))
        {
            other.gameObject.GetComponent<EnemyAgentController>().IncrementNumItems();
            Destroy(this.gameObject);
        }
    }
}
