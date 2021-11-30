/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            await DemonstrateA();
            await DemonstrateB();
            await DemonstrateC();
            await DemonstrateD();

            Console.ReadLine();
        }

        static Task DemonstrateA()
        {
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            var parentTask = Task.Run(() =>
            {
                Console.WriteLine("Parent throws exception");
                throw null;
            });
            return parentTask.ContinueWith(x =>
            {
                Console.WriteLine("Child continues anyway");
            });
        }
        
        static Task DemonstrateB()
        {
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            var parentTask = Task.Run(() =>
            {
                Console.WriteLine("Parent throws exception");
                throw null;
            });
            return parentTask.ContinueWith(x =>
            {
                Console.WriteLine("Child continues only in this case");
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        
        static Task DemonstrateC()
        {
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            var parentTask = Task.Run(() =>
            {
                Console.WriteLine($"Parent fails in thread - {Thread.CurrentThread.ManagedThreadId}");
                throw null;
                return true;
            });
            return parentTask.ContinueWith(x =>
            {
                try
                {
                    Console.WriteLine(x.Result);
                }
                catch
                {
                    Console.WriteLine($"Child catches exception and continues using parent task thread - {Thread.CurrentThread.ManagedThreadId}");
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        static Task DemonstrateD()
        {
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            var cancelTokenSource = new CancellationTokenSource();
            cancelTokenSource.Cancel();
            var parentTask = Task.Run(() =>
            {
                Thread.Sleep(3000);
            }, cancelTokenSource.Token);
            return parentTask
                .ContinueWith(x =>
                {
                    Console.WriteLine("Child continues only if parent is canceled");
                }, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning);
        }
    }
}
