using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using WpfApp4;

namespace WpfApp4.Tests
{
    [TestClass]
    public class MainWindowTests
    {
        private DataTable GetTestData()
        {
            DataTable testData = new DataTable();
            testData.Columns.Add("id", typeof(int));
            testData.Columns.Add("name", typeof(string));
            testData.Columns.Add("price", typeof(float));
            testData.Columns.Add("amount", typeof(int));
            testData.Columns.Add("ageRange", typeof(string));

            testData.Rows.Add(1, "Лялька", 150.5f, 10, "3-5");
            testData.Rows.Add(2, "Конструктор", 500f, 5, "7-12");
            testData.Rows.Add(3, "М'яч", 50f, 20, "2-99");
            testData.Rows.Add(4, "Машинка", 200f, 15, "5-8");
            return testData;
        }

        [TestMethod]
        public void FilterToysByAge_CorrectlyFiltersToys()
        {
            DataTable testData = GetTestData();
            int searchMin = 4;
            int searchMax = 6;
            int expectedCount = 3;

            DataTable actualResult = MainWindow.filterByAge(testData, searchMin, searchMax);

            Assert.AreEqual(expectedCount, actualResult.Rows.Count);
        }

        [TestMethod]
        public void FilterToysByAge_NoResultsFound()
        {
            DataTable testData = GetTestData();
            int searchMin = 100;
            int searchMax = 120;
            int expectedCount = 0;

            DataTable actualResult = MainWindow.filterByAge(testData, searchMin, searchMax);

            Assert.AreEqual(expectedCount, actualResult.Rows.Count);
        }

        [TestMethod]
        public void FindCheapestToy_FindsCheapestToyCorrectly()
        {
            DataTable testData = GetTestData();
            float expectedPrice = 50f;
            string expectedName = "М'яч";

            var result = MainWindow.FindCheapestToy(testData);

            Assert.AreEqual(expectedPrice, result.Price);
            Assert.AreEqual(expectedName, result.Name);
        }
    }
}
