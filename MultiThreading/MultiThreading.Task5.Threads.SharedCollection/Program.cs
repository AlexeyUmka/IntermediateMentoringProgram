/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var synchronizedReadWrite = new SynchronizedReadWrite();
            synchronizedReadWrite.ElementAdded += () =>
            {
                return Task.Run(() =>
                {
                    Console.WriteLine("Element was added!");
                    Console.WriteLine(synchronizedReadWrite.Read(Enumerable.Range(1, synchronizedReadWrite.Count)));
                });
            };
            Task.Run(() =>
            {
                Enumerable.Range(1, 10).ToList().ForEach(key =>
                {
                    synchronizedReadWrite.Add(key, (key * 3).ToString()).GetAwaiter().GetResult();
                });
            });

            Console.ReadLine();
        }
    }
    
    public class SynchronizedReadWrite
    {
        public event Func<Task> ElementAdded;
        
        private readonly Dictionary<int, string> _dictionary = new();

        public int Count => _dictionary.Count;

        public string Read(IEnumerable<int> keys)
        {
            return string.Join(", ", keys.Select(k => _dictionary[k]));
        }

        public Task Add(int key, string value)
        {
            _dictionary.Add(key, value);
            return ElementAdded?.Invoke();
        }
    }
}
