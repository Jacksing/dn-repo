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

    public interface TraceableObject
    {
        string Traced { get; set; }
    }

    public abstract class Tracer<T> where T : TraceableObject
    {
        private Dictionary<string, List<T>> _traceCollection = new Dictionary<string, List<T>>();

        public Dictionary<string, List<T>> TraceCollection
        {
            get { return _traceCollection; }
        }

        // Get all trace data that linked to trace.
        protected virtual List<T> GetTrace(string traceid)
        {
            var trace = _traceCollection.Get(traceid, null);
            if (trace != null)
            {
                _traceCollection.Remove(traceid);
            }
            return trace;
        }

        protected virtual string NewTraceId()
        {
            return Guid.NewGuid().ToString();
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

        // Combine all trace data into one new trace data.
        protected abstract T DoForward(List<T> objList);

        public virtual T Forward(List<T> objList)
        {
            var traceId = this.SaveTrace(objList);
            var newObj = this.DoForward(objList.ToList());
            newObj.Traced = traceId;

            return newObj;
        }

        public T Forward(params T[] objArray)
        {
            return this.Forward(objArray.ToList());
        }

        public virtual List<T> Backward(T obj)
        {
            return this.GetTrace(obj.Traced);
        }
    }

    /////////////////////////////////////////////////////////////
    /// basic test
    /////////////////////////////////////////////////////////////

    public class TestObj : List<string>, TraceableObject
    {
        public string Traced { get; set; }
    }

    public class TestTracer : Tracer<TestObj>
    {
        protected override string NewTraceId()
        {
            return base.NewTraceId().Split('-')[0];
        }

        protected override TestObj DoForward(List<TestObj> objList)
        {
            if (objList == null || objList.Count == 0)
            {
                return null;
            }

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

            return ret;
        }
    }
}
