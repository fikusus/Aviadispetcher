using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aviadispetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aviadispetcher.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        [DeploymentItem("Aviadispetcher.exe")]
        public void FillCityListTest()
        {
            var target = new MainWindow();

            object sender = target;
            RoutedEventArgs e = null;
            
            target.InfoFlightForm_Loaded(sender, e);
            target.FillCityList();

            string[] expected = new string[8] { "Київ", "Лондон", "Париж", "Відень", "Мюнхен", "Берлін", "Київ", "Мюнхен"};
            string[] actual = new string[MainWindow.fList.Count];
            
            for (int i = 0; i < MainWindow.fList.Count; i++)
            {
                actual[i] = MainWindow.fList[i].city;
            }
            for (int i = 0; i < 8; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}