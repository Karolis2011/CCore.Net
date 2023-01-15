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
            // Initialize single thread scheduler
            using var s = new JsPFIFOScheduler();

            var e1 = new ManualResetEvent(false);
            var e2 = new ManualResetEvent(false);
            var e3 = new ManualResetEvent(false);
            var e4 = new ManualResetEvent(false);
            var et1 = new ManualResetEvent(false);
            
            var counter = 0;
            
            // Occupy thread with some work 
            var t1 = s.Run(() => { et1.Set(); e1.WaitOne(); outputHelper.WriteLine("T1 has finished."); counter++; }).WithName("T1");
            // Wait for T1 to start before scheduling other tasks. 
            et1.WaitOne();
            var t2 = s.Run(() => { e2.WaitOne(); outputHelper.WriteLine("T2 has finished."); counter++; }).WithName("T2");
            var t3 = s.Run(() => { e3.WaitOne(); outputHelper.WriteLine("T3 has finished."); counter++; }, JsTaskPriority.INITIALIZATION).WithName("T3");
            var t4 = s.Run(() => { e4.WaitOne(); outputHelper.WriteLine("T4 has finished."); counter++; }, JsTaskPriority.INITIALIZATION).WithName("T4");

            t1.State.Should().Be(JsTaskState.Running);
            t2.State.Should().Be(JsTaskState.Pending);
            t3.State.Should().Be(JsTaskState.Pending);
            t4.State.Should().Be(JsTaskState.Pending);
            counter.Should().Be(0);
            
            e1.Set();
            t1.Wait();
            t1.State.Should().Be(JsTaskState.Complete);
            t2.State.Should().Be(JsTaskState.Pending);
            t3.State.Should().Be(JsTaskState.Running);
            t4.State.Should().Be(JsTaskState.Pending);
            counter.Should().Be(1);

            e3.Set();
            t3.Wait();
            t1.State.Should().Be(JsTaskState.Complete);
            t2.State.Should().Be(JsTaskState.Pending);
            t3.State.Should().Be(JsTaskState.Complete);
            t4.State.Should().Be(JsTaskState.Running);
            counter.Should().Be(2);

            e4.Set();
            t4.Wait();
            t1.State.Should().Be(JsTaskState.Complete);
            t2.State.Should().Be(JsTaskState.Running);
            t3.State.Should().Be(JsTaskState.Complete);
            t4.State.Should().Be(JsTaskState.Complete);
            counter.Should().Be(3);
            
            e2.Set();
            t2.Wait();
            t1.State.Should().Be(JsTaskState.Complete);
            t2.State.Should().Be(JsTaskState.Complete);
            t3.State.Should().Be(JsTaskState.Complete);
            t4.State.Should().Be(JsTaskState.Complete);
            counter.Should().Be(4);
        }
        
    }
}
