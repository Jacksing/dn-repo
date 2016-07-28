using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Traceable;

namespace Traceable
{
    internal class Program
    {
        private static int _counter = 1;

        private static readonly bool LONGxMESSAGE = true;

        private static void Main(string[] args)
        {
            #region allDataCollection
            List<TestObj> allDataCollection = new List<TestObj>()
            {
                new TestObj()
                {
                    "A1",
                    "A2",
                    "A3",
                    "A4",
                },
                new TestObj()
                {
                    "B1",
                    "B2",
                    "B3",
                    "B4",
                },
                new TestObj()
                {
                    "C1",
                    "C2",
                    "C3",
                    "C4",
                },
                new TestObj()
                {
                    "D1",
                    "D2",
                    "D3",
                    "D4",
                },
                new TestObj()
                {
                    "E1",
                    "E2",
                    "E3",
                    "E4",
                },
            }; 
            #endregion

            TestTracer tracer = new TestTracer();

            // foward
            for (int i = 0; i < 10; i++)
            {
                PrintStatus(allDataCollection, tracer);

                var randomCombine = RandomCombine(allDataCollection);

                if (randomCombine != null)
                {
                    var forwarded = tracer.Forward(randomCombine.ToList());
                    allDataCollection.Add(forwarded[0]);
                }

                PrintStatus(allDataCollection, tracer);

                var randomSeparate = RandomSeparate(allDataCollection, out tracer.SeparateCopies);

                allDataCollection.AddRange(tracer.Forward(new List<TestObj>() { randomSeparate }));
            }

            PrintStatus(allDataCollection, tracer);

            var allTraceId = (
                from item in allDataCollection.GroupBy(x => x.Traced)
                select item.Key).ToList();

            var allTracerTraceId = tracer.TraceCollection.Keys;

            // backward
            while (true)
            {
                if (!allDataCollection.Any(x => x.Traced != null))
                {
                    break;
                }

                // Attention:
                //   Make single action traced back in each loop,
                //   because if get multi actions traced back,
                //   the relationship of actions each other will
                //   be confused.
                var hasTracebackedInThisLoop = false;

                List<TestObj> tracedCollection = new List<TestObj>();
                foreach (var grouped in allDataCollection.GroupBy(x => x.Traced))
                {
                    if (grouped.Key != null && !hasTracebackedInThisLoop)
                    {
                        Console.WriteLine("pop " + grouped.Key);
                        try
                        {
                            tracedCollection.AddRange(tracer.Backward(grouped.Key));
                            hasTracebackedInThisLoop = true;
                        }
                        catch (TestTracer.CannotBackwardException)
                        {
                            tracedCollection.AddRange(grouped.ToList());
                        }
                    }
                    else
                    {
                        tracedCollection.AddRange(grouped.ToList());
                    }
                }
                allDataCollection = tracedCollection;

                PrintStatus(allDataCollection, tracer);
            }

            Console.WriteLine("\r\nFinished, Enter to exit...");
            Console.ReadLine();
        }

        private static List<TestObj> RandomCombine(List<TestObj> original, int? count = null)
        {
            if (original.Count == 1)
            {
                return null;
            }

            List<TestObj> ret = new List<TestObj>();

            Random random = new Random();
            TestObj removed = null;

            if (!count.HasValue)
            {
                count = random.Next(2, original.Count + 1);
            }

            for (int i = 0; i < count; i++)
            {
                if (original.Count == 1)
                {
                    removed = original[0];
                    original.Remove(removed);
                    ret.Add(removed);
                    return ret;
                }

                removed = original[random.Next(0, original.Count)];
                original.Remove(removed);
                ret.Add(removed);
            }

            return ret;
        }

        private static TestObj RandomSeparate(List<TestObj> original, out int separateCopies)
        {
            if (original.Count == 0)
            {
                separateCopies = 0;
                return null;
            }

            Random random = new Random();
            var toBeSeparate = original[random.Next(0, original.Count)];
            original.Remove(toBeSeparate);

            separateCopies = random.Next(2, 5);

            return toBeSeparate;
        }

        private static void PrintStatus(List<TestObj> allDataCollection, TestTracer tracer)
        {
            Func<string, string> tracedMsg = x =>
            {
                return x == null ? "  null  " : x;
            };

            Console.WriteLine(_counter++.ToString() + ") --------------------");

            Console.WriteLine(">> Current Data");

            foreach (var data in allDataCollection)
            {
                var msg = LONGxMESSAGE
                    ? string.Join("\t", data) + "\t" + tracedMsg(data.Traced)
                    : tracedMsg(data.Traced);
                Console.WriteLine(msg);
            }

            Console.WriteLine();
            Console.WriteLine(">> Trace");

            foreach (var trace in tracer.TraceCollection)
            {
                foreach (var data in trace.Value)
                {
                    var msg = LONGxMESSAGE
                        ? string.Join("\t", data) + "\t" + tracedMsg(data.Traced) + "\t" + trace.Key
                        : tracedMsg(data.Traced) + "\t" + trace.Key;
                    Console.WriteLine(msg);
                }
            }

            Console.WriteLine("\r\n\r\n");
        }
    }
}
