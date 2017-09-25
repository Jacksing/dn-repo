using System;
using System.IO;

namespace JTests
{
    public class CustomTestsDataHelper
    {
        private string _workPath = "";

        public string RootVariable { get; private set; } = "TESTS_DATA";

        public CustomTestsDataHelper(string workPath)
        {
            _workPath = workPath;
        }

        public CustomTestsDataHelper() { }

        public static CustomTestsDataHelper CreateTestsDataHelper(string workPath, string rootVariable)
        {
            return new CustomTestsDataHelper(workPath) { RootVariable = rootVariable };
        }

        public static CustomTestsDataHelper CreateTestsDataHelper(string workPath)
        {
            return new CustomTestsDataHelper(workPath);
        }

        public string GetFullPath(string fileName)
        {
            string fullPath = Path.Combine(
                Environment.GetEnvironmentVariable(RootVariable),
                _workPath,
                fileName
            );
            if (File.Exists(fullPath)) return fullPath;
            return TryFindFile(fullPath);
        }

        private string TryFindFile(string targetFullPath)
        {
            var targetName = Path.GetFileName(targetFullPath);
            foreach (var fp in Directory.GetFiles(Path.GetDirectoryName(targetFullPath)))
            {
                if (Path.GetFileNameWithoutExtension(fp) == targetName)
                    return fp;
            }
            throw new FileNotFoundException(nameof(targetFullPath));
        }

        public string ReadTestData(string fileName)
        {
            return File.ReadAllText(GetFullPath(fileName));
        }

        public void SetRootVariable(string value) => RootVariable = value;
    }

    public static class TestsDataHelper
    {
        private static CustomTestsDataHelper That;
        static TestsDataHelper() => That = new CustomTestsDataHelper();

        public static string GetFullPath(string fileName) => That.GetFullPath(fileName);

        public static string ReadTestData(string fileName) => That.ReadTestData(fileName);
    }
}
