using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net
{
    /// <summary>
    /// Base class for scheduling JsTask
    /// </summary>
    public abstract class JsScheduler
    {
        /// <summary>
        /// Internal use only. Enter break state processing on current thread, make this method return to exit break state.
        /// </summary>
        public abstract void EnterBreakState();

        /// <summary>
        /// Used to signal to scheduler in Break state to exit break state. 
        /// </summary>
        public abstract void ExitBreakState();

        /// <summary>
        /// Add task to be scheduled. Should not be used directly. Use <see cref="JsTask.Start(JsScheduler)"/> and <see cref="JsTask{TResult}.Start(JsScheduler)"/>
        /// </summary>
        /// <param name="task">Task to schedule</param>
        public abstract void QueueTask(JsTask task);

        /// <summary>
        /// Add task to be scheduled in debug queue. Should not be used directly. Use <see cref="JsTask.Start(JsScheduler)"/> and <see cref="JsTask{TResult}.Start(JsScheduler)"/>
        /// </summary>
        /// <param name="task">Task to schedule</param>
        /// <exception cref="Exception">Thrown when task is attempted to be scheduled when not in break state.</exception>
        public abstract void QueueDebugTask(JsTask task);

        /// <summary>
        /// Determines if task can be executed on this thread.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanExecuteInThisThread();

        /// <summary>
        /// Create and start new task on this scheduler
        /// </summary>
        /// <typeparam name="TResult">Return type of task</typeparam>
        /// <param name="func">Function to execute</param>
        /// <param name="priority">Priority</param>
        /// <returns>Newly created task.</returns>
        public virtual JsTask<TResult> Run<TResult>(Func<TResult> func, JsTaskPriority priority = JsTaskPriority.LOWEST) =>
            new JsTask<TResult>(func, priority).Start(this);
        /// <summary>
        /// Create and start new task on this scheduler
        /// </summary>
        /// <param name="action">Function to execute</param>
        /// <param name="priority">Priority</param>
        /// <returns>Newly created task.</returns>
        public virtual JsTask Run(Action action, JsTaskPriority priority = JsTaskPriority.LOWEST) => new JsTask(action, priority).Start(this);

        /// <summary>
        /// Create and start new timed task on this scheduler
        /// </summary>
        /// <typeparam name="TResult">Return type of task</typeparam>
        /// <param name="func">Function to execute</param>
        /// <param name="onTimeout">Action to perform on timeout</param>
        /// <param name="timeout">Timeout time</param>
        /// <param name="priority">Priority</param>
        /// <returns></returns>
        public virtual JsTask<TResult> RunTimed<TResult>(Func<TResult> func, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST) =>
                    new JsTaskTimed<TResult>(func, onTimeout, timeout, priority).Start(this);

        /// <summary>
        /// Create and start new timed task on this scheduler
        /// </summary>
        /// <param name="action">action to execute</param>
        /// <param name="onTimeout">Action to perform on timeout</param>
        /// <param name="timeout">Timeout time</param>
        /// <param name="priority">Priority</param>
        /// <returns></returns>
        public virtual JsTask RunTimed(Action action, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST) =>
            new JsTaskTimed(action, onTimeout, timeout, priority).Start(this);


        public virtual JsTask<TResult> RunDebug<TResult>(Func<TResult> func, JsTaskPriority priority = JsTaskPriority.LOWEST) =>
            new JsTask<TResult>(func, priority).StartDebug(this);
        public virtual JsTask RunDebug(Action action, JsTaskPriority priority = JsTaskPriority.LOWEST) =>
            new JsTask(action, priority).StartDebug(this);

        public virtual JsTask<TResult> RunDebugTimed<TResult>(Func<TResult> func, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST) =>
            new JsTaskTimed<TResult>(func, onTimeout, timeout, priority).StartDebug(this);
        public virtual JsTask RunDebugTimed(Action action, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST) =>
            new JsTaskTimed(action, onTimeout, timeout, priority).StartDebug(this);

    }
}
