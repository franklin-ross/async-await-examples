using System;
using System.Threading;

namespace AsyncAwaitExamples
{
    /// <summary>Temporarily installs a new <see cref="SynchronizationContext"/> until
    /// disposed.</summary>
    public sealed class SynchronizationContextScope : IDisposable
    {
        private bool _disposed = false;
        private readonly SynchronizationContext? _oldContext;

        public SynchronizationContextScope(SynchronizationContext? newContext)
        {
            _oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(newContext);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                SynchronizationContext.SetSynchronizationContext(_oldContext);
                _disposed = true;
            }
        }
    }
}