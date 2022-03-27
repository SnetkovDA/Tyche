using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tyche.Scanner.Workers
{
    public class TaskWorker
    {
        public Dictionary<string, Task> QueuedTasks { get; set; } = new Dictionary<string, Task>();

        public string AddTask(WebWorker webWorker, Action executedAction, string scannerId)
        {
            string taskId = Guid.NewGuid().ToString();
            QueuedTasks.Add(taskId, Task.Factory.StartNew(executedAction).ContinueWith(t => TaskEnd(webWorker, t, taskId, scannerId)));
            return taskId;
        }

        void TaskEnd(WebWorker webWorker, Task task, object taskId, string scannerId)
        {
            QueuedTasks.Remove(taskId.ToString());
            task.Dispose();
            webWorker.SendRequest(System.Net.Http.HttpMethod.Post, $"Scanner/End/{scannerId}", "");
        }
    }
}
