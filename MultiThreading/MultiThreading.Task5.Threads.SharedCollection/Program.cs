/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var myBlockingCollection = new MyBlockingCollection<int>();
            var elementsAmount = 100;
            
            var updateTask = Task.Run(() =>
            {
                Enumerable.Range(1, elementsAmount).ToList().ForEach(i =>
                {
                    myBlockingCollection.WaitForDisplay();
                    myBlockingCollection.AddItem(i);
                });
            });
            
            var displayTask = Task.Run(() =>
            {
                Enumerable.Range(1, elementsAmount).ToList().ForEach(i =>
                {
                    myBlockingCollection.WaitForUpdate();
                    myBlockingCollection.Display();
                });
            });

            await Task.WhenAll(updateTask, displayTask);
            
            Console.ReadLine();
        }
    }

    public class MyBlockingCollection<T>
    {
        private readonly List<T> _items;
        private readonly ManualResetEvent _displayOperation;
        private readonly ManualResetEvent _updateOperation;
        public MyBlockingCollection()
        {
            _items = new List<T>();
            _displayOperation = new(false);
            _updateOperation = new(true);
        }

        public void WaitForDisplay()
        {
            _displayOperation.WaitOne();
        }

        public void WaitForUpdate()
        {
            _updateOperation.WaitOne();
        }

        public void Display()
        {
            _updateOperation.Reset();
            Console.WriteLine(string.Join(", ", _items));
            _displayOperation.Set();
        }

        public void AddItem(T item)
        {
            _displayOperation.Reset();
            _items.Add(item);
            _updateOperation.Set();
        }
    }
}
