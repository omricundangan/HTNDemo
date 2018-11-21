using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgentController : MonoBehaviour {

    public GameManager gameMgr;
    private int numItems = 0;
    public bool isAlive = true;
    public int numTeleportTraps = 2;
    public float teleDistance = 150;
    public bool isHiding = true;

    public GameObject[] itemsRemaining;
    public GameObject[] hidingSpots;
    public NavMeshAgent agent;
    public EnemyAgentPlanner planner;
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
        planner = GetComponent<EnemyAgentPlanner>();
        
    }

    private void Update()
    {
        UpdateWorldState();

        planner.MakePlan();
    }

    void UpdateWorldState()
    {
        planner.CurrentWorldState.setNumTeleports(numTeleportTraps);

        float playerDist = -1;

        Vector3 diff = FindClosestEnemy().transform.position - transform.position;
        float enemyDist = diff.sqrMagnitude;
        if (FindAgent() != null)    // in case Player gets removed from game
        {
            Vector3 diff2 = FindAgent().transform.position - transform.position;
            playerDist = diff2.sqrMagnitude;
        }

        if (playerDist < teleDistance && playerDist != -1)
        {
            planner.CurrentWorldState.setPlayerNear(true);
        }
        else if (playerDist >= teleDistance || playerDist == -1)
        {
            planner.CurrentWorldState.setPlayerNear(false);
        }
        if (enemyDist < teleDistance)
        {
            planner.CurrentWorldState.setEnemyNear(true);
        }
        else if (enemyDist >= teleDistance)
        {
            planner.CurrentWorldState.setEnemyNear(false);
        }

        if(transform.position == GetBestHidingSpot().transform.position)
        {
            planner.CurrentWorldState.isHidden = true;
        }
        else
        {
            planner.CurrentWorldState.isHidden = false;
        }
    }
    
    // NOT USED. Obsolete!!!
    void primitiveAI() {
        if (!frozen)
        {
            Vector3 diff = FindClosestEnemy().transform.position - transform.position;
            float enemyDist = diff.sqrMagnitude;
            if (enemyDist < 150)
            {
                GoToBestHidingSpot();
            }
            else
            {
                GoToClosestItem();
            }
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
        else if(closestAgent)
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

        // If the agent is closer to the closest alcove than the enemy, this is the best alcove
        // Or, if the closest spot is the edge spot, this is the best spot
        if ((closest == gameMgr.spawner.leftSpot || closest == gameMgr.spawner.rightSpot) || diffAgent.sqrMagnitude < diffEnemy.sqrMagnitude)
        {
            return closest;
        }
        else // otherwise, the best alcove is the second closest alcove to the agent
        {
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

    public void Idle()
    {
        agent.destination = transform.position;
    }
}
