using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aviadispetcher
{
    public class SelectData
    {
        public List<Flight> selectedCityList = new List<Flight>(85);
        public List<Flight> selectedCityTimeList = new List<Flight>(85);
        Microsoft.Office.Interop.Word.Application wordApp;
        Microsoft.Office.Interop.Word.Document wordDoc;
        string filePath;

        public SelectData() { }
       
        public void SelextX(string cityX = " ")
        {
            int j = 0;
            for (int i = 0; i < MainWindow.flightCount; i++)
            {
                if (cityX == MainWindow.fList[i].city)
                {
                    selectedCityList.Add(MainWindow.fList[i]);
                    j++;
                }
            }
        }
        public void SelectXY(TimeSpan deadLine)
        {
            int j = 0;
            for (int i = 0; i < selectedCityList.Count; i++)
            {
                if (deadLine.Hours > selectedCityList[i].depature_time.Hours)
                {
                    selectedCityTimeList.Add(selectedCityList[i]);
                    j++;
                }
                if ((deadLine.Hours == selectedCityList[i].depature_time.Hours) &&
                    (deadLine.Minutes >= selectedCityList[i].depature_time.Minutes))
                {
                    selectedCityTimeList.Add(selectedCityList[i]);
                    j++;
                }             
            }           
        }
        public void WriteData(List<Flight> selXList, List<Flight> SelXYList)
        {
            filePath = Environment.CurrentDirectory.ToString();
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                wordDoc = wordApp.Documents.Add(filePath + "\\Шаблон_Пошуку_рейсів.dot");
                wordApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + char.ConvertFromUtf32(13) + "Помістіть файл Шаблон_Пошуку_рейсів.dot " + char.ConvertFromUtf32(13) + 
                    "у каталог із exe-файлом і повторіть збереження", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ReplaceText(MainWindow.selectedCity, "[X]");
            ReplaceText(selectedCityList, 1);

            ReplaceText(MainWindow.timeFlight.ToString(@"hh\:mm"), "[Y]");
            ReplaceText(SelXYList, 2);
            try
            {
                wordDoc.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + char.ConvertFromUtf32(13) + "Помилка відкриття даних " + char.ConvertFromUtf32(13),
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }        
        }
        private void ReplaceText(string textToReplace, string replacedText)
        {
            Object missing = Type.Missing;

            Microsoft.Office.Interop.Word.Range selText;
            selText = wordDoc.Range(wordDoc.Content.Start, wordDoc.Content.End);

            Microsoft.Office.Interop.Word.Find find = wordApp.Selection.Find;
            find.Text = replacedText;
            find.Replacement.Text = textToReplace;

            Object wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindContinue;

            Object replace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;

            find.Execute(Type.Missing, false, false, false, missing, false, true, wrap, false, missing, replace);
        }
        private void ReplaceText(List<Flight> selectedList, int numTable)
        {
            for (int i = 0; i < selectedList.Count; i++)
            {
                wordDoc.Tables[numTable].Rows.Add();
                wordDoc.Tables[numTable].Cell(2 + i, 1).Range.Text = selectedList[i].number;
                wordDoc.Tables[numTable].Cell(2 + i, 2).Range.Text = selectedList[i].depature_time.ToString();
                if (numTable == 2)
                {
                    wordDoc.Tables[numTable].Cell(2 + i, 3).Range.Text = selectedList[i].free_seats.ToString();
                }
            }
        }
        ~SelectData()
        {
            if (wordDoc != null)
            {
                wordDoc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdPromptToSaveChanges);
            }
            if (wordApp != null)
            {
                wordApp.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdPromptToSaveChanges);
            }
        }
    }
}
