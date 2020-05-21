using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Aviadispetcher
{
    public partial class MainWindow : Window
    {
        public string connStr;
        public static List<Flight> fList = new List<Flight>(85);
        int flightNum;
        bool flightAdd = false;
        public static int flightCount;
        public static SelectData selXY;
        public static string selectedCity;
        public static TimeSpan timeFlight;
        public static Authorization logedUser = new Authorization();
        public MainWindow()
        {
            InitializeComponent();
        }
        public void OpenDbFile()
        {
            try
            {
                connStr = "Server =teplo23.mysql.ukraine.com.ua; Database = teplo23_avia2020; Uid = teplo23_avia2020; Pwd = )f9tuZM92&;";
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand command = new MySqlCommand();
                string commandString = "SELECT * FROM rozklad;";
                command.CommandText = commandString;
                command.Connection = conn;
                MySqlDataReader reader;
                command.Connection.Open();
                reader = command.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    fList.Add(new Flight((int)reader["id"], (string)reader["number"], (string)reader["city"],
                        (System.TimeSpan)reader["depature_time"], (int)reader["free_seats"]));
                    i++;
                }
                reader.Close();
                FlightListDG.ItemsSource = fList;
            }
            catch (Exception ex)
            {
                // TODO: Implement throw new Exceprion with needed message
                MessageBox.Show(ex.Message+char.ConvertFromUtf32(13)+char.ConvertFromUtf32(13)+"Для завантаження файлу "+
                    "виконайте команду Файл-Завантажити", "Помилку", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResizeForm(double FormWidth)
        {
            MainGrid.Width = FormWidth;
        }

        public void InfoFlightForm_Loaded(object sender, RoutedEventArgs e)
        {
            OpenDbFile();
            ResizeForm(FlightListDG.Margin.Left + FlightListDG.RenderSize.Width + 20);
            selFlightGroupBox.Visibility = Visibility.Hidden;
            flightGroupBox.Visibility = Visibility.Hidden;

            FlightMenuItem.Visibility = Visibility.Hidden;
            FlightMenuItem.Width = 0;
        }

        private void LoadDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FlightListDG.ItemsSource = null;
                fList.Clear();
            }
            catch (Exception ex)
            {
                ErrorShow(ex);
            }
            OpenDbFile();
        }

        public static void ErrorShow(Exception ex)
        {
            MessageBox.Show(ex.Message + char.ConvertFromUtf32(13), "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void EditDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            flightGroupBox.Visibility = Visibility.Visible;
            ResizeForm(flightGroupBox.Margin.Left + flightGroupBox.ActualWidth + 420);
            
            flightAdd = false;
        }
        private void ChangeFlightListData(int num)
        {
            TimeSpan depTime;
            if (flightAdd)
            {
                fList.Add(new Flight(fList.Count, "", "", TimeSpan.Zero, 0));
                num = fList.Count - 1;  
            }
            fList[num].number = numFlightTextBox.Text;
            fList[num].city = cityFlightTextBox.Text;
            
            if (TimeSpan.TryParse(timeFlightTextBox.Text, out depTime))
            {
                fList[num].depature_time = depTime;
            }
            fList[num].free_seats = Convert.ToInt16(freeSeatsTextBox.Text);
            FlightListDG.ItemsSource = null;
            FlightListDG.ItemsSource = fList;
            string errMsg;

            if (flightAdd)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO rozklad (number, city, depature_time, free_seats) VALUES (?,?,?,?)", conn))
                    {
                        cmd.Parameters.Add("@number", MySqlDbType.VarChar, 6).Value = numFlightTextBox.Text;
                        cmd.Parameters.Add("@city", MySqlDbType.VarChar, 25).Value = cityFlightTextBox.Text;
                        if (TimeSpan.TryParse(timeFlightTextBox.Text, out depTime))
                        {
                            cmd.Parameters.Add("@depature_time", MySqlDbType.Time).Value = depTime;
                        }
                        cmd.Parameters.Add("@free_seats", MySqlDbType.Int16, 4).Value = Convert.ToInt16(freeSeatsTextBox.Text);
                        cmd.Parameters.Add("@id", MySqlDbType.Int32, 11).Value = fList[num].id;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Unable to connect to any of the specified MySQL hosts.")
                    {
                        errMsg = "Підключіть веб-сервер MySQL та завантажте дані командою Файл-Завантажити";
                    }
                    else
                    {
                        errMsg = "Для завантаження даних виконайте команду Файл-Завантажити";
                    }
                    MessageBox.Show(ex.Message + char.ConvertFromUtf32(13) + char.ConvertFromUtf32(13) + errMsg, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }else
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE rozklad SET number = ?, city = ?, depature_time = ?, free_seats = ? WHERE id = ?", conn))
                    {
                        cmd.Parameters.Add("@number", MySqlDbType.VarChar, 6).Value = numFlightTextBox.Text;
                        cmd.Parameters.Add("@city", MySqlDbType.VarChar, 25).Value = cityFlightTextBox.Text;
                        if (TimeSpan.TryParse(timeFlightTextBox.Text, out depTime))
                        {
                            cmd.Parameters.Add("@depature_time", MySqlDbType.Time).Value = depTime;
                        }
                        cmd.Parameters.Add("@free_seats", MySqlDbType.Int16, 4).Value = Convert.ToInt16(freeSeatsTextBox.Text);
                        cmd.Parameters.Add("@id", MySqlDbType.Int32, 11).Value = fList[num].id;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Unable to connect to any of the specified MySQL hosts.")
                    {
                        errMsg = "Підключіть веб-сервер MySQL та завантажте дані командою Файл-Завантажити";
                    }
                    else
                    {
                        errMsg = "Для завантаження даних виконайте команду Файл-Завантажити";
                    }
                    MessageBox.Show(ex.Message + char.ConvertFromUtf32(13) + char.ConvertFromUtf32(13) + errMsg, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void FlightListDG_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Flight editedFlight = FlightListDG.SelectedItem as Flight;
            try
            {
                numFlightTextBox.Text = editedFlight.number;
                cityFlightTextBox.Text = editedFlight.city;
                timeFlightTextBox.Text = editedFlight.depature_time.ToString(@"hh\:mm");
                freeSeatsTextBox.Text = editedFlight.free_seats.ToString();
            }
            catch (Exception ex)
            {
                ErrorShow(ex);
            }
            flightNum = FlightListDG.SelectedIndex;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeFlightListData(flightNum);
        }

        private void AddDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            if (fList.Count <= 85)
            {
                flightAdd = true;
                flightGroupBox.Visibility = Visibility.Visible;
                ResizeForm(flightGroupBox.Margin.Left + flightGroupBox.RenderSize.Width + 420);
                flightNum = fList.Count;
            }
            else
            {
                flightAdd = false;
                MessageBox.Show(char.ConvertFromUtf32(13)+"Кількість рейсів не може бути більше 85!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }      
        }

        public void FillCityList()
        {
            flightCount = 0;
            for (int i = 0; i < fList.Count; i++)
            {
                if (fList[i].city != null)
                {
                    if (fList[i].city != " ")
                    {
                        flightCount++;
                    }
                }
            }
            bool nameExist = false;
            cityList.Items.Add(fList[0].city);

            for (int i = 1; i < flightCount; i++)
            {
                for (int j = 0; j < cityList.Items.Count; j++)
                {
                    if (cityList.Items[j].ToString() == fList[i].city)
                    {
                        nameExist = true;
                    }
                }
                if (!nameExist)
                {
                    cityList.Items.Add(fList[i].city);
                }
                nameExist = false;
            }
        }

        public void SelectXMenuItem_Click(object sender, RoutedEventArgs e)
        {
            selFlightGroupBox.Visibility = Visibility.Visible;
            timeFlightLabel.Visibility = Visibility.Visible;
            sTime.Visibility = Visibility.Hidden;
            timeFlightLabelY.Visibility = Visibility.Hidden;
            ResizeForm(flightGroupBox.Margin.Left + flightGroupBox.RenderSize.Width + 420);
            cityList.Items.Clear();

            FillCityList();
        }

        private void SelBtn_Click(object sender, RoutedEventArgs e)
        {
            selXY = new SelectData();
            selectedCity = "";
            selectedCity = Convert.ToString(cityList.Items[cityList.SelectedIndex]);
            selXY.SelextX(selectedCity);

            for (int i = 0; i < selXY.selectedCityList.Count; i++)
            {
                if (selXY.selectedCityList[i].number != null)
                    FlightListDG.ItemsSource = selXY.selectedCityList;
            }

            TimeSpan.TryParse(sTime.Text, out timeFlight);
            selXY.SelectXY(timeFlight);

            for (int i = 0; i <= selXY.selectedCityTimeList.Count; i++)
            {
                FlightListDG.ItemsSource = null;
                if (timeFlightLabelY.Visibility == Visibility.Hidden)
                {
                    FlightListDG.ItemsSource = selXY.selectedCityList;
                }
                else
                {
                    FlightListDG.ItemsSource = selXY.selectedCityTimeList;
                }
            }
            if (selXY.selectedCityTimeList.Count == 0 && timeFlightLabelY.Visibility == Visibility.Visible)
            {
                FlightListDG.ItemsSource = null;
            }

        }

        public void SelectXYMenuItem_Click(object sender, RoutedEventArgs e)
        {

            selFlightGroupBox.Visibility = Visibility.Visible;
            timeFlightLabel.Visibility = Visibility.Visible;
            sTime.Visibility = Visibility.Visible;
            timeFlightLabelY.Visibility = Visibility.Visible;
            ResizeForm(flightGroupBox.Margin.Left + flightGroupBox.RenderSize.Width + 420);
            cityList.Items.Clear();
            FillCityList();

            MessageBox.Show("Для відбору рейсів не пізнаше вказаного часу потрібно " + 
                "також вказати місто прильоту", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        private void SaveSelBtn_Click(object sender, RoutedEventArgs e)
        {
            selXY.WriteData(selXY.selectedCityList, selXY.selectedCityTimeList);
        }

        private void SaveDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if ((selFlightGroupBox.Visibility == Visibility.Visible) && 
                (selXY.selectedCityList.Count > 0))
            {
                selXY.WriteData(selXY.selectedCityList, selXY.selectedCityTimeList);
            }
            if (flightGroupBox.Visibility == Visibility.Visible)
            {
                if ((FlightListDG.SelectedIndex < 0) && (!flightAdd))
                {
                    MessageBox.Show("Оберіть у списку рейс для редагування подвійним кліком", "Увага", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }else
                {
                    ChangeFlightListData(flightNum);
                }
            }
        }

        private void AutorizationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LogInForm logWnd = new LogInForm();
            logWnd.Show();
            this.Visibility = Visibility.Collapsed;
        }

        private void InfoFlightForm_Activated(object sender, EventArgs e)
        {
            if (Authorization.logUser == 2)
            {
                FlightMenuItem.Visibility = Visibility.Visible;
                FlightMenuItem.Width = 50;
            }else
            {
                FlightMenuItem.Visibility = Visibility.Hidden;
                FlightMenuItem.Width = 0;
            }
        }
    }
}
