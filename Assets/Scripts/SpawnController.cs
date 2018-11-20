using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

    public GameObject agent1;
    public GameObject agent2;
    public GameObject enemy;
    public GameObject[] agentSpawnPoints = new GameObject[10];
    public GameObject[] enemySpawnPoint1 = new GameObject[2];
    public GameObject[] enemySpawnPoint2 = new GameObject[2];
    public GameObject gsm;

    public GameObject enemy1;
    public GameObject enemy2;

    public bool spawnEnemies = true;

    public GameManager manager;

    void Start () {
        manager = gsm.GetComponent<GameManager>();
        manager.spawner = this;
        SpawnAgents();
        manager.SetAgentScripts();
        manager.alcoves = agentSpawnPoints;
        if (spawnEnemies)
        {
            SpawnEnemies();
        }
        manager.GetPickUps();
    }

    // Select two distinct random spawn points and spawn the agents there.
    void SpawnAgents()
    {
        // Get two random, distinct indexes to select which spawn point the agents spawn in
        int rng1 = Random.Range(0, agentSpawnPoints.Length);
        int rng2 = Random.Range(0, agentSpawnPoints.Length);
        while (rng2 == rng1) // keep selecting a new number until the numbers are not the same
        {
            rng2 = Random.Range(0, agentSpawnPoints.Length);
        }

        GameObject a1 = Instantiate(agent1,
            agentSpawnPoints[rng1].transform.position + new Vector3(0, 0.5f, 0),
            Quaternion.identity);
        a1.GetComponent<PlayerController2>().gameMgr = manager;
        manager.agent1 = a1;


        GameObject a2 = Instantiate(agent2,
            agentSpawnPoints[rng2].transform.position + new Vector3(0, 4f, 0),
            Quaternion.identity);
        a2.GetComponent<EnemyAgentController>().gameMgr = manager;
        manager.agent2 = a2;

    }

    void SpawnEnemies()
    {
        // Get two random, distinct indexes to select which spawn point the agents spawn in
        int rng1 = Random.Range(0, enemySpawnPoint1.Length);
        int rng2 = Random.Range(0, enemySpawnPoint2.Length);

        GameObject enemy1 = Instantiate(enemy,
            enemySpawnPoint1[rng1].transform.position + new Vector3(0, 0, 0),
            Quaternion.identity);

        GameObject enemy2 = Instantiate(enemy,
            enemySpawnPoint2[rng2].transform.position + new Vector3(0, 0, 0),
            Quaternion.identity);

        if(rng1 == 1)
        {
            enemy1.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        for(int i = 0; i <enemySpawnPoint1.Length; i++)
        {
            enemy1.GetComponent<EnemyController>().doorways[i] = enemySpawnPoint1[i];
        }
        enemy1.GetComponent<EnemyController>().respawning = true;
        this.enemy1 = enemy1;


        if (rng2 == 1)
        {
            enemy2.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        for (int i = 0; i < enemySpawnPoint2.Length; i++)
        {
            enemy2.GetComponent<EnemyController>().doorways[i] = enemySpawnPoint2[i];
        }
        enemy2.GetComponent<EnemyController>().respawning = true;
        this.enemy2 = enemy2;
    }
}
