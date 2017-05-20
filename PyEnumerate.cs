/*
 * Util class simulates the python `enumerate`.
 * 
 * Create by jackrole 2017/05/19
 */

namespace Jackrole.Utils.Translation
{
  public class PyEnumerate<T>
  {
    public int Index { get; set; }
    public T Value { get; set; }

    public static IEnumerable<PyEnumerate<T>> Walk(IEnumerable<T> items, int start)
    {
      int idx = start;
      foreach (var item in items)
      {
        yield return new PyEnumerate<T>() { Value = item, Index = idx++ };
      }
    }

    public static IEnumerable<PyEnumerate<T>> Walk(IEnumerable<T> items)
    {
      return Walk(items, 0);
    }
  }

  public class PyEnumerate : PyEnumerate<object> { }
}

// // Usage
// // --
// var fooList = new List<string>();
// foreach (var enumerator in PyEnumerate<string>.Walk(fooList))
// {
//   Console.WriteLine(enumerator.Value);
//   Console.WriteLine(enumerator.Index);
// }
