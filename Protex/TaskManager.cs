using System;
using System.Collections.Generic;
using ProtexCore.Config;
using ProtexCore.Runner;
using ProtexCore.Tasks;

namespace ProtexCore
{
    internal delegate RunnerCommandResult TaskPerformDelegate(IRunner runner);

    public sealed class TaskManager
    {
        private IRunner runner;
        private Dictionary<ProtexTask, TaskPerformDelegate> pendingTasks;
        private const int MaxTasks = 3;

        public TaskManager(RemoteConfig remoteConfig)
        {
            runner = new RemoteRunner(remoteConfig);
            pendingTasks = new Dictionary<ProtexTask, TaskPerformDelegate>();
        }

        public TaskManager(LocalConfig localConfig)
        {
            runner = new LocalRunner(localConfig);
            pendingTasks = new Dictionary<ProtexTask, TaskPerformDelegate>();
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
            lock (pendingTasks)
            {
                if (pendingTasks.Count >= MaxTasks)
                    return ManagerResult.CapacityLimit;

                if (pendingTasks.ContainsKey(task))
                    return ManagerResult.Pending;

                TaskPerformDelegate performDelegate = new TaskPerformDelegate(task.Perform);
                task.IsActive = true;
                task.IsFinished = false;

                // if queue capacity is ok
                // than add new task
                pendingTasks.Add(task, performDelegate);

                // create instance of RemoteRunner/LocalRunner
                // depending on TaskManager inner Runner
                // (can be changed if future to make system
                // more flexible - runner can be passed to 
                // parameter of AddTask() method)
                performDelegate.BeginInvoke(
                    (IRunner) Activator.CreateInstance(runner.GetType(), runner),
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

                        lock (pendingTasks)
                        {
                            // remove task from dictionary
                            pendingTasks.Remove(task);
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