using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connStr = "server=localhost; user=root; password=sd000615; database=igrashky; port=3306;";
        bool isEditMode = false;
        int selectedToyId = -1;
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM igrashky", connStr))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    MainDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження даних: " + ex.Message);
            }
        }

        // --- РЕДАГУВАННЯ ---
        private void BtnEdit_Click_1(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.SelectedItem is DataRowView row)
            {
                isEditMode = true;
                selectedToyId = (int)row["id"];
                TxtName.Text = row["name"].ToString();
                TxtPrice.Text = row["price"].ToString();
                TxtAmount.Text = row["amount"].ToString();
                TxtAge.Text = row["ageRange"].ToString();

                PanelManage.Visibility = Visibility.Visible;
                TxtManageTitle.Text = "Редагування запису";
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть іграшку для редагування.");
            }
        }

        private void MenuLogin_Click(object sender, RoutedEventArgs e)
        {
            Window1 authForm = new Window1();

            // ShowDialog зупиняє виконання коду тут, поки Window1 не закриється
            if (authForm.ShowDialog() == true)
            {
                // Якщо повернулося true, вмикаємо режим адміністратора
                EnableAdminMode();
                MessageBox.Show("Ви успішно увійшли як робітник магазину.", "Авторизація", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuLogout_Click(object sender, RoutedEventArgs e)
        {
            DisableAdminMode();
            MessageBox.Show("Ви вийшли з системи управління.", "Вихід", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EnableAdminMode()
        {

            MenuLogin.Visibility = Visibility.Collapsed;
            MenuSearch.Visibility = Visibility.Collapsed;
            PanelSearch.Visibility = Visibility.Collapsed;


            MenuLogout.Visibility = Visibility.Visible;
            MenuManage.Visibility = Visibility.Visible;
            BtnAdd.Visibility = Visibility.Visible;
            BtnDelete.Visibility = Visibility.Visible;
            BtnEdit.Visibility = Visibility.Visible;
        }


        private void DisableAdminMode()
        {

            MenuLogout.Visibility = Visibility.Collapsed;
            MenuManage.Visibility = Visibility.Collapsed;
            BtnAdd.Visibility = Visibility.Collapsed;
            BtnDelete.Visibility = Visibility.Collapsed;
            BtnEdit.Visibility = Visibility.Collapsed;
            PanelManage.Visibility = Visibility.Collapsed; 

            MenuLogin.Visibility = Visibility.Visible;
            MenuSearch.Visibility = Visibility.Visible;
        }


    }
}