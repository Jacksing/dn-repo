/*
 * Common checker for matrix data.
 * 
 * Supporting cell, row and whole matrix checks, and generate 
 * a result view in the format of a html table.
 * 
 * Create by jackrole 2016/03/05
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TS = jackrole.Utils.Translation.TranslatedString;
using System.Text;
using System.Diagnostics;

namespace Jackrole.DataSource.Checker
{
    //public delegate bool CellCheckerActionEventHandler(string value, out string message);
    //public delegate bool RowCheckerActionEventHandler(string[] cells, out string messge);
    //public delegate bool MatrixCheckerActionEventHandler(List<string[]> matrix, out string message);
    //public delegate bool RowCheckerActionXEventHandler<T>(string[] cells, out string messge, out T value);  // TODO: <jackrole> be generic in the future??

    public delegate bool CommonCheckerActionEventHandler<TContent>(TContent value, out string message);

    public struct Cell
    {
        public int Row;
        public int Col;

        public string ToString()
        {
            return string.Format("[Row={0},Col={1}]", this.Row, this.Col);
        }
    }

    public abstract class CommonChecker<TContent>
    {
        public CommonCheckerActionEventHandler<TContent>[] Actions { get; set; }

        public bool ContinueOnError { get; set; }

        public virtual bool Valid(TContent content, out string[] errors, out string[] warnings)
        {
            var errorList = new List<string>();
            var warningList = new List<string>();

            // do custom check(s)
            // in each loop here there is three result patterns:
            // 1) [result = false] and [message != ""]
            //    means check error and exit following check(s)
            //
            // 2) [result = true] and [message != ""]
            //    means check error but continues with following check(s)
            //
            // 3) [result = true] and [message == ""]
            //    means check OK and continues with following check(s)
            if (this.Actions != null)
            {
                var message = "";
                foreach (var action in this.Actions)
                {
                    if (action(content, out message))
                    {
                        if (message != "")
                        {
                            warningList.Add(message);
                        }
                    }
                    else
                    {
                        Debug.Assert(message != "", "No error message returned while check result is false.");
                        errorList.Add(message);
                        if (!this.ContinueOnError)
                        {
                            errors = errorList.ToArray();
                            warnings = warningList.ToArray();
                            return false;
                        }
                    }
                }
            }

            errors = errorList.ToArray();
            warnings = warningList.ToArray();
            return true;
        }
    }

    public class CellChecker : CommonChecker<string>
    {
        public int? MaxLength { get; set; }
        public int? MinLengh { get; set; }
        public Type DataType { get; set; }
        //public CellCheckerActionEventHandler[] Actions { get; set; }

        //public bool ContinueOnError { get; set; }

        public CellChecker()
        {

        }

        public CellChecker(params CommonCheckerActionEventHandler<string>[] actions)
        {
            this.Actions = actions;
        }

        public override bool Valid(string content, out string[] errors, out string[] warnings)
        {
            var errorList = new List<string>();
            var warningList = new List<string>();

            if (this.MaxLength.HasValue && this.MaxLength.Value < content.Length)
            {
                errorList.Add(string.Format((TS)"Length Error, \"{0}\"(Length:{1}) is longer than {2}", content, content.Length, MaxLength));
                if (!this.ContinueOnError)
                {
                    errors = errorList.ToArray();
                    warnings = warningList.ToArray();
                    return false;
                }
            }
            if (this.MinLengh.HasValue && this.MinLengh.Value > content.Length)
            {
                errorList.Add(string.Format((TS)"Length Error, \"{0}\"(Length:{1}) is shorter than {2}", content, content.Length, MinLengh));
                if (!this.ContinueOnError)
                {
                    errors = errorList.ToArray();
                    warnings = warningList.ToArray();
                    return false;
                }
            }

            string[] baseErrors;
            string[] baseWarnings;

            var baseResult = base.Valid(content, out baseErrors, out baseWarnings);

            errorList.AddRange(baseErrors);
            warningList.AddRange(baseWarnings);

            errors = errorList.ToArray();
            warnings = warningList.ToArray();

            return !(!baseResult || (errors.Length > 0 && !this.ContinueOnError));
        }

        //public bool Valid(string value, out string[] errors, out string[] warnings)
        //{
        //    var errorList = new List<string>();
        //    var warningList = new List<string>();

        //    if (this.MaxLength.HasValue && this.MaxLength.Value < value.Length)
        //    {
        //        errorList.Add(string.Format((TS)"Length Error, <{0}>({1}) is longer than {2}", value, value.Length, MaxLength));
        //        if (!this.ContinueOnError)
        //        {
        //            errors = errorList.ToArray();
        //            warnings = warningList.ToArray();
        //            return false;
        //        }
        //    }
        //    if (this.MinLengh.HasValue && this.MinLengh.Value > value.Length)
        //    {
        //        errorList.Add(string.Format((TS)"Length Error, <{0}>({1}) is shorter than {2}", value, value.Length, MinLengh));
        //        if (!this.ContinueOnError)
        //        {
        //            errors = errorList.ToArray();
        //            warnings = warningList.ToArray();
        //            return false;
        //        }
        //    }

        //    // do custom check(s)
        //    // in each loop here there is three result patterns:
        //    // 1) [result = false] and [message != ""]
        //    //    means check error and exit following check(s)
        //    //
        //    // 2) [result = true] and [message != ""]
        //    //    means check error but continues with following check(s)
        //    //
        //    // 3) [result = true] and [message == ""]
        //    //    means check OK and continues with following check(s)
        //    if (this.Actions !=null)
        //    {
        //        var message = "";
        //        foreach (var action in this.Actions)
        //        {
        //            if (action(value, out message))
        //            {
        //                if (message != "")
        //                {
        //                    warningList.Add(message);
        //                }
        //            }
        //            else
        //            {
        //                Debug.Assert(message != "", "No error message returned while check result is false.");
        //                errorList.Add(message);
        //                if (!this.ContinueOnError)
        //                {
        //                    errors = errorList.ToArray();
        //                    warnings = warningList.ToArray();
        //                    return false;
        //                }
        //            }
        //        } 
        //    }

        //    errors = errorList.ToArray();
        //    warnings = warningList.ToArray();
        //    return errors.Length == 0;
        //}

        #region check methods
        // Todo: <jackrole> this is for test, delete it in the future or add some common check methods here.
        public static bool IsLongDateTime(string value, out string message)
        {
            try
            {
                DateTime.Parse(value);
                message = "";
                return true;
            }
            catch
            {
                message = string.Format((TS)"<{0}> is not long date time.", value);
                return false;
            }
        }

        public static bool HasGoodInSentence(string value, out string message)
        {
            if (!value.Contains("good"))
            {
                message = (TS)"Can't find good in sentence.";
            }
            else
            {
                message = "";
            }
            return true;
        }

        public static bool HasIsInSentence(string value, out string message)
        {
            if (!value.Contains("is"))
            {
                message = (TS)"Can't find is in sentence.";
            }
            else
            {
                message = "";
            }
            return true;
        }
        #endregion
    }

    public static class CellCheckerFactory
    {
        private static bool VerifyShort(string value, out string message)
        {
            try
            {
                short.Parse(value);
                message = "";
                return true;
            }
            catch (Exception)
            {
                message = (TS)"It should be a short type.";
                return false;
            }
        }

        private static bool VerifyShortNullable(string value, out string message)
        {
            if (value.Trim().Length == 0)
            {
                message = "";
                return true;
            }

            return CellCheckerFactory.VerifyShort(value, out message);
        }

        private static bool VerifyInt(string value, out string message)
        {
            try
            {
                int.Parse(value);
                message = "";
                return true;
            }
            catch (Exception)
            {
                message = (TS)"It should be a int type.";
                return false;
            }
        }

        private static bool VerifyIntNullable(string value, out string message)
        {
            if (value.Trim().Length == 0)
            {
                message = "";
                return true;
            }

            return CellCheckerFactory.VerifyInt(value, out message);
        }

        private static bool VerifyDateTime(string value, out string message)
        {
            try
            {
                DateTime.Parse(value);
                message = "";
                return true;
            }
            catch (Exception)
            {
                message = (TS)"It should be a DateTime type.";
                return false;
            }
        }

        private static bool VerifyDateTimeNullable(string value, out string message)
        {
            if (value.Trim().Length == 0)
            {
                message = "";
                return true;
            }

            return CellCheckerFactory.VerifyDateTime(value, out message);
        }

        //private bool VerifyDecimal13x8(string value, out string message)
        //{
        //    try
        //    {
        //        decimal.Parse(value);
        //        if (DecimalRound.VerifyDecimal(value, 13, 8))
        //        {
        //            message = (TS)"Length error. It should be 13 integer place and 8 decimal place.";
        //            return false;
        //        }
        //        message = "";
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        message = (TS)"It should be a decimal type.";
        //        return false;
        //    }
        //}

        public static CellChecker StringChecker(int maxLength, int minLength)
        {
            return new CellChecker { MaxLength = maxLength, MinLengh = minLength };
        }

        public static CellChecker StringChecker(int maxLength, bool allowNull)
        {
            return CellCheckerFactory.StringChecker(maxLength, allowNull ? 0 : 1);
        }

        public static CellChecker ShortChecker(bool allowNull = false)
        {
            if (allowNull)
            {
                return new CellChecker { Actions = new CommonCheckerActionEventHandler<string>[] { CellCheckerFactory.VerifyShortNullable } };
            }
            else
            {
                return new CellChecker { Actions = new CommonCheckerActionEventHandler<string>[] { CellCheckerFactory.VerifyShort } };
            }
        }

        public static CellChecker IntChecker(bool allowNull = false)
        {
            if (allowNull)
            {
                return new CellChecker { Actions = new CommonCheckerActionEventHandler<string>[] { CellCheckerFactory.VerifyIntNullable } };
            }
            else
            {
                return new CellChecker { Actions = new CommonCheckerActionEventHandler<string>[] { CellCheckerFactory.VerifyInt } };
            }
        }

        public static CellChecker DateTimeChecker(bool allowNull)
        {
            if (allowNull)
            {
                return new CellChecker { Actions = new CommonCheckerActionEventHandler<string>[] { CellCheckerFactory.VerifyDateTimeNullable } };
            }
            else
            {
                return new CellChecker { Actions = new CommonCheckerActionEventHandler<string>[] { CellCheckerFactory.VerifyDateTime } };
            }
        }
    }

    public class RowChecker : CommonChecker<string[]>
    {
        //public RowCheckerActionEventHandler[] Actions { get; set; }

        //public bool ContinueOnError { get; set; }

        public RowChecker()
        {

        }

        public RowChecker(params CommonCheckerActionEventHandler<string[]>[] actions)
        {
            this.Actions = actions;
        }

        //public bool Valid(string[] cells, out string[] errors, out string[] warnings)
        //{
        //    var errorList = new List<string>();
        //    var warningList = new List<string>();

        //    // do custom check(s)
        //    // in each loop here there is three result patterns:
        //    // 1) [result = false] and [message != ""]
        //    //    means check error and exit following check(s)
        //    //
        //    // 2) [result = true] and [message != ""]
        //    //    means check error but continues with following check(s)
        //    //
        //    // 3) [result = true] and [message == ""]
        //    //    means check OK and continues with following check(s)
        //    if (this.Actions != null)
        //    {
        //        var message = "";
        //        foreach (var action in this.Actions)
        //        {
        //            if (action(cells, out message))
        //            {
        //                if (message != "")
        //                {
        //                    warningList.Add(message);
        //                }
        //            }
        //            else
        //            {
        //                Debug.Assert(message != "", "No error message returned while check result is false.");
        //                errorList.Add(message);
        //                if (!this.ContinueOnError)
        //                {
        //                    errors = errorList.ToArray();
        //                    warnings = warningList.ToArray();
        //                    return false;
        //                }
        //            }
        //        } 
        //    }

        //    errors = errorList.ToArray();
        //    warnings = warningList.ToArray();
        //    return true;
        //}
    }

    public class MatrixChecker : CommonChecker<List<string[]>>
    {
        //public MatrixCheckerActionEventHandler[] Actions { get; set; }

        //public bool ContinueOnError { get; set; }

        public MatrixChecker()
        {

        }

        public MatrixChecker(params CommonCheckerActionEventHandler<List<string[]>>[] actions)
        {
            this.Actions = actions;
        }

        //public bool Valid(List<string[]> matrix, out string[] errors, out string[] warnings)
        //{
        //    var errorList = new List<string>();
        //    var warningList = new List<string>();

        //    if (this.Actions != null)
        //    {
        //        var message = "";
        //        foreach (var action in this.Actions)
        //        {
        //            if (action(matrix, out message))
        //            {
        //                if (message != "")
        //                {
        //                    warningList.Add(message);
        //                }
        //            }
        //            else
        //            {
        //                Debug.Assert(message != "", "No error message returned while check result is false.");
        //                errorList.Add(message);
        //                if (!this.ContinueOnError)
        //                {
        //                    errors = errorList.ToArray();
        //                    warnings = warningList.ToArray();
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    errors = errorList.ToArray();
        //    warnings = warningList.ToArray();
        //    return true;
        //}
    }

    /// <summary>
    /// チェック結果
    /// </summary>
    public class CheckResult
    {
        public Dictionary<Cell, List<string>> ErrorList { get; private set; }
        public Dictionary<Cell, List<string>> WarningList { get; private set; }

        public string Name { get; set; }
        public List<string[]> Content { get; set; }

        public int ErrorCount { get { return ErrorList.Count; } }
        public int WarningCount { get { return this.WarningList.Count; } }

        public CheckResult(List<string[]> content=null)
        {
            this.Name = "";
            this.Content = content;
            this.ErrorList = new Dictionary<Cell, List<string>>();
            this.WarningList = new Dictionary<Cell, List<string>>();
        }

        public bool Any()
        {
            return this.ErrorCount > 0 || this.WarningCount > 0;
        }

        public void AppendCsvWarning(string[] messages)
        {
            this.AppendCellWarning(-1, -1, messages);
        }

        public void AppendCsvWarning(string message)
        {
            this.AppendCsvWarning(new[] { message });
        }

        public void AppendCsvError(string[] messages)
        {
            this.AppendCellError(-1, -1, messages);
        }

        public void AppendCsvError(string message)
        {
            this.AppendCsvError(new[] { message });
        }

        public void AppendRowWarning(int row, string[] messages)
        {
            this.AppendCellWarning(row, -1, messages);
        }

        public void AppendRowWarning(int row, string message)
        {
            this.AppendRowWarning(row, new[] { message });
        }

        public void AppendRowWarning(Predicate<string[]> match, string[] message)
        {
            Debug.Assert(this.Content != null, "No check content to locate the specified row.");
            for (int i = 0; i < this.Content.Count; i++)
            {
                if (match(this.Content[i]))
                {
                    this.AppendRowWarning(i, message);
                }
            }
        }

        public void AppendRowWarning(Predicate<string[]> match, string message)
        {
            this.AppendRowWarning(match, new[] { message });
        }

        public void AppendRowError(int row, string[] messages)
        {
            this.AppendCellError(row, -1, messages);
        }

        public void AppendRowError(int row, string message)
        {
            this.AppendRowError(row, new[] { message });
        }

        public void AppendRowError(Predicate<string[]> match, string[] message)
        {
            Debug.Assert(this.Content != null, "No check content to locate the specified row.");
            for (int i = 0; i < this.Content.Count; i++)
            {
                if (match(this.Content[i]))
                {
                    this.AppendRowError(i, message);
                }
            }
        }

        public void AppendRowError(Predicate<string[]> match, string message)
        {
            this.AppendRowError(match, new[] { message });
        }

        public void AppendCellWarning(int row, int col, string[] messages)
        {
            var item = from w in this.WarningList where w.Key.Row == row && w.Key.Col == col select w;
            if (item.Any())
            {
                item.FirstOrDefault().Value.AddRange(messages);
            }
            else
            {
                this.WarningList.Add(new Cell { Row = row, Col = col }, new List<string>(messages));
            }
        }

        public void AppendCellWarning(int row, int col, string message)
        {
            this.AppendCellWarning(row, col, new[] { message });
        }

        public void AppendCellError(int row, int col, string[] messages)
        {
            var item = from e in this.ErrorList where e.Key.Row == row && e.Key.Col == col select e;
            if (item.Any())
            {
                item.FirstOrDefault().Value.AddRange(messages);
            }
            else
            {
                this.ErrorList.Add(new Cell { Row = row, Col = col }, new List<string>(messages));
            }
        }

        public void AppendCellError(int row, int col, string message)
        {
            this.AppendCellError(row, col, new[] { message });
        }

        private void _Concat(Dictionary<Cell, List<string>> old, Dictionary<Cell, List<string>> nevv)
        {
            foreach (var item in nevv)
            {
                if (old.ContainsKey(item.Key))
                {
                    old[item.Key].AddRange(item.Value);
                }
                else
                {
                    old.Add(item.Key, new List<string>(item.Value));
                }
            }
        }

        public void Concat(CheckResult newResult)
        {
            this._Concat(this.WarningList, newResult.WarningList);
            this._Concat(this.ErrorList, newResult.ErrorList);
        }

        private List<string[]> SplitResult(bool getVerfied, bool includeWarning, Predicate<string[]> include, Predicate<string[]> exclude)
        {
            if (this.Content == null)
            {
                return null;
            }

            var errorLineNumber = (from item in this.ErrorList select item.Key.Row).Distinct().ToList();

            if (getVerfied != includeWarning)
            {
                var warningLineNumber = (from item in this.WarningList select item.Key.Row).Distinct().ToList();
                errorLineNumber.AddRange(warningLineNumber);
                errorLineNumber = errorLineNumber.Distinct().ToList(); ;
            }

            List<string[]> result = new List<string[]>();
            int index = 0;
            var enumerator = this.Content.GetEnumerator();
            while (enumerator.MoveNext())
            {
                bool extra = true;
                if (include != null)
                {
                    extra = include(enumerator.Current);
                }
                if (exclude !=null)
                {
                    extra = !exclude(enumerator.Current);
                }

                if (errorLineNumber.Contains(index++) != getVerfied && extra)
                {
                    result.Add(enumerator.Current);
                }
            }
            return result;
        }

        /// <summary>
        /// Get contents with no error, set `includeWarning = true` to get the warning contents included.
        /// </summary>
        /// <param name="includeWarning">Whether to return warning content.</param>
        /// <returns></returns>
        public List<string[]> GetVerifiedContent(bool includeWarning = false)
        {
            return this.SplitResult(true, includeWarning, null, null);
        }

        /// <summary>
        /// Get contents with no error, set `includeWarning = true` to get the warning contents included.
        /// </summary>
        /// <param name="includeWarning">Whether to return warning content.</param>
        /// <param name="include">Get specified contents included.</param>
        /// <param name="exclude">Get specified contents excluded.</param>
        /// <returns></returns>
        public List<string[]> GetVerifiedContent(bool includeWarning, Predicate<string[]> include, Predicate<string[]> exclude)
        {
            return this.SplitResult(true, includeWarning, include, exclude);
        }

        /// <summary>
        /// Get error content, and warning content while specified.
        /// </summary>
        /// <param name="includeWarning">Whether to return warning content.</param>
        /// <returns></returns>
        public List<string[]> GetContentiousContent(bool includeWarning = false)
        {
            return this.SplitResult(false, includeWarning, null, null);
        }

        /// <summary>
        /// Get error content, and warning content while specified.
        /// </summary>
        /// <param name="includeWarning">Whether to return warning content.</param>
        /// <param name="include">Get specified contents included.</param>
        /// <param name="exclude">Get specified contents excluded.</param>
        /// <returns></returns>
        public List<string[]> GetContentiousContent(bool includeWarning, Predicate<string[]> include, Predicate<string[]> exclude)
        {
            return this.SplitResult(false, includeWarning, include, exclude);
        }
    }

    /// <summary>
    /// マトリクスの仕組みの正確性をチェックする。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CsvChecker<T>
    {
        private static Dictionary<int, CellChecker> _cellCheckerList = null;
        private static RowChecker _rowPrevChecker = null;
        private static RowChecker _rowPostChecker = null;
        private static MatrixChecker _matrixPrevChecker = null;
        private static MatrixChecker _matrixPostChecker = null;

        public bool ContinueOnError { get; set; }

        // deep: whether to set CellChecker/RowChecker with the same ContinueOnError value.
        public void SetContinueOnError(bool value, bool deep = true)
        {
            this.ContinueOnError = value;
            if (deep)
            {
                if (CsvChecker<T>._cellCheckerList != null)
                {
                    foreach (var item in CsvChecker<T>._cellCheckerList.Values)
                    {
                        item.ContinueOnError = value;
                    }
                }
                if (_rowPrevChecker != null)
                {
                    _rowPrevChecker.ContinueOnError = value;
                }
                if (_rowPostChecker != null)
                {
                    _rowPostChecker.ContinueOnError = value;
                }
            }
        }

        public CsvChecker()
        {
            if (CsvChecker<T>._matrixPrevChecker == null)
            {
                CsvChecker<T>._matrixPrevChecker = this.InitializeMatrixPrevChecker();
            }
            if (CsvChecker<T>._rowPrevChecker == null)
            {
                CsvChecker<T>._rowPrevChecker = this.InitializeRowPrevChecker();
            }
            if (CsvChecker<T>._cellCheckerList == null)
            {
                CsvChecker<T>._cellCheckerList = this.InitializeCellCheckerList();
            }
            if (CsvChecker<T>._rowPostChecker == null)
            {
                CsvChecker<T>._rowPostChecker = this.InitializeRowPostChecker();
            }
            if (CsvChecker<T>._matrixPostChecker == null)
            {
                CsvChecker<T>._matrixPostChecker = this.InitializeMatrixPostChecker();
            }
        }

        // create cell check patterns
        protected virtual Dictionary<int, CellChecker> InitializeCellCheckerList()
        {
            return null;
        }

        // create row check pattern, it will be run after cell checkers.
        protected virtual RowChecker InitializeRowPrevChecker()
        {
            return null;
        }

        // create row check pattern, it will be run before cell checkers.
        protected virtual RowChecker InitializeRowPostChecker()
        {
            return null;
        }

        // create matrix check pattern, it will be run before previous-row checkers.
        protected virtual MatrixChecker InitializeMatrixPrevChecker()
        {
            return null;
        }

        // create matrix check pattern, it will be run after post-row checkers.
        protected virtual MatrixChecker InitializeMatrixPostChecker()
        {
            return null;
        }

        // clear cache data in valid progress.
        //protected abstract void ClearCache();  // not used, because static cache can not be cleared by instance method. // TODO: <jackrole> implete it in the future.

        // check with pattern that configured in class.
        public CheckResult Valid(List<string[]> matrix)
        {
            return this.Valid(matrix, CsvChecker<T>._cellCheckerList, CsvChecker<T>._rowPrevChecker, CsvChecker<T>._rowPostChecker, CsvChecker<T>._matrixPrevChecker, CsvChecker<T>._matrixPostChecker);
        }

        // check with pattern that specified in parameter.
        public CheckResult Valid(List<string[]> matrix, Dictionary<int, CellChecker> cellCheckerDictionary, RowChecker rowPrevChecker, RowChecker rowPostChecker, MatrixChecker matrixPrevChecker, MatrixChecker matrixPostChecker)
        {
            CheckResult result = new CheckResult();
            string[] errors;
            string[] warnings;

            result.Content = matrix;

            #region Previous Matrix Check
            if (matrixPrevChecker != null)
            {
                matrixPrevChecker.Valid(matrix, out errors, out warnings);

                if (warnings.Length > 0)
                {
                    result.AppendCsvWarning(warnings);
                }

                if (errors.Length > 0)
                {
                    result.AppendCsvError(errors);
                    if (!this.ContinueOnError)
                    {
                        return result;
                    }
                }
            } 
            #endregion

            for (int row = 0; row < matrix.Count; row++)
            {
                #region Previous Row Check
                if (rowPrevChecker != null)
                {
                    rowPrevChecker.Valid(matrix[row], out errors, out warnings);

                    if (warnings.Length > 0)
                    {
                        result.AppendRowWarning(row, warnings);
                    }

                    if (errors.Length > 0)
                    {
                        result.AppendRowError(row, errors);
                        if (!this.ContinueOnError)
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region Check Cells
                for (int col = 0; col < (cellCheckerDictionary != null ? matrix[row].Length : -1); col++)
                {
                    if (!cellCheckerDictionary.ContainsKey(col))
                    {
                        continue;
                    }

                    cellCheckerDictionary[col].Valid(matrix[row][col], out errors, out warnings);

                    if (warnings.Length > 0)
                    {
                        result.AppendCellWarning(row, col, warnings);
                    }

                    if (errors.Length > 0)
                    {
                        result.AppendCellError(row, col, errors);
                        if (!this.ContinueOnError)
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region Post Row Check
                if (rowPostChecker != null)
                {
                    if (rowPostChecker.Valid(matrix[row], out errors, out warnings))
                    {
                        if (warnings.Length > 0)
                        {
                            result.AppendRowWarning(row, warnings);
                        }
                    }
                    else
                    {
                        Debug.Assert(errors.Length > 0, "No error message returned while check result is false.");
                        result.AppendRowError(row, errors);
                        if (!this.ContinueOnError)
                        {
                            break;
                        }
                    }
                }
                #endregion
            }

            #region Post Matrix Check
            if (matrixPostChecker != null)
            {
                matrixPostChecker.Valid(matrix, out errors, out warnings);

                if (warnings.Length > 0)
                {
                    result.AppendCsvWarning(warnings);
                }

                if (errors.Length > 0)
                {
                    result.AppendCsvError(errors);
                    if (!this.ContinueOnError)
                    {
                        return result;
                    }
                }
            } 
            #endregion

            return result;
        }

        // check with pattern that configured in class,
        // the mapping of check is specified by each id of indexedMaxtrix.
        public CheckResult Valid(List<Dictionary<int, string[]>> indexedMaxtrix)
        {
            // TODO: <jackrole> data of column can specifies which checker to check column itself.
            throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// チェック結果出力時、結果内容作成用のCache
    /// </summary>
    public class ResultTableBuilder
    {
        const string ERRORxCLASS = "e";
        const string WARNINGxCLASS = "w";
        const string ERRORxWARNINGxCLASS = "ew";

        const string ERRORxMSGxCLASS = "em";
        const string WARNINGxMSGxCLASS = "wm";

        const string DIVISIONxCLASS = "d";

        readonly string ERRORxFILTER = (TS)"Cell Error";
        readonly string WARNINGxFILTER = (TS)"Cell Warning";

        readonly string ROWxERRORxFILTER = (TS)"Record Error";
        readonly string ROWxWARNINGxFILTER = (TS)"Record Warning";

        private StringBuilder _stringBuilder = null;

        private StringBuilder _errorStackBuilder = null;
        private StringBuilder _warningStackBuikder = null;

        // records the summary of columns while adding cell. <see: function AddCell()>
        private int _columnCount = 0;

        private bool _hasError = false;
        private bool _hasWarning = false;

        public bool CreateFilter { get; set; }

        string _tableStyle = string.Format("<style type=\"text/css\"> "
            + "table {{border-spacing: 0; border-collapse: collapse; }} "
            + "tr {{height: 20px; }} "
            + "tr.{3} {{height: 15px; font-size: 10px; }} "
            + "tr.{4} {{height: 15px; font-size: 10px; }} "
            + "td {{border: .5pt solid #c0c0c0; background-color: #eaeaea; }} "
            + "td.{0} {{color: #ff0000; font-weight: 700; }} "
            + "td.{1} {{color: #0000ff; font-weight: 700; }} "
            + "td.{2} {{color: #ff0000; font-weight: 700; }} "
            + "td.{3} {{background-color: #fff; color: #ff9797; }}"
            + "td.{4} {{background-color: #fff; color: #9797ff; }}"
            + "td.{5} {{height: 22px; font-size: 14px; font-weight: 700; background-color: #ffc000; color: #fff; }}"
            + "</style>", ERRORxCLASS, WARNINGxCLASS, ERRORxWARNINGxCLASS, ERRORxMSGxCLASS, WARNINGxMSGxCLASS, DIVISIONxCLASS);

        public ResultTableBuilder(bool createFilter = true)
        {
            _stringBuilder = new StringBuilder(_tableStyle);
            _errorStackBuilder = new StringBuilder();
            _warningStackBuikder = new StringBuilder();

            this.CreateFilter = createFilter;
        }

        public StringBuilder BeginTable()
        {
            return _stringBuilder.Append("<table>");
        }

        public StringBuilder EndTable()
        {
            return _stringBuilder.Append("</table>");
        }

        public StringBuilder BeginRow()
        {
            Debug.Assert(_errorStackBuilder.Length == 0, "Some thing error in your coding, error stack should be empty while beginning a row.");
            Debug.Assert(_warningStackBuikder.Length == 0, "Some thing error in your coding, warning stack should be empty while beginning a row.");

            _columnCount = 0;

            if (this.CreateFilter)
            {
                _stringBuilder.Append("<tr><td></td>");

                _errorStackBuilder.AppendFormat("<tr class=\"{0}\"><td>{1}</td>", ERRORxMSGxCLASS, ERRORxFILTER);
                _warningStackBuikder.AppendFormat("<tr class=\"{0}\"><td>{1}</td>", WARNINGxMSGxCLASS, WARNINGxFILTER);
            }
            else
            {
                _stringBuilder.Append("<tr>");

                _errorStackBuilder.AppendFormat("<tr class=\"{0}\">", ERRORxMSGxCLASS);
                _warningStackBuikder.AppendFormat("<tr class=\"{0}\">", WARNINGxMSGxCLASS);
            }

            return _stringBuilder;
        }

        public StringBuilder EndRow(string[] error, string[] warning)
        {
            _stringBuilder.Append("</tr>");

            if (_hasError)
            {
                _errorStackBuilder.Append("</tr>");
                _stringBuilder.Append(_errorStackBuilder);
                _hasError = false;
            }
            if (_errorStackBuilder.Length > 0)
            {
                _errorStackBuilder.Clear();
            }

            if (_hasWarning)
            {
                _warningStackBuikder.Append("</tr>");
                _stringBuilder.Append(_warningStackBuikder);
                _hasWarning = false;
            }
            if (_warningStackBuikder.Length > 0)
            {
                _warningStackBuikder.Clear();
            }

            if (error != null && error.Length > 0)
            {
                if (this.CreateFilter)
                {
                    _stringBuilder.AppendFormat("<tr class=\"{0}\"><td>{1}</td><td class=\"{0}\" colspan=\"{2}\">{3}</td></tr>",
                            ERRORxMSGxCLASS,
                            ROWxERRORxFILTER,
                            _columnCount,
                            this.Format(string.Join("\n", error))); 
                }
                else
                {
                    _stringBuilder.AppendFormat("<tr class=\"{0}\"><td class=\"{0}\" colspan=\"{1}\">{2}</td></tr>",
                            ERRORxMSGxCLASS,
                            _columnCount,
                            this.Format(string.Join("\n", error))); 
                }
            }

            if (warning != null && warning.Length > 0)
            {
                if (this.CreateFilter)
                {
                    _stringBuilder.AppendFormat("<tr class=\"{0}\"><td>{1}</td><td class=\"{0}\" colspan=\"{2}\">{3}</td></tr>",
                            WARNINGxMSGxCLASS,
                            ROWxWARNINGxFILTER,
                            _columnCount,
                            this.Format(string.Join("\n", warning)));
                }
                else
                {
                    _stringBuilder.AppendFormat("<tr class=\"{0}\"><td colspan=\"{1}\">{2}</td></tr>",
                            WARNINGxMSGxCLASS,
                            _columnCount,
                            this.Format(string.Join("\n", warning)));
                }
            }

            return _stringBuilder;
        }

        public StringBuilder InsertDivisionRow(string divisionName="", int colSpan=0)
        {
            if (this.CreateFilter)
            {
                _stringBuilder.AppendFormat("<tr class=\"{0}\"><td class=\"{0}\"></td><td class=\"{0}\" colspan=\"{1}\">{2}</td></tr>",
                    DIVISIONxCLASS, colSpan, this.Format(divisionName));
            }
            else
            {
                _stringBuilder.AppendFormat("<tr class=\"{0}\"><td class=\"{0}\" colspan=\"{1}\">{2}</td></tr>",
                    DIVISIONxCLASS, colSpan, this.Format(divisionName));
            }
            return _stringBuilder;
        }

        private string Format(string value)
        {
            return value.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private void StackErrorCell(string value = "")
        {
            //if (_errorStackBuilder.Length == 0)
            //{
            //    _errorStackBuilder.AppendFormat("<tr class=\"{0}\">", ERRORxCLASS);
            //}

            if (value.Length > 0 && !_hasError)
            {
                _hasError = true;
            }
            _errorStackBuilder.AppendFormat("<td class=\"{0}\">{1}</td>", ERRORxMSGxCLASS, this.Format(value));
        }

        private void StackWarningCell(string value = "")
        {
            //if (_warningStackBuikder.Length == 0)
            //{
            //    _warningStackBuikder.AppendFormat("<tr class=\"{0}\">", WARNINGxCLASS);
            //}

            if (value.Length > 0 && !_hasWarning)
            {
                _hasWarning = true;
            }
            _warningStackBuikder.AppendFormat("<td class=\"{0}\">{1}</td>", WARNINGxMSGxCLASS, this.Format(value));
        }

        public StringBuilder AddCell(string value, string[] error, string[] warning)
        {
            if (error != null && warning != null)
            {
                this.AddErrorWarningCell(value);
                this.StackErrorCell(string.Join("\n", error));
                this.StackWarningCell(string.Join("\n", warning));
            }
            else if (error != null)
            {
                this.AddErrorCell(value);
                this.StackErrorCell(string.Join("\n", error));
                this.StackWarningCell();
            }
            else if (warning != null)
            {
                this.AddWarningCell(value);
                this.StackErrorCell();
                this.StackWarningCell(string.Join("\n", warning));
            }
            else
            {
                this.AddCell(value);
                this.StackErrorCell();
                this.StackWarningCell();
            }

            _columnCount += 1;

            return _stringBuilder;
        }

        private StringBuilder AddCell(string value)
        {
            return _stringBuilder.AppendFormat("<td>{0}</td>", this.Format(value));
        }

        private StringBuilder AddErrorCell(string value)
        {
            return _stringBuilder.AppendFormat("<td class=\"{0}\">{1}</td>", ERRORxCLASS, this.Format(value));
        }

        private StringBuilder AddWarningCell(string value)
        {
            return _stringBuilder.AppendFormat("<td class=\"{0}\">{1}</td>", WARNINGxCLASS, this.Format(value));
        }

        private StringBuilder AddErrorWarningCell(string value)
        {
            return _stringBuilder.AppendFormat("<td class=\"{0}\" >{1}</td>", ERRORxWARNINGxCLASS, this.Format(value));
        }

        public String ToString()
        {
            return _stringBuilder.ToString();
        }
    }

    /// <summary>
    /// チェック結果を表すのビューアー
    /// </summary>
    public class Viewer
    {
        private List<CheckResult> _result = null;

        public Viewer(CheckResult result)
        {
            _result = new List<CheckResult>();
            _result.Add(result);
        }

        public bool Any()
        {
            if (_result != null)
            {
                foreach (var item in _result)
                {
                    if (item.Any())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void BuildRows(ResultTableBuilder tableBuilder, CheckResult result)
        {
            if (result.Content == null)
            {
                return;
            }

            for (int i = 0; i < result.Content.Count; i++)
            {
                var rowErrorList = (from e in result.ErrorList where e.Key.Row == i select e).ToList();
                var rowWarningList = (from w in result.WarningList where w.Key.Row == i select w).ToList();

                if (rowErrorList.Count == 0 && rowWarningList.Count == 0) continue;

                tableBuilder.BeginRow();

                var row = result.Content[i];

                for (int j = 0; j < row.Length; j++)
                {
                    var cellError = (from e in rowErrorList where e.Key.Col == j select e.Value).FirstOrDefault();
                    var cellWarning = (from w in rowWarningList where w.Key.Col == j select w.Value).FirstOrDefault();

                    tableBuilder.AddCell(row[j], cellError == null ? null : cellError.ToArray(), cellWarning == null ? null : cellWarning.ToArray());
                }

                var rowError = (from e in rowErrorList where e.Key.Col == -1 select e.Value).FirstOrDefault();
                var rowWarning = (from w in rowWarningList where w.Key.Col == -1 select w.Value).FirstOrDefault();

                tableBuilder.EndRow(rowError == null ? null : rowError.ToArray(), rowWarning == null ? null : rowWarning.ToArray());
            }
        }

        public static Viewer CreateUnioned(params CheckResult[] results)
        {
            Viewer resultViewer = null;
            foreach (var result in results)
            {
                if (result == null)
                {
                    continue;
                }

                if (resultViewer == null)
                {
                    resultViewer = new Viewer(result);
                }
                else
                {
                    resultViewer.Union(result);
                }
            }

            return resultViewer;
        }

        public string ToTable(bool skipEmpty=true, List<string> header = null)
        {
            // Todo: <jackrole> csv error/waring are not output to view result. add them in the future.

            ResultTableBuilder tableString = new ResultTableBuilder();

            tableString.BeginTable();

            foreach (var result in _result)
            {
                if (skipEmpty && !result.Any())
                {
                    continue;
                }

                Debug.Assert(result.Content != null && result.Content.Count > 0, "");
                var colSpan = result.Content[0].Length;

                // add a division row with a name if possible.
                if (_result.IndexOf(result) != 0)
                {
                    tableString.InsertDivisionRow(result.Name, colSpan);
                }
                else if (result.Name != null)
                {
                    tableString.InsertDivisionRow(result.Name, colSpan);
                }

                if (header != null)
                {
                    tableString.BeginRow();
                    header.ForEach(x => tableString.AddCell(x, null, null));
                    tableString.EndRow(null, null);
                }

                this.BuildRows(tableString, result);
            } 

            tableString.EndTable();

            return tableString.ToString();
        }

        public void Union(params CheckResult[] results)
        {
            _result.AddRange(results);
        }

        public void Union(params Viewer[] viewers)
        {
            foreach (var item in viewers)
            {
                _result.AddRange(item._result);
            }
        }
    }
}
