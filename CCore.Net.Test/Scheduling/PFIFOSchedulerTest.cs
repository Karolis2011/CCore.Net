using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CCore.Net.Test.Scheduling
{
    public class PFIFOSchedulerTest
    {
        private readonly ITestOutputHelper outputHelper;
        public PFIFOSchedulerTest(ITestOutputHelper testOutputHelper)
        {
            outputHelper = testOutputHelper;
        }


        [Fact]
        public void CreateAndDispose()
        {
            var s = new JsPFIFOScheduler();
            s.CanExecuteInThisThread().Should().BeFalse();
            var t1 = s.Run(() => s.CanExecuteInThisThread());
            t1.Wait();
            t1.GetResult().Should().BeTrue();

            Action dispose = () => s.Dispose();
            dispose.Should().NotThrow();

            Action insanity = () => s.Run(() => Console.WriteLine("This won't work"));

            insanity.Should().Throw<ObjectDisposedException>();
        }

        [Fact]
        public void PriorityAndOrder()
        {
            using var s = new JsPFIFOScheduler();

            // Ocupy thread with some work 
            var t1 = s.Run(() => { Thread.Sleep(30); outputHelper.WriteLine("T1 has finished."); });
            var t2 = s.Run(() => { Thread.Sleep(10); outputHelper.WriteLine("T2 has finished."); });
            Thread.Sleep(1);
            var t3 = s.Run(() => { Thread.Sleep(10); outputHelper.WriteLine("T3 has finished."); }, JsTaskPriority.INITIALIZATION);
            var t4 = s.Run(() => { Thread.Sleep(30); outputHelper.WriteLine("T4 has finished."); }, JsTaskPriority.INITIALIZATION);

            t1.IsCompleted.Should().BeFalse();
            t2.IsCompleted.Should().BeFalse();
            t3.IsCompleted.Should().BeFalse();
            t4.IsCompleted.Should().BeFalse();

            t3.Wait();
            t1.IsCompleted.Should().BeTrue();
            t2.IsCompleted.Should().BeFalse();
            t3.IsCompleted.Should().BeTrue();
            t4.IsCompleted.Should().BeFalse();

            t2.Wait();
            t2.IsCompleted.Should().BeTrue();
            t4.IsCompleted.Should().BeTrue();

        }
    }
}
