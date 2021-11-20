using ChakraCore.Net.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ChakraCore.Net
{
    /// <summary>
    /// Pririority of task execution.
    /// </summary>
    public enum JsTaskPriority
    {
        INITIALIZATION,
        EXECUTION,
        CALLBACK,
        PROMISE,
        LOWEST,
    }

    /// <summary>
    /// Current state of JsTask
    /// </summary>
    public enum JsTaskState
    {
        Initialized,
        Pending,
        Running,
        Complete,
        Failed
    }

    /// <summary>
    /// Base class representing of group of execution commands do be performed inside JsContext
    /// </summary>
    public class JsTask
    {
        /// <summary>
        /// Is this task finished with work.
        /// </summary>
        public bool IsCompleted => State >= JsTaskState.Complete;
        /// <summary>
        /// This task's current priority. To be used in scheduler determining order tasks should be executed in.
        /// </summary>
        public JsTaskPriority Priority { get; protected set; }
        /// <summary>
        /// Current state of the task
        /// </summary>
        public JsTaskState State { get; protected set; } = JsTaskState.Initialized;

        private Action m_action;
        protected JsScheduler assignedScheduler;
        protected LinkedList<Action> finishedCallbacks = new LinkedList<Action>();

        protected JsTask() { }

        /// <summary>
        /// Constructs task without assigning it to a scheduler
        /// </summary>
        /// <param name="function">Function to be executed with this task</param>
        /// <param name="priority">Pririority of this task</param>
        public JsTask(Action function, JsTaskPriority priority = JsTaskPriority.LOWEST)
        {
            m_action = function;
            Priority = priority;
        }

        /// <summary>
        /// Executes this task in this thread. This method should be invoked from single thread.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public virtual void Run()
        {
            if (IsCompleted)
            {
                throw new InvalidJsTaskStateException("Task is already complete.");
            }
            State = JsTaskState.Running;
            try
            {
                m_action();
            }
            catch (Exception)
            {
                State = JsTaskState.Failed;
                throw;
            }
            finally
            {
                State = JsTaskState.Complete;
            }
            foreach (Action callback in finishedCallbacks)
            {
                callback();
            }
            finishedCallbacks.Clear();
        }

        /// <summary>
        /// Schedules this task to be executed in scheduler.
        /// </summary>
        /// <param name="scheduler">Scheduler responsible for executing this task.</param>
        /// <returns>Task for method chaining</returns>
        public JsTask Start(JsScheduler scheduler)
        {
            if (State != JsTaskState.Initialized || assignedScheduler != null)
                throw new InvalidJsTaskStateException("Task is already scheduled for execution.");
            scheduler.QueueTask(this);
            assignedScheduler = scheduler;
            State = JsTaskState.Pending;
            return this;
        }

        /// <summary>
        /// Schedules this task to be executed in scheduler's debug queue.
        /// </summary>
        /// <param name="scheduler">Scheduler responsible for executing this task.</param>
        /// <returns>Task for method chaining</returns>
        public JsTask StartDebug(JsScheduler scheduler)
        {
            if (State != JsTaskState.Initialized || assignedScheduler != null)
                throw new InvalidJsTaskStateException("Task is already scheduled for execution.");
            scheduler.QueueDebugTask(this);
            assignedScheduler = scheduler;
            State = JsTaskState.Pending;
            return this;
        }

        /// <summary>
        /// Gets task's awaiter. Used in async programming.
        /// </summary>
        /// <returns></returns>
        public JsTaskAwaiter GetAwaiter()
        {
            if (assignedScheduler == null)
                throw new UnscheduledJsTaskException("Attempted to await unscheduled task.");
            return new JsTaskAwaiter(this);
        }

        /// <summary>
        /// Generalize JsTask to <see cref="System.Threading.Tasks.Task"/>
        /// </summary>
        /// <returns>Task representing this JsTask</returns>
        public async System.Threading.Tasks.Task ToTask()
        {
            await this;
        }

        /// <summary>
        /// Registers a callback to be called when task execution is over and sucessfull.
        /// </summary>
        /// <param name="action">Action to execute on completion</param>
        public void RegisterOnCompleteCallback(Action action)
        {
            finishedCallbacks.AddLast(action);
        }

        /// <summary>
        /// Wait for task to finish execution with provided timeout for waiting.
        /// </summary>
        /// <param name="timeout">Time in witch execution is not finished, stop waiting and return false</param>
        /// <returns>Value determining whenewer it waited till task execution completion or halted waiting.</returns>
        public bool Wait(TimeSpan timeout)
        {
            if (assignedScheduler == null)
                throw new UnscheduledJsTaskException();
            if (!IsCompleted && assignedScheduler.CanExecuteInThisThread())
                Run();
            else if (!IsCompleted)
                return SpinWait.SpinUntil(() => IsCompleted, timeout);
            return true;
        }

        /// <summary>
        /// Wait for current task to finish execution.
        /// </summary
        public void Wait() {
            if (assignedScheduler == null)
                throw new UnscheduledJsTaskException();
            if (!IsCompleted && assignedScheduler.CanExecuteInThisThread())
                Run();
            else if (!IsCompleted)
                SpinWait.SpinUntil(() => IsCompleted);
        }

        /// <summary>
        /// Wait for task to finish execution with provided timeout for waiting.
        /// </summary>
        /// <param name="timeout">Timeout in ms</param>
        /// <returns>Value determining whenewer it waited till task execution completion or halted waiting.</returns>
        public bool Wait(int timeout) => Wait(new TimeSpan(TimeSpan.TicksPerMillisecond * timeout));

        public virtual void OnBreak() { }
        public virtual void OnResume() { }


        public static explicit operator System.Threading.Tasks.Task(JsTask jsTask) => jsTask.ToTask();
    }

    /// <summary>
    /// Generic Task with return value
    /// </summary>
    /// <typeparam name="TResult">Type of return value</typeparam>
    public class JsTask<TResult> : JsTask
    {
        Delegate m_action;
        TResult result = default;

        /// <summary>
        /// Constructs task without assigning it to a scheduler
        /// </summary>
        /// <param name="function">Function to be executed with this task</param>
        /// <param name="priority">Pririority of this task</param>
        public JsTask(Func<TResult> function, JsTaskPriority priority = JsTaskPriority.LOWEST)
        {
            m_action = function;
            Priority = priority;
        }

        /// <inheritdoc/>
        public override void Run()
        {
            if (IsCompleted)
            {
                throw new InvalidJsTaskStateException("Task is already complete.");
            }
            State = JsTaskState.Running;
            try
            {
                if (m_action is Action action)
                {
                    action();
                }
                else if (m_action is Func<TResult> func)
                {
                    result = func();
                }
                else
                {
                    throw new InvalidJsTaskStateException("Action type is not supported.");
                }
            }
            catch (Exception)
            {
                State = JsTaskState.Failed;
                throw;
            }
            finally
            {
                State = JsTaskState.Complete;
            }
            foreach (Action callback in finishedCallbacks)
            {
                callback();
            }
            finishedCallbacks.Clear();
        }

        /// <summary>
        /// Schedules this task to be executed in scheduler.
        /// </summary>
        /// <param name="scheduler">Scheduler responsible for executing this task.</param>
        /// <returns>Task for method chaining</returns>
        public new JsTask<TResult> Start(JsScheduler scheduler)
        {
            if (State != JsTaskState.Initialized || assignedScheduler != null)
                throw new InvalidJsTaskStateException("Task is already scheduled for execution.");
            scheduler.QueueTask(this);
            assignedScheduler = scheduler;
            State = JsTaskState.Pending;
            return this;
        }

        /// <summary>
        /// Schedules this task to be executed in scheduler's debug queue.
        /// </summary>
        /// <param name="scheduler">Scheduler responsible for executing this task.</param>
        /// <returns>Task for method chaining</returns>
        public new JsTask<TResult> StartDebug(JsScheduler scheduler)
        {
            if (State != JsTaskState.Initialized || assignedScheduler != null)
                throw new InvalidJsTaskStateException("Task is already scheduled for execution.");
            scheduler.QueueDebugTask(this);
            assignedScheduler = scheduler;
            State = JsTaskState.Pending;
            return this;
        }

        /// <summary>
        /// Gets task's awaiter. Used in async programming.
        /// </summary>
        /// <returns></returns>
        public new JsTaskAwaiter<TResult> GetAwaiter()
        {
            if (assignedScheduler == null)
                throw new UnscheduledJsTaskException("Attempted to await unscheduled task.");
            return new JsTaskAwaiter<TResult>(this);
        }

        /// <summary>
        /// Generalize JsTask to <see cref="System.Threading.Tasks.Task{TResult}"/>
        /// </summary>
        /// <returns>Task representing this JsTask</returns>
        public new async System.Threading.Tasks.Task<TResult> ToTask()
        {
            return await this;
        }

        /// <summary>
        /// Gets execution result
        /// </summary>
        /// <returns></returns>
        public TResult GetResult()
        {
            return result;
        }

        public static explicit operator System.Threading.Tasks.Task<TResult>(JsTask<TResult> jsTask) => jsTask.ToTask();
    }

    public class JsTaskAwaiter : INotifyCompletion
    {
        protected JsTask _task;

        public bool IsCompleted => _task.IsCompleted;

        protected JsTaskAwaiter() { }
        public JsTaskAwaiter(JsTask task)
        {
            _task = task;
        }

        public void OnCompleted(Action continuation) => _task.RegisterOnCompleteCallback(continuation);

        public object GetResult() => null;
    }

    public class JsTaskAwaiter<TResult> : JsTaskAwaiter
    {
        protected JsTask<TResult> _ttask;


        public JsTaskAwaiter(JsTask<TResult> task)
        {
            _task = task;
            _ttask = task;
        }

        public new TResult GetResult() => _ttask.GetResult();
    }
}
