using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitExamples
{
    /// <summary>A simple synchronisation context which uses a single thread to run one piece of
    /// work at a time. Work is processed FIFO.</summary>
    /// <remarks>This should not be used for anything more than instruction. Don't go using this
    /// in a production project as it might break everything and steal your babies.</remarks>
    public sealed class BasicSynchronizationContext : SynchronizationContext, IDisposable
    {
        private readonly bool _ownsQueue;
        private readonly BlockingCollection<(SendOrPostCallback d, object? state)> _queue;
        private readonly CancellationTokenSource _disposing;

        /// <summary>Creates a new synchronisation context which runs work on a dedicated thread.
        /// </summary>
        public BasicSynchronizationContext()
        {
            _ownsQueue = true;
            _queue = new BlockingCollection<(SendOrPostCallback d, object? state)>();
            _disposing = new CancellationTokenSource();
            Task.Run(MessagePump);
        }

        /// <summary>Creates a new synchronisation context which forwards work to another instance.
        /// The new context doesn't own the master, and dispose on the slave is a no-op.</summary>
        private BasicSynchronizationContext(BasicSynchronizationContext master)
        {
            _ownsQueue = false;
            _queue = master._queue;
            _disposing = master._disposing;
        }

        /// <summary>The "message pump" or "event loop" of this synchronisation context. This is
        /// conceptually similar to the JS event loop in that there's only a single thread of
        /// execution running those messages. If there aren't any messages to run, the thread will
        /// block until there are.</summary>
        private void MessagePump()
        {
            Thread.CurrentThread.Name = nameof(BasicSynchronizationContext) + "Thread";
            var oldSyncCtx = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(this);
            try
            {
                foreach (var action in _queue.GetConsumingEnumerable())
                {
                    try
                    {
                        action.d(action.state);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unhandled exception " + ex);
                    }
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldSyncCtx);
                _queue.Dispose();
            }
        }

        /// <summary>Add an item to this synchronisation context for later execution.</summary>
        public override void Post(SendOrPostCallback d, object? state) => _queue.Add((d, state));

        /// <summary>Add an item to this synchronisation context for later execution and block until
        /// it has finished running. Beware of deadlocks when using this.</summary>
        public override void Send(SendOrPostCallback d, object? state)
        {
            var finished = new TaskCompletionSource<bool>();
            using var completed = _disposing.Token.Register(() => finished.TrySetCanceled());

            SendOrPostCallback callbackAndTrack = s =>
            {
                try
                {
                    d(s);
                }
                finally
                {
                    finished.TrySetResult(true);
                    completed.Unregister();
                }
            };
            _queue.Add((callbackAndTrack, state));

            finished.Task.Wait();
        }

        /// <summary>Creates a new synchronization context which executes work on this context.
        /// </summary>
        public override SynchronizationContext CreateCopy() =>
            new BasicSynchronizationContext(this);

        #region Equality Support
        public bool Equals(BasicSynchronizationContext? obj) =>
            Object.ReferenceEquals(_queue, obj?._queue);

        public override bool Equals(object? obj) => Equals(obj as BasicSynchronizationContext);

        public override int GetHashCode() => _queue.GetHashCode();
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        public void Dispose()
        {
            if (_ownsQueue && !disposedValue)
            {
                _disposing.Cancel();
                _queue.CompleteAdding();
            }
            disposedValue = true;
        }
        #endregion
    }
}