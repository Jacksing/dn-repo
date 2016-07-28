using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traceable
{
    public static class DictionaryExtend
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            TValue ret;
            if (dict.TryGetValue(key, out ret))
            {
                return ret;
            }
            else
            {
                return defaultValue;
            }
        }
    }

    public interface Traceable
    {
        string Traced { get; set; }
    }

    public abstract class Tracer<T> where T : Traceable
    {
        public class CannotBackwardException: Exception
        {
            public override string Message
            {
                get { return "Cannot trace back."; }
            }
        }

        private Dictionary<string, List<T>> _traceCollection = new Dictionary<string, List<T>>();

        public Dictionary<string, List<T>> TraceCollection
        {
            get { return _traceCollection; }
        }

        protected virtual string NewTraceId()
        {
            return Guid.NewGuid().ToString();
        }

        // Get all trace data that linked to trace.
        protected virtual List<T> GetTrace(string traceid)
        {
            if (_traceCollection.Values.Any(x => x.Any(y => y.Traced == traceid)))
            {
                throw new CannotBackwardException();
            }

            var trace = _traceCollection.Get(traceid, null);
            if (trace != null)
            {
                _traceCollection.Remove(traceid);
            }
            return trace;
        }

        // Create a new trace id and make all the trace data
        // linked to it, then save these linked trace data.
        // With the trace id all saved trace data can be found
        // by calling `GetTrace()` in the future.
        //
        // After all returns the trace id.
        protected virtual string SaveTrace(List<T> objList)
        {
            var traceId = this.NewTraceId();
            _traceCollection.Add(traceId, objList);
            return traceId;
        }

        protected abstract List<T> DoForward(List<T> objList);

        public virtual List<T> Forward(List<T> objList)
        {
            var newObjList = this.DoForward(objList.ToList());

            var traceId = this.SaveTrace(objList);
            newObjList.ForEach(x => x.Traced = traceId);

            return newObjList;
        }

        public List<T> Forward(params T[] objArray)
        {
            return this.Forward(objArray.ToList());
        }

        protected abstract void DoBackward(string tracdid);

        public virtual List<T> Backward(string tracdid)
        {
            this.DoBackward(tracdid);
            return this.GetTrace(tracdid);
        }

        public List<T> Backward(T obj)
        {
            return this.Backward(obj.Traced);
        }
    }

    /////////////////////////////////////////////////////////////
    /// basic test
    /////////////////////////////////////////////////////////////

    public class TestObj : List<string>, Traceable
    {
        public string Traced { get; set; }
    }

    public class TestTracer : Tracer<TestObj>
    {
        public int SeparateCopies = int.MinValue;

        protected override string NewTraceId()
        {
            return base.NewTraceId().Split('-')[0];
        }

        protected override List<TestObj> DoForward(List<TestObj> objList)
        {
            if (objList == null || objList.Count == 0)
            {
                return null;
            }

            // Do combination while get multi object, 
            // and do separate while get single one.

            if (objList.Count > 1)
            {
                var maxCount = objList.Max(x => x.Count);
                TestObj ret = new TestObj();
                ret.AddRange(Enumerable.Repeat("", maxCount));

                objList.ForEach(x =>
                {
                    for (int i = 0; i < x.Count; i++)
                    {
                        ret[i] = ret[i] + x[i];
                    }
                });

                return new List<TestObj>() {ret};
            }
            else
            {
                List<TestObj> ret = new List<TestObj>();

                for (int i = 0; i < SeparateCopies; i++)
                {
                    TestObj testObject = new TestObj();
                    objList[0].ForEach(x => testObject.Add(string.Format("{0}{1}", x, i.ToString())));
                    ret.Add(testObject);
                }

                return ret;
            }
        }

        protected override void DoBackward(string tracdid)
        {
        }
    }
}
