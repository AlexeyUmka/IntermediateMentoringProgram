/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            const int randomIntegerAmount = 10;
            var randomizer = new Random();
            var chainIndex = 0;
            await Task.Run(() =>
                    {
                        var result = Enumerable.Range(0, randomIntegerAmount).Select(_ => randomizer.Next()).ToList();
                        PrintArray(result, chainIndex);
                        chainIndex++;
                        return result;
                    })
                .ContinueWith(numbers =>
                {
                    var result = numbers.Result.Select(number => number * randomizer.Next()).ToList();
                    PrintArray(result, chainIndex);
                    chainIndex++;
                    return result;
                }, TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(numbers =>
                {
                    var result = numbers.Result.OrderBy(n => n).ToList();
                    PrintArray(result, chainIndex);
                    chainIndex++;
                    return result;
                }, TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(numbers => Console.WriteLine($"Chain index - {chainIndex} Average: {numbers.Result.Average()}"), TaskContinuationOptions.OnlyOnRanToCompletion);

            Console.ReadLine();
        }

        private static void PrintArray(IEnumerable<int> array, int chainIndex)
        {
            Console.WriteLine($"Chain index - {chainIndex}, array: {string.Join(", ", array)}");
        }
    }
}
