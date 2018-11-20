using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayController : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!other.gameObject.GetComponent<EnemyController>().respawning)
            {
                // Respawn the enemy
                other.gameObject.GetComponent<EnemyController>().Respawn();
                other.gameObject.GetComponent<EnemyController>().respawning = true;
            }
            else
            {
                other.gameObject.GetComponent<EnemyController>().respawning = false;
            }
        }
    }
}
