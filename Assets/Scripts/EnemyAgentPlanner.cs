using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgentPlanner : MonoBehaviour {

    public static EnemyAgentController ctrlr;
    public WorldState CurrentWorldState;
    public Stack<Task> TasksToProcess;
    public Stack<Task> FinalPlan;
    public EnemyAgentHTN htn;

	void Start () {
        ctrlr = GetComponent<EnemyAgentController>();
        TasksToProcess = new Stack<Task>();
        FinalPlan = new Stack<Task>();
        CurrentWorldState = new WorldState(2, false, false, false);

        htn = new EnemyAgentHTN();
	}

    public void MakePlan()
    {
        WorldState WorkingWS = CurrentWorldState;
        TasksToProcess.Push(htn.taskRoot);
        while(TasksToProcess.Count != 0)
        {
            Task CurrentTask = TasksToProcess.Pop();
            if(CurrentTask.type == TYPE.Compound)
            {
                Method SatisfiedMethod = CurrentTask.FindSatisfiedMethod(WorkingWS);
                if (SatisfiedMethod != null)
                {
                    Stack<Task> reverseStack = new Stack<Task>();
                    foreach (Task subTask in SatisfiedMethod.subTasks)
                    {
                        reverseStack.Push(subTask);   // InsertTop(subTask) ??
                    }
                    while(reverseStack.Count != 0)    // Achieve InsertTop(subTask) by using two stacks
                    {
                        TasksToProcess.Push(reverseStack.Pop());
                    }
                }
            }
            else // Primitive Task
            {
                if (PrimitiveConditionMet(CurrentTask, CurrentWorldState)){
                    if (CurrentTask.effects != null)
                    {
                        WorkingWS.ApplyEffects(CurrentTask.effects);
                    }
                    FinalPlan.Push(CurrentTask);    // PushBack(CurrentTask) ??
                }
            }
        }
        ExecutePlan();
    }

    // Checks if the passed Task meets the pre conditions in the world state
    public static bool PrimitiveConditionMet(Task CurrentTask, WorldState ws)
    {
        switch (CurrentTask.task)
        {
            case TASKS.Teleport:
                if ((ws.numTeleports > 0) && (ws.playerNear || ws.enemyNear))
                {
                    return true;
                }
                return false;
            case TASKS.Idle:
                if(ws.enemyNear && ws.isHidden)
                {
                    return true;
                }
                return false;
            case TASKS.Hide:
                if (ws.enemyNear && !ws.isHidden)
                {
                    return true;
                }
                return false;
            case TASKS.GetItem:
                if (true) // do we need this? or if(true) instead?
                {
                    return true;
                }
            default:
                return false;
        }
    }

    // The FinalPlan stack is reversed, we have to reverse it to pop the Tasks out properly.
    void ExecutePlan()
    {
        Stack<Task> fixedFinalStack = new Stack<Task>();
        while(FinalPlan.Count != 0)
        {
            fixedFinalStack.Push(FinalPlan.Pop());   // InsertTop(subTask) ??
        }
        
        while(fixedFinalStack.Count != 0)
        {
            Task n = fixedFinalStack.Pop();
            EvaluateTask(n);
        }
    }

    // Executes Operator based on Primitive Task
    void EvaluateTask(Task n)
    {
        switch(n.task)
        {
            case TASKS.Teleport:
                ctrlr.ActivateTeleportTrap();
                break;
            case TASKS.Idle:
                ctrlr.Idle();
                break;
            case TASKS.Hide:
                ctrlr.GoToBestHidingSpot();
                break;
            case TASKS.GetItem:
                ctrlr.GoToClosestItem();
                break;
        }
    }

    // Checks if the passed Method meets the pre conditions in the world state
    public static bool MethodConditionMet(Method m, WorldState ws)
    {
        if (m.task == TASKS.RootTask)
        {
            switch (m.methodNum)
            {
                case 0: 
                    if ((ws.numTeleports > 0) && (ws.playerNear || ws.enemyNear))
                    {
                        return true;
                    }
                    return false;
                case 1:
                    if (ws.enemyNear)
                    {
                        return true;
                    }
                    return false;
                case 2:
                    return true;
            }
        }
        if(m.task == TASKS.Avoid)
        {
            switch(m.methodNum)
            {
                case 0:
                    if (ws.isHidden)
                    {
                        return true;
                    }
                    return false;
                case 1:
                    if (!ws.isHidden)
                    {
                        return true;
                    }
                    return false;
            }
        }
        return false;
    }
}

public class WorldState
{
    public int numTeleports;
    public bool enemyNear;
    public bool playerNear;
    public bool isHidden = true;

    public WorldState(int rt, bool e, bool p, bool h)
    {
        numTeleports = rt;
        enemyNear = e;
        playerNear = p;
        isHidden = h;
    }
    public void ApplyEffects(WorldState effects)    // i havent found a use for this yet
    {
        this.numTeleports = effects.numTeleports;
        this.enemyNear = effects.enemyNear;
        this.playerNear = effects.playerNear;
        this.isHidden = effects.isHidden;
    }
    public void setNumTeleports(int n)
    {
        numTeleports = n;
    }
    public void setEnemyNear(bool b)
    {
        enemyNear = b;
    }
    public void setPlayerNear(bool b)
    {
        playerNear = b;
    }
    public void setIsHidden(bool b)
    {
        isHidden = b;
    }
}
