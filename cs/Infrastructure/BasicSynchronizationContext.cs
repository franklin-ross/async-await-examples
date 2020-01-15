﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitExamples
{
    /// <summary>A simple synchronisation context which uses a single thread to run one piece of
    /// work at a time. Work is processed FIFO.</summary>
    public sealed class BasicSynchronizationContext : SynchronizationContext, IDisposable
    {
        private readonly bool _ownsQueue;
        private readonly BlockingCollection<(SendOrPostCallback d, object? state)> _queue;
        private readonly CancellationTokenSource _disposing;
        private readonly Ticker _executedCount;

        /// <summary>Creates a new synchronisation context which runs work on a dedicated thread.
        /// </summary>
        public BasicSynchronizationContext()
        {
            _ownsQueue = true;
            _queue = new BlockingCollection<(SendOrPostCallback d, object? state)>();
            _disposing = new CancellationTokenSource();
            _executedCount = new Ticker();
            Task.Run(Drain);
        }

        /// <summary>Creates a new synchronisation context which executes work on the provided
        /// queue.</summary>
        private BasicSynchronizationContext(BasicSynchronizationContext master)
        {
            _ownsQueue = false;
            _queue = master._queue;
            _disposing = master._disposing;
            _executedCount = master._executedCount;
        }

        private void Drain()
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
                    _executedCount.Tick();
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldSyncCtx);
                _queue.Dispose();
            }
        }

        public long ExecutedCount => _executedCount.Value;

        public override void Post(SendOrPostCallback d, object? state) => _queue.Add((d, state));

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

        class Ticker
        {
            public long Value;
            public void Tick() => Interlocked.Increment(ref Value);
        }
    }
}