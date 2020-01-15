using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitExamples
{
    /// <summary>
    /// This example shows two cases of using language features that yield execution to the caller,
    /// and can then return back to the yield point later. `yield return` has been around longer
    /// than `await` and seems better understood, but the concept is very similar.
    /// </summary>
    public class StateMachinesTests
    {
        [Test]
        public async Task AsyncStateMachine()
        {
            Console.WriteLine("Hi");
            await Task.Delay(1000);
            Console.WriteLine("Hi");
            await Task.Delay(1000);
            Console.WriteLine("Hi");
        }

        [Test]
        public void EnumerableStateMachine()
        {
            IEnumerable<int> EnumerableStateMachine()
            {
                Console.WriteLine("Hi");
                yield return 1000;
                Console.WriteLine("Hi");
                yield return 1000;
                Console.WriteLine("Hi");
            }

            foreach (var timeout in EnumerableStateMachine())
                Thread.Sleep(timeout);
        }
    }
}