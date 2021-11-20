using ChakraCore.Net.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ChakraCore.Net.Test.Scheduling
{
    public class TasksTest
    {
        public class NullJsScheduler : JsScheduler
        {
            public override bool CanExecuteInThisThread()
            {
                return true;
            }

            public override void EnterBreakState()
            {
                return;
            }

            public override void ExitBreakState()
            {
                return;
            }

            public override void QueueDebugTask(JsTask task)
            {
                //task.Run();
            }

            public override void QueueTask(JsTask task)
            {
                //task.Run();
            }
        }


        [Fact]
        public async Task CreateAsync()
        {
            var task = new JsTask(() => Console.WriteLine("Hi"));

            task.Priority.Should().Be(JsTaskPriority.LOWEST);
            task.State.Should().Be(JsTaskState.Initialized);
            Action wait = () => task.Wait();
            wait.Should().Throw<UnscheduledJsTaskException>();
            Func<Task> awaitAction = async () => await task;
            await awaitAction.Should().ThrowAsync<UnscheduledJsTaskException>();

            Action run = () => task.Run();
            task.IsCompleted.Should().BeFalse();
            run.Should().NotThrow();
            task.IsCompleted.Should().BeTrue();
            run.Should().Throw<InvalidJsTaskStateException>();
        }

        [Fact]
        public async void CreateGeneric()
        {
            var task = new JsTask<string>(() => "Hi");

            task.Priority.Should().Be(JsTaskPriority.LOWEST);
            task.State.Should().Be(JsTaskState.Initialized);
            Action wait = () => task.Wait();
            wait.Should().Throw<UnscheduledJsTaskException>();
            Func<Task> awaitAction = async () => await task;
            await awaitAction.Should().ThrowAsync<UnscheduledJsTaskException>();

            Action run = () => task.Run();
            task.GetResult().Should().Be(default);
            task.IsCompleted.Should().BeFalse();
            run.Should().NotThrow();
            task.IsCompleted.Should().BeTrue();
            task.GetResult().Should().Be("Hi");

            run.Should().Throw<InvalidJsTaskStateException>();

        }

        [Fact]
        public void Scheduled()
        {
            var s = new NullJsScheduler();

            var task = s.Run(() => Console.WriteLine("Hi"));

            Action start = () => task.Start(s);
            start.Should().Throw<InvalidJsTaskStateException>();

            task.Priority.Should().Be(JsTaskPriority.LOWEST);
            task.State.Should().Be(JsTaskState.Pending);
            Action wait = () => task.Wait();
            wait.Should().NotThrow();

            task.IsCompleted.Should().BeTrue();

            Action run = () => task.Run();
            run.Should().Throw<InvalidJsTaskStateException>();
        }

        [Fact]
        public void ScheduledGeneric()
        {
            var s = new NullJsScheduler();

            var task = s.Run(() => "Hi");

            Action start = () => task.Start(s);
            start.Should().Throw<InvalidJsTaskStateException>();

            task.Priority.Should().Be(JsTaskPriority.LOWEST);
            task.State.Should().Be(JsTaskState.Pending);
            task.GetResult().Should().Be(default);

            Action wait = () => task.Wait();
            wait.Should().NotThrow();

            task.IsCompleted.Should().BeTrue();
            task.GetResult().Should().Be("Hi");

            Action run = () => task.Run();
            run.Should().Throw<InvalidJsTaskStateException>();
        }
    }
}
