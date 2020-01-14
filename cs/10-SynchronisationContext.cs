using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitExamples
{
    public class SynchronisationContextTests
    {
        private static BasicSynchronizationContext UseSynchronizationContext()
        {
            var syncCtx = new BasicSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncCtx);
            return syncCtx;
        }

        [Test]
        public async Task SynchronizationContextFlowsThroughAwaits()
        {
            using var context = UseSynchronizationContext();

            await Task.Delay(1);

            Assert.That(SynchronizationContext.Current, Is.EqualTo(context));
        }

        [Test]
        public async Task SynchronizationContextIsLostOnConfigureAwaitFalse()
        {
            using var context = UseSynchronizationContext();

            await Task.Delay(1).ConfigureAwait(false);

            Assert.That(SynchronizationContext.Current, Is.EqualTo(null));
        }

        [Test]
        public async Task SynchronizationContextFlowsThroughNestedAwaits()
        {
            using var context = UseSynchronizationContext();

            async Task NestedConfigureAwaitFalse()
            {   // We lose the context here because of the .ConfigureAwait(false), but the outer
                // await still has it captured. This is common for library code which doesn't
                // have any thread affinity, without effecting the context of the caller.
                await Task.Delay(1).ConfigureAwait(false);
                Assert.That(SynchronizationContext.Current, Is.EqualTo(null));
            }

            await NestedConfigureAwaitFalse();

            Assert.That(SynchronizationContext.Current, Is.EqualTo(context));
        }

        [Test]
        public async Task SynchronizationContextDoesNotFlowIntoTaskRun()
        {
            using var context = UseSynchronizationContext();

            await Task.Run(() =>
            {
                // The synchronisation context is not captured by Task.Run(). That means work done
                // here is likely to be run on another thread. If this wasn't the case, imagine
                // calling Task.Run() from the UI thread, assuming some heavy computation would be
                // done on the thread pool, only to find the synchronisation context was captured
                // and the work is instead being done on the UI thread. That would make you unhappy.
                // Many functions internal to the BCL lose the context like this. You don't
                // generally have to think too hard about this, since you can be sure that your
                // code, at least, will be marshalled back to it's original synchronisation context
                // if you're using async/await.
                Assert.That(SynchronizationContext.Current, Is.EqualTo(null));
            });

            Assert.That(SynchronizationContext.Current, Is.EqualTo(context));
        }

        // [Test]
        // public async Task SynchronizationContextDoesNotFlowIntoTask()
        // {
        //     using var context = UseSynchronizationContext();

        //     await Task.Delay(1).ContinueWith(task =>
        //     {
        //         // The synchronisation context is not captured by Task.Run(). That means work done
        //         // here is likely to be run on another thread. If this wasn't the case, imagine
        //         // calling Task.Run() from the UI thread, assuming some heavy computation would be
        //         // done on the thread pool, only to find the synchronisation context was captured
        //         // and the work is instead being done on the UI thread. That would make you unhappy.
        //         // Many functions internal to the BCL lose the context like this. You don't
        //         // generally have to think too hard about this, since you can be sure that your
        //         // code, at least, will be marshalled back to it's original synchronisation context
        //         // if you're using async/await.
        //         Assert.That(SynchronizationContext.Current, Is.EqualTo(null));
        //     });

        //     Assert.That(SynchronizationContext.Current, Is.EqualTo(context));
        // }
    }
}