using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitExamples
{
    public class AsyncVoidTests
    {
        [Test]
        public void AsyncVoidLosesErrorsAndIsInvalidInSomeEnvironments()
        {
            // This is why you shouldn't ever write async void functions.
            async void SwallowsExceptions() =>
                await Task.FromException(new Exception("Nobody will see this error"));

            // In fact, they now throw a nasty "unsupported" error at runtime.
            // Assert.That(SwallowsError, Throws.Nothing);
        }
    }
}