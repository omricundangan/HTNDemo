using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> subTasks = new List<Node>();
    public TASKS task;
    public TASKTYPE type;
    public WorldState effects;

    public Node FindSatisfiedMethod(WorldState ws)
    {
        return null;
    }
}

// List of tasks available`
public enum TASKS
{
    RootTask,
    Teleport,
    Avoid,
    Move,
    GetItem
}

public enum TASKTYPE
{
    Compound,
    Primitive
}

public class WorldState
{
    public int remainingTeleports = 2;
    public Vector3 position;

    public void ApplyEffects(WorldState effects)
    {
        this.remainingTeleports = effects.remainingTeleports;
        this.position = effects.position;
    }
}


public class EnemyAgentPlanner : MonoBehaviour {

    public EnemyAgentController ctrlr;
    public WorldState CurrentWorldState;
    public Stack<Node> TasksToProcess;
    public Stack<Node> FinalPlan;
    public Node RootTask;

	void Start () {
        ctrlr = GetComponent<EnemyAgentController>();
        TasksToProcess = new Stack<Node>();
        FinalPlan = new Stack<Node>();

        // Create Tree
        RootTask = new Node();
        RootTask.task = TASKS.RootTask;
	}
	
	void Update () {
		
	}

    void MakePlan()
    {
        WorldState WorkingWS = CurrentWorldState;
        TasksToProcess.Push(RootTask);
        while(TasksToProcess.Count != 0)
        {
            Node CurrentTask = TasksToProcess.Pop();
            if(CurrentTask.type == TASKTYPE.Compound)
            {
                Node SatisfiedMethod = CurrentTask.FindSatisfiedMethod(WorkingWS);
                if(SatisfiedMethod != null)
                {
                    // RecordDecomposition?
                    foreach (Node subTask in SatisfiedMethod.subTasks)
                    {
                        TasksToProcess.Push(subTask);   // InsertTop(subTask) ??
                    }
                }
                else
                {
                    // RestoreToLastDecomposedTask();
                }
            }
            else // Primitive Task
            {
                if (PrimitiveConditionMet(CurrentTask)){
                    WorkingWS.ApplyEffects(CurrentTask.effects);
                    FinalPlan.Push(CurrentTask);    // PushBack(CurrentTask) ??
                }
                else
                {
                    //RestoreToLastDecomposedTask();
                }
            }
        }
    }

    void RecordDecompositionOfTask(Node currentTask, Stack<Node> finalPlan, Stack<Node> decompHistory)
    {

    }

    void RestoreToLastDecomposedTask()
    {

    }

    bool PrimitiveConditionMet(Node CurrentTask)
    {
        return false;
    }

    void EvaluateNode(Node n)
    {
        if(n.task == TASKS.RootTask)
        {

        }
    }

    void ExecuteNode()
    {

    }

}

