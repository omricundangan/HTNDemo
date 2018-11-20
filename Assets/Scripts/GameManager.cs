using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject agent1;
    public GameObject agent2;
    public PlayerController2 a1;
    public EnemyAgentController a2;

    public Text gameOverText;
    public Text overMsg;
    public Text winnerMsg;
    public Text caughtMsg;
    public GameObject overlayMsg;
    public SpawnController spawner;
    public Text trapsRemDisplay;
    public Text playerScore;
    public Text enemyScore;

    public GameObject[] items;
    public GameObject[] alcoves;


    // Use this for initialization
    void Start () {
        GetPickUps();   

    }
	
	// Update is called once per frame
	void Update () {

	}

    public void GetPickUps()
    {
        items = GameObject.FindGameObjectsWithTag("Pick Up");
    }

    public void SetAgentScripts()
    {
        a1 = agent1.GetComponent<PlayerController2>();
        a2 = agent2.GetComponent<EnemyAgentController>();
    }

    public void notifyItemPickedUp()
    {
        GetPickUps();
        playerScore.text = "" + a1.GetNumItems();
        enemyScore.text = "" + a2.GetNumItems();

        if(a1.GetNumItems() + a2.GetNumItems() >= 10)
        {
            a1.gameObject.SetActive(false);
            a2.gameObject.SetActive(false);
            GameOver("All items were picked up.");
        }
    }

    void RemoveText()
    {
        overlayMsg.SetActive(false);
        caughtMsg.text = "";
    }

    public void OverlayText(string s, float time)
    {
        overlayMsg.SetActive(true);
        caughtMsg.text = s;
        Invoke("RemoveText", time);
    }

    public void notifyCaught(string msg)
    {
        if (!a1.isAlive && !a2.isAlive)
        {
            GameOver("Both agents were caught.");
        }
        else
        {
            OverlayText(msg, 5);
        }
    }

    public void GameOver(string reason)
    {
        overlayMsg.SetActive(false);
        caughtMsg.text = "";
        /*
        playerScore.text = "";
        enemyScore.text = "";
        */
        string winMsg = "";
        if(a1.GetNumItems() > a2.GetNumItems())
        {
            winMsg = "The Player won with a score of " + a1.GetNumItems() + "-" + a2.GetNumItems() + ".";
        }
        else if (a1.GetNumItems() < a2.GetNumItems())
        {
            winMsg = "The Enemy won with a score of " + a2.GetNumItems() + "-" + a1.GetNumItems() + ".";
        }
        else
        {
            winMsg = "Both agents tied with a score of " + a1.GetNumItems() +".";
        }

        gameOverText.text = "GAME\nOVER";
        winnerMsg.text = winMsg;
        overMsg.text = reason;

        gameOverText.gameObject.SetActive(true);
        winnerMsg.gameObject.SetActive(true);
        overMsg.gameObject.SetActive(true);

        if (spawner.enemy1 && spawner.enemy2)
        {
            spawner.enemy1.GetComponent<EnemyController>().forwardSpeed = 0;
            spawner.enemy2.GetComponent<EnemyController>().forwardSpeed = 0;
        }
    }
}
