using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgentController : MonoBehaviour {

    public GameManager gameMgr;
    private int numItems = 0;
    public bool isAlive = true;
    public int numTeleportTraps = 2;

    public GameObject[] itemsRemaining;
    public GameObject[] hidingSpots;
    public NavMeshAgent agent;
    private bool frozen = false;
    
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        itemsRemaining = gameMgr.items;
        hidingSpots = new GameObject[12];
        for(int i = 0; i < gameMgr.alcoves.Length; i++)
        {
            hidingSpots[i] = gameMgr.alcoves[i];
        }
        hidingSpots[10] = gameMgr.spawner.leftSpot;
        hidingSpots[11] = gameMgr.spawner.rightSpot;
    }

    // Update is called once per frame
    void Update () {
        if (!frozen)
        {
            Vector3 diff = FindClosestEnemy().transform.position - transform.position;
            float enemyDist = diff.sqrMagnitude;
            print(enemyDist);
            if (enemyDist < 150)
            {
                print("enemy too close!");
                GoToBestHidingSpot();
            }
            else
            {
               // print("enemy is far enough!");
                GoToClosestItem();
            }
            // print(agent.destination);
        }
    }

    public void IncrementNumItems()
    {
        numItems++;
        gameMgr.notifyItemPickedUp();
    }

    public int GetNumItems()
    {
        return numItems;
    }

    public void Caught()
    {
        this.gameObject.SetActive(false);
        isAlive = false;
        gameMgr.notifyCaught("The enemy agent has been caught!");
    }

    public void Respawn()
    {
        agent.Warp(gameMgr.spawner.GetComponent<SpawnController>().agentSpawnPoints[Random.Range(0, 10)].transform.position
            + new Vector3(0, 0.5f, 0));
    }

    // public just for testing
    public void ActivateTeleportTrap()
    {
        numTeleportTraps--;

        GameObject closestEnemy = FindClosestEnemy();
        GameObject closestAgent = FindAgent();

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

        if (closestEnemy && enemyDist < agentDist)
        {
            gameMgr.OverlayText("Your opponent teleported an enemy!", 3);
            closestEnemy.GetComponent<EnemyController>().respawning = true;
            closestEnemy.GetComponent<EnemyController>().Respawn();
        }
        else
        {
            gameMgr.OverlayText("Your opponent teleported you!", 3);
            closestAgent.GetComponent<PlayerController2>().Respawn();
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
        return GameObject.FindGameObjectWithTag("Player");
    }

    public GameObject GetClosestItem()
    {
        itemsRemaining = gameMgr.items;
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        for(int i = 0; i < itemsRemaining.Length; i++)
        {
            if (itemsRemaining[i])
            {
                Vector3 diff = itemsRemaining[i].transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = itemsRemaining[i];
                    distance = curDistance;
                }
            }
        }

       // print("Closest item: " + closest);
        return closest;
    }

    public GameObject GetClosestAlcove()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 diff = hidingSpots[i].transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = hidingSpots[i];
                distance = curDistance;
            }
        }
        print("Closest alcove: " + closest);
        return closest;
    }

    public GameObject GetBestHidingSpot()
    {
        GameObject closest = null;
        GameObject second = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 diff = hidingSpots[i].transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                second = closest;
                closest = hidingSpots[i];
                distance = curDistance;
            }
        }

        GameObject enemy = FindClosestEnemy();
        GameObject vision = enemy.GetComponent<EnemyController>().vision;
        Bounds bds = vision.GetComponent<Renderer>().bounds;

        Vector3 closestPt = bds.ClosestPoint(closest.transform.position);

        Vector3 diffEnemy = closest.transform.position - closestPt;
        Vector3 diffAgent = closest.transform.position - position;

        /*
        print("Enemy: "+ closestPt);
        print("Agent: "+ position);
        print("Alcove: "+ closest.transform.position);
        print("dist between enemy = " + diffEnemy.sqrMagnitude);

        print(diffEnemy.sqrMagnitude);
        print(diffAgent.sqrMagnitude);

        enemy.GetComponent<EnemyController>().forwardSpeed = 0;
        frozen = true;
        agent.destination = position;
        */

        // If the agent is closer to the closest alcove than the enemy, this is the best alcove
        // Or, if the closest spot is the edge spot, this is the best spot
        if ((closest == gameMgr.spawner.leftSpot || closest == gameMgr.spawner.rightSpot) || diffAgent.sqrMagnitude < diffEnemy.sqrMagnitude)
        {
            print("Agent is closer");
            return closest;
        }
        else // otherwise, the best alcove is the second closest alcove to the agent
        {
            print("Enemy is closer");
            return second;
        }
    }

    public void GoToClosestAlcove()
    {
        agent.destination = GetClosestAlcove().transform.position;
    }

    public void GoToClosestItem()
    {
        agent.destination = GetClosestItem().transform.position;
    }

    public void GoToBestHidingSpot()
    {
        agent.destination = GetBestHidingSpot().transform.position;
    }
}
