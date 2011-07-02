using System;
using System.Collections.Generic;
using System.Threading;

namespace ProtexCore
{
    internal delegate RunnerCommandResult TaskPerformDelegate(IRunner runner);
    public enum ManagerResult { Pending, CapacityLimit, Fail, Queued }

    public sealed class TaskManager
    {
        private IRunner runner;
        private Dictionary<ProtexTask, TaskPerformDelegate> pendingTasks;
        private const int MAX_TASKS = 3;

        public TaskManager(RemoteConfig remoteConfig)
        {
            this.runner = new RemoteRunner(remoteConfig);
            this.pendingTasks = new Dictionary<ProtexTask, TaskPerformDelegate>();
        }

        public TaskManager(LocalConfig localConfig)
        {
            this.runner = new LocalRunner(localConfig);
            this.pendingTasks = new Dictionary<ProtexTask, TaskPerformDelegate>();
        }

        /// <summary>
        /// Adds a task to queue and starts it immedialy.
        /// If queue is full, rejects a task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public ManagerResult AddTask(ProtexTask task)
        {
            // if AddTask will be called 
            // from different threads, than
            // elementary object is task list
            lock (this.pendingTasks)
            {
                if (this.pendingTasks.Count >= MAX_TASKS)
                    return ManagerResult.CapacityLimit;

                if (this.pendingTasks.ContainsKey(task))
                    return ManagerResult.Pending;

                TaskPerformDelegate performDelegate = new TaskPerformDelegate(task.Perform);
                task.IsActive = true;
                task.IsFinished = false;

                // if queue capacity is ok
                // than add new task
                this.pendingTasks.Add(task, performDelegate);

                // create instance of RemoteRunner/LocalRunner
                // depending on TaskManager inner Runner
                // (can be changed if future to make system
                // more flexible - runner can be passed to 
                // parameter of AddTask() method)
                performDelegate.BeginInvoke(
                    (IRunner)Activator.CreateInstance(this.runner.GetType(), this.runner),
                    // use closure to get task reference
                    delegate(IAsyncResult result)
                    {
                        if (result == null)
                            throw new ArgumentNullException();

                        TaskPerformDelegate taskPerformDelegate = result.AsyncState as TaskPerformDelegate;
                        // some paranoidal checks...
                        if (taskPerformDelegate == null)
                            throw new Exception("Delegate cannot be null. Fatal error.");

                        RunnerCommandResult commandResult = taskPerformDelegate.EndInvoke(result);

                        lock (this.pendingTasks)
                        {
                            // remove task from dictionary
                            this.pendingTasks.Remove(task);
                            task.OnTaskCompleted(commandResult);
                        }
                    },
                    null);

                return ManagerResult.Queued;
            }
        }

        public bool KillTask(ProtexTask task)
        {
            throw new NotImplementedException();
        }
    }
}

