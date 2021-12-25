using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CCore.Net
{
    /// <summary>
    /// Pririotized First In First Out scheduler.
    /// </summary>
    public class JsPFIFOScheduler : JsScheduler, IDisposable
    {
        private object aliveLock = new object();
        private bool alive = true;
        private Thread thread;
        private JsTask currentlyExecuting = null;
        private LinkedList<JsTask> tasks = new LinkedList<JsTask>();
        private LinkedList<JsTask> debugTasks = new LinkedList<JsTask>();
        private AutoResetEvent newTaskEvent = new AutoResetEvent(false);
        private AutoResetEvent newDebugTaskEvent = new AutoResetEvent(false);
        private object inBreakLock = new object();
        private bool inBreak = false;


        public JsPFIFOScheduler()
        {
            thread = new Thread(ExecutionThread);
            thread.Name = nameof(JsPFIFOScheduler);
            thread.IsBackground = true;
            thread.Start();
        }

        private JsTask FirstToExecute(LinkedList<JsTask> tasks)
        {
            LinkedListNode<JsTask> lowest = tasks.First;
            if (lowest == null)
                return null;
            if(tasks.Last == lowest)
            {
                tasks.RemoveFirst();
                return lowest.Value;
            }
            for (var node = tasks.First; node != null; node = node.Next)
                if (lowest != null && lowest.Value.Priority > node.Value.Priority)
                    lowest = node;
            var value = lowest?.Value;
            if (lowest != null)
                tasks.Remove(lowest);
            return value;
        }

        public override void QueueTask(JsTask task)
        {
            lock (aliveLock)
                if (!alive)
                    throw new ObjectDisposedException(nameof(JsPFIFOScheduler));
            newTaskEvent.Reset();
            lock (tasks)
                _ = tasks.AddLast(task);
            newTaskEvent.Set();
        }

        public override void QueueDebugTask(JsTask task)
        {
            lock (aliveLock)
                if (!alive)
                    throw new ObjectDisposedException(nameof(JsPFIFOScheduler));
            lock (inBreakLock)
                if (!inBreak)
                    throw new Exception("Can't queue a debug task on to scheduler that's not in break state. That won't execute.");
            newDebugTaskEvent.Reset();
            lock (debugTasks)
                _ = debugTasks.AddLast(task);
            newDebugTaskEvent.Set();
        }

        private void ExecutionThread()
        {
            bool shouldStayAlive = true;
            while (shouldStayAlive)
            {
                if (currentlyExecuting == null)
                    lock (tasks)
                        currentlyExecuting = FirstToExecute(tasks);

                if (currentlyExecuting != null)
                {
                    if (currentlyExecuting.State == JsTaskState.Pending)
                        currentlyExecuting.Run();
                    currentlyExecuting = null;
                }
                else
                {
                    if (shouldStayAlive)
                        newTaskEvent.WaitOne();
                }
                lock (aliveLock)
                    shouldStayAlive = alive;
            }
        }

        public override void EnterBreakState()
        {
            if (Thread.CurrentThread != thread)
                throw new InvalidOperationException("Current thread is not same as scheduler's thread.");
            bool breakState = true;
            lock (inBreakLock)
                inBreak = true;

            currentlyExecuting.OnBreak();
            JsTask currentlyExecutingDebug = null;
            while (breakState)
            {
                lock (debugTasks)
                    currentlyExecutingDebug = FirstToExecute(debugTasks);

                if (currentlyExecutingDebug != null)
                {
                    if (currentlyExecutingDebug.State == JsTaskState.Pending)
                        currentlyExecutingDebug.Run();
                    currentlyExecutingDebug = null;
                }
                else
                {
                    newDebugTaskEvent.WaitOne();
                }

                lock (inBreakLock)
                    breakState = inBreak;
            }
            currentlyExecuting.OnResume();
        }

        public override void ExitBreakState()
        {
            newDebugTaskEvent.Reset();
            lock (inBreakLock)
                inBreak = false;
            newDebugTaskEvent.Set();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        virtual protected void Dispose(bool full)
        {
            if (full)
            {
                lock (inBreakLock)
                    inBreak = false;
                lock (aliveLock)
                    alive = false;
                newTaskEvent.Reset();
                newDebugTaskEvent.Reset();
                Thread.SpinWait(1);
                newTaskEvent.Set();
                newDebugTaskEvent.Set();
            }

        }

        public override bool CanExecuteInThisThread() => Thread.CurrentThread == thread;

        public override JsTask GetCurrentTask()
        {
            if (CanExecuteInThisThread())
                return currentlyExecuting;
            return null;
        }
    }
}
