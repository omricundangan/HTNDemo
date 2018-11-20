using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();

            // Toggle the enemy's field of vision when inside the obstacle
            enemy.ToggleVision();

            // Randomly choose between respawning the enemy, reversing direction, or doing nothing
            int rng = Random.Range(0, 3);  
            if(rng == 0)
            {
                // do nothing
            }
            else if(rng == 1)
            {
                enemy.gameObject.GetComponent<EnemyController>().respawning = true;
                enemy.Respawn();
            }
            else if(rng == 2)
            {
                enemy.ReverseDirection();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // re-toggle the enemy's field of vision
            other.gameObject.GetComponent<EnemyController>().ToggleVision();
        }
    }
}
