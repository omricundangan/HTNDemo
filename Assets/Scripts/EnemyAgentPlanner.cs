using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgentPlanner : MonoBehaviour {

    public static EnemyAgentController ctrlr;
    public WorldState CurrentWorldState;
    public Stack<Task> TasksToProcess;
    public Stack<Task> FinalPlan;

    public Task taskRoot;
    public Task taskUseTeleport;
    public Task taskIdle;
    public Task taskHide;
    public Task taskAvoid;
    public Task taskGetItem;

    public bool planning;

	void Start () {
        planning = false;
        ctrlr = GetComponent<EnemyAgentController>();
        TasksToProcess = new Stack<Task>();
        FinalPlan = new Stack<Task>();
        CurrentWorldState = new WorldState(2, false, false, false);

        CreateHTN();
	}

    void CreateHTN()
    {
        // Create HTN
        taskRoot = new Task(TASKS.RootTask, TYPE.Compound);
        taskUseTeleport = new Task(TASKS.Teleport, TYPE.Primitive);
 //      taskUseTeleport.effects = new WorldState(CurrentWorldState.numTeleports--, CurrentWorldState.enemyNear, CurrentWorldState.playerNear, CurrentWorldState.isHidden);
        taskAvoid = new Task(TASKS.Avoid, TYPE.Compound);
        taskIdle = new Task(TASKS.Idle, TYPE.Primitive);
        taskHide = new Task(TASKS.Hide, TYPE.Primitive);
        taskGetItem = new Task(TASKS.GetItem, TYPE.Primitive);

        // Construct the methods in the RootTask (BeEnemyAgent)
        Method rootm0 = new Method(TASKS.RootTask, 0);
        rootm0.subTasks.Add(taskUseTeleport);
        Method rootm1 = new Method(TASKS.RootTask, 1);
        rootm1.subTasks.Add(taskAvoid);
        Method rootm2 = new Method(TASKS.RootTask, 2);
        rootm2.subTasks.Add(taskGetItem);

        // Add the methods to the methods in the Compound Task "BeEnemyAgent"
        taskRoot.methods.Add(rootm0);
        taskRoot.methods.Add(rootm1);
        taskRoot.methods.Add(rootm2);

        // Construct the methods in the Compound Task "Avoid"
        Method avoidm0 = new Method(TASKS.Avoid, 0);
        avoidm0.subTasks.Add(taskIdle);
        Method avoidm1 = new Method(TASKS.Avoid, 1);
        avoidm1.subTasks.Add(taskHide);

        // Add the methods to the Compound Task
        taskAvoid.methods.Add(avoidm0);
        taskAvoid.methods.Add(avoidm1);
    }

    public void MakePlan()
    {
        WorldState WorkingWS = CurrentWorldState;
        TasksToProcess.Push(taskRoot);
        while(TasksToProcess.Count != 0)
        {
            Task CurrentTask = TasksToProcess.Pop();
            if(CurrentTask.type == TYPE.Compound)
            {
                Method SatisfiedMethod = CurrentTask.FindSatisfiedMethod(WorkingWS);
                if (SatisfiedMethod != null)
                {
                    // RecordDecompositionOfTask(CurrentTask, FinalPlan, decompHistory);

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
                else
                {
                    // RestoreToLastDecomposedTask();
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
                else
                {
                    //RestoreToLastDecomposedTask();
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
                else
                {
                    return false;
                }
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

public class Task
{
    public List<Method> methods = new List<Method>();   // null if primitive Task
    public TASKS task;  // name of task
    public TYPE type;   // type of task (primitive/compound)
    public WorldState effects;  // the effects of this task, if any

    public Task(TASKS ta, TYPE ty)
    {
        task = ta;
        type = ty;
    }

    // Returns the Method in this Task that fulfills the preconditions in the worldstate, if any
    public Method FindSatisfiedMethod(WorldState ws)
    {
        foreach (Method method in methods)
        {
            if (EnemyAgentPlanner.MethodConditionMet(method, ws))   // only true if a method satisfies the preconditions in the worldstate
            {
                return method;
            }
        }
        return null;
    }
}

// Compound Tasks contain Methods, which contain SubTasks
public class Method
{
    public List<Task> subTasks = new List<Task>();  // methods hold subTasks that are either primtive or compound
    public int methodNum;   // the number of this Method
    public TASKS task;

    public Method(TASKS t, int m)
    {
        task = t;
        methodNum = m;
    }
}

// List of tasks available
public enum TASKS
{
    RootTask,
    Teleport,
    Avoid,
    Hide,
    GetItem,
    Idle
}

// Type of Tasks available
public enum TYPE
{
    Compound,
    Primitive
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
