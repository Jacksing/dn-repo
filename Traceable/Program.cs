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
            while (true)
            {
                PrintStatus(allDataCollection, tracer);

                var randomForward = RandomForward(allDataCollection, 4);

                if (randomForward == null)
                {
                    break;
                }

                var forwarded = tracer.Forward(randomForward.ToList());
                allDataCollection.Add(forwarded);
            }

            // backward
            while (true)
            {
                if (!allDataCollection.Any(x => x.Traced != null))
                {
                    break;
                }

                List<TestObj> tracedCollection = new List<TestObj>();
                foreach (var data in allDataCollection)
                {
                    if (data.Traced != null)
                    {
                        tracedCollection.AddRange(tracer.Backward(data));
                    }
                    else
                    {
                        tracedCollection.Add(data);
                    }
                }
                allDataCollection = tracedCollection;

                PrintStatus(allDataCollection, tracer);
            }

            Console.WriteLine("\r\nFinished, Enter to exit...");
            Console.ReadLine();
        }

        private static List<TestObj> RandomForward(List<TestObj> original, int count)
        {
            if (original.Count == 1)
            {
                return null;
            }

            List<TestObj> ret = new List<TestObj>();

            Random random = new Random();
            TestObj removed = null;

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
                var msg = string.Join("\t", data) + "\t" + tracedMsg(data.Traced);
                Console.WriteLine(msg);
            }

            Console.WriteLine();
            Console.WriteLine(">> Trace");

            foreach (var trace in tracer.TraceCollection)
            {
//                Console.WriteLine(trace.Key);

                foreach (var data in trace.Value)
                {
                    var msg = string.Join("\t", data) + "\t" + tracedMsg(data.Traced) + "\t" + trace.Key;
                    Console.WriteLine(msg);
                }

//                Console.WriteLine();
            }

            Console.WriteLine("\r\n\r\n");
        }
    }
}
