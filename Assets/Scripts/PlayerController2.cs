using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController2 : MonoBehaviour {

    public float playerSpeed = 5; //speed player moves
    private int numItems = 0;
    public int numTeleportTraps = 2;
    public GameManager gameMgr;
    public bool isAlive = true;
    public Text numTraps;

    // Use this for initialization
    void Start () {
        numTraps = gameMgr.GetComponent<GameManager>().trapsRemDisplay;
        numTraps.text = "Teleport Traps Remaining: " + numTeleportTraps + " ('Space' to use)";
    }
   
   void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space) && numTeleportTraps > 0){
            ActivateTeleportTrap();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            gameMgr.GetComponent<GameManager>().a2.ActivateTeleportTrap();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameMgr.agent2.GetComponent<EnemyAgentController>().GetBestHidingSpot();
        }
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal") * playerSpeed;
        float moveVertical = Input.GetAxis("Vertical") * playerSpeed;
        
        transform.Translate(moveHorizontal * Time.deltaTime, 0, moveVertical * Time.deltaTime);
    }

    public void IncrementNumItems()
    {
        numItems++;
        // should we check if the game is over here?
        gameMgr.notifyItemPickedUp();
    }

    public int GetNumItems()
    {
        return numItems;
    }

    void ActivateTeleportTrap()
    {
        numTeleportTraps--;
        numTraps.text = "Teleport Traps Remaining: " + numTeleportTraps + " ('Space' to use)";

        GameObject closestEnemy = FindClosestEnemy();
        GameObject closestAgent = FindAgent();

        print("Closest enemy: " + closestEnemy);

        Vector3 diff = Vector3.zero;

        if (closestEnemy)
        {
            diff = closestEnemy.transform.position - transform.position;
        }
        float enemyDist = diff.sqrMagnitude;

        if (closestAgent)
        {
            diff = closestAgent.transform.position - transform.position;
        }
        float agentDist = diff.sqrMagnitude;

        if(closestEnemy && enemyDist < agentDist || enemyDist == agentDist)
        {
            gameMgr.OverlayText("You teleported an enemy!", 3);
            closestEnemy.GetComponent<EnemyController>().respawning = true;
            closestEnemy.GetComponent<EnemyController>().Respawn();
        }
        else
        {
            gameMgr.OverlayText("You teleported your opponent!", 3);
            closestAgent.GetComponent<EnemyAgentController>().Respawn();
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public GameObject FindAgent()
    {
        return GameObject.FindGameObjectWithTag("EnemyAgent");
    }

    public void Caught()
    {
        this.gameObject.SetActive(false);
        isAlive = false;
        gameMgr.notifyCaught("You were seen by the enemy!\nWait for the game to end.");
    }

    public void Respawn()
    {
        transform.position = gameMgr.spawner.GetComponent<SpawnController>().agentSpawnPoints[Random.Range(0, 10)].transform.position
            + new Vector3(0, 0.5f, 0); ;
    }

}
