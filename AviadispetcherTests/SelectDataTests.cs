using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aviadispetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio;

namespace Aviadispetcher.Tests
{
    [TestClass()]
    public class SelectDataTests
    {
        [TestMethod()]
        public void SelextXTest()
        {
            List<Flight> expected = new List<Flight>(85);
            expected.Add(new Flight(1, "КА-123", "Київ", TimeSpan.Parse("21:25:00"), 80));
            expected.Add(new Flight(7, "КА-199", "Київ", TimeSpan.Parse("00:55:00"), 6));

            var target = new MainWindow();
            object sender = target;
            RoutedEventArgs e = null;
            target.InitializeComponent();
            target.InfoFlightForm_Loaded(sender, e);

            target.SelectXMenuItem_Click(sender, e);

            string cityX = "Київ";
            SelectData selXytest = new SelectData();
            selXytest.SelextX(cityX);

            List<Flight> actual = selXytest.selectedCityList;

            for (int i = 0; i < 2; i++)
            {
                Assert.AreEqual(expected[i].id, actual[i].id);
                Assert.AreEqual(expected[i].number, actual[i].number);
                Assert.AreEqual(expected[i].city, actual[i].city);
                Assert.AreEqual(expected[i].depature_time, actual[i].depature_time);
                Assert.AreEqual(expected[i].free_seats, actual[i].free_seats);
            }

        }

        [TestMethod()]
        [DeploymentItem("Aviadispetcher.exe")]
        public void SelectXYTest()
        {
            List<Flight> expected = new List<Flight>(85);

            expected.Add(new Flight(4, "КВ-834", "Відень", TimeSpan.Parse("13:40:00"), 45));

            var target = new MainWindow();

            object sender = target;
            RoutedEventArgs e = null;

            target.InitializeComponent();
            target.InfoFlightForm_Loaded(sender, e);
            target.SelectXYMenuItem_Click(sender, e);

            string cityX = "Відень";
            TimeSpan deadLine;
            TimeSpan.TryParse("14:00", out deadLine);

            SelectData selXYtest = new SelectData();
            selXYtest.SelextX(cityX);
            selXYtest.SelectXY(deadLine);
            List<Flight> actual;

            actual = selXYtest.selectedCityTimeList;
            
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(expected[i].id, actual[i].id);
                Assert.AreEqual(expected[i].number, actual[i].number);
                Assert.AreEqual(expected[i].city, actual[i].city);
                Assert.AreEqual(expected[i].depature_time, actual[i].depature_time);
                Assert.AreEqual(expected[i].free_seats, actual[i].free_seats);
            }
        }
    }
}