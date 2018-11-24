using System.Collections.Generic;

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