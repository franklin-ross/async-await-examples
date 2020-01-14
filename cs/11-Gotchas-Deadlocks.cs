using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitExamples
{
    public class DeadlockTests
    {
        private static BasicSynchronizationContext UseSynchronizationContext()
        {
            var syncCtx = new BasicSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncCtx);
            return syncCtx;
        }

        [Test]
        public void WaitCanDeadlock() // task.Result is worse since you can't give a timeout.
        {
            using var context = UseSynchronizationContext();

            async Task DoWorkAsyncAndMarshalBack()
            {
                await Task.Delay(1);
                context.Send(_ =>
                {
                    // The body of this wants to run on the synchronisation context, and will block
                    // until it does so. Since Deadlock() is already running on the synchronisation
                    // context and waiting for this to finish, they're in a deadlocked state.
                }, null);
            }

            async Task Deadlock()
            {
                await Task.Delay(1);
                if (!DoWorkAsyncAndMarshalBack().Wait(500))
                    throw new Exception("Deadlock!");
            }

            Assert.That(Deadlock,
                Throws.InstanceOf<Exception>()
                    .With.Message.EqualTo("Deadlock!"));
        }
    }
}