public class EnemyAgentHTN{

    public Task taskRoot;
    public Task taskUseTeleport;
    public Task taskIdle;
    public Task taskHide;
    public Task taskAvoid;
    public Task taskGetItem;

    public EnemyAgentHTN()
    {
        // Create HTN
        taskRoot = new Task(TASKS.RootTask, TYPE.Compound);
        taskUseTeleport = new Task(TASKS.Teleport, TYPE.Primitive);
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
}
