using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    // Get the doorways that this enemy uses ??? dunno yet
    public GameObject[] doorways = new GameObject[2];
    public float forwardSpeed = 5;
    private bool visionState = true;
    public bool respawning = true;
    public GameObject vision;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        MoveForward();
	}

    // moving forward every frame
    public void MoveForward()
    {
        transform.Translate(forwardSpeed * Time.deltaTime, 0, 0);
    }

    // chance of being called when entering an obstacle
    public void Respawn()
    {
        int rng1 = Random.Range(0, doorways.Length);
        if (rng1 == 1)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        transform.position = doorways[rng1].transform.position;
        
    }

    // chance of being called when entering an obstacle
    public void ReverseDirection()
    {
        transform.Rotate(new Vector3(0, 180, 0));
    }

    // called when entering and exiting an obstacle
    public void ToggleVision()
    {
        visionState = !visionState;
        transform.GetChild(0).gameObject.SetActive(visionState);
    }

}
