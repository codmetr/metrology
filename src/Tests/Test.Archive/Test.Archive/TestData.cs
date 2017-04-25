using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Archive
{
    public class TestData
    {
        public static CheckSimple GetTestData(int res, List<int> resItems, int res2, List<int> resItems2)
        {
            var resData = new List<ResultSimple>()
            {
                new ResultSimple() {PointRes = res, Points = resItems,},
                new ResultSimple() {PointRes = res2, Points = resItems2,}
            };
            var data = new CheckSimple() { TestResult = new TestSimple() { Result = resData } };
            return data;
        }

        /// <summary>
        /// Тестовый класс - корень/сессия/проверка
        /// </summary>
        public class CheckSimple
        {
            private TestSimple _testResult = new TestSimple();

            public TestSimple TestResult
            {
                get { return _testResult; }
                set { _testResult = value; }
            }
        }

        /// <summary>
        /// Тестовый класс - тест/тех.карта
        /// </summary>
        public class TestSimple
        {
            private List<ResultSimple> _result = new List<ResultSimple>();

            public List<ResultSimple> Result
            {
                get { return _result; }
                set { _result = value; }
            }
        }

        /// <summary>
        /// Тестовый класс - результат
        /// </summary>
        public class ResultSimple
        {
            private int _pointRes = 777;
            private List<int> _points = new List<int> { 1, 2, 3 };

            public int PointRes
            {
                get { return _pointRes; }
                set { _pointRes = value; }
            }

            public List<int> Points
            {
                get { return _points; }
                set { _points = value; }
            }
        }
    }
}
