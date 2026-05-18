using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Xceed.Document.NET;
using Xceed.Words.NET;

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
        private DataTable searchResult = null;
        float minPrice = 0;
        string cheapestToyName = "";
        string searchX = "";
        string searchY = "";
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

        // РЕДАГУВАННЯ 
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

        // ЗБЕРЕЖЕННЯ 
        private void BtnSave_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = isEditMode
                        ? "UPDATE igrashky SET name=@n, price=@p, amount=@a, ageRange=@age WHERE id=@id"
                        : "INSERT INTO igrashky (name, price, amount, ageRange) VALUES (@n, @p, @a, @age)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@n", TxtName.Text);
                    cmd.Parameters.AddWithValue("@p", TxtPrice.Text);
                    cmd.Parameters.AddWithValue("@a", TxtAmount.Text);
                    cmd.Parameters.AddWithValue("@age", TxtAge.Text);
                    if (isEditMode) cmd.Parameters.AddWithValue("@id", selectedToyId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show(isEditMode ? "Запис оновлено!" : "Іграшку додано!");

                    PanelManage.Visibility = Visibility.Collapsed;
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка збереження: " + ex.Message);
            }
        }

        private void MenuLogin_Click(object sender, RoutedEventArgs e)
        {
            Window1 authForm = new Window1();

            if (authForm.ShowDialog() == true)
            {
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

        // ДОДАВАННЯ
        private void BtnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.Items.Count >= 150)
            {
                MessageBox.Show("Досягнуто ліміт асортименту (150 найменувань)!", "Обмеження");
                return;
            }
            ClearFields();
            isEditMode = false;
            PanelManage.Visibility = Visibility.Visible;
            TxtManageTitle.Text = "Додавання іграшки";
        }

        // ВИДАЛЕННЯ 
        private void BtnDelete_Click_1(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.SelectedItem is DataRowView row)
            {
                if (MessageBox.Show("Видалити цей запис?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(connStr))
                        {
                            conn.Open();
                            MySqlCommand cmd = new MySqlCommand("DELETE FROM igrashky WHERE id=@id", conn);
                            cmd.Parameters.AddWithValue("@id", row["id"]);
                            cmd.ExecuteNonQuery();
                            LoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Помилка видалення: " + ex.Message);
                    }
                }
            }
        }

        private void ClearFields()
        {
            TxtName.Clear(); TxtPrice.Clear(); TxtAmount.Clear(); TxtAge.Clear();
        }

        // ПОШУК
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtSearchX.Text, out int searchX) || !int.TryParse(TxtSearchY.Text, out int searchY))
            {
                MessageBox.Show("Будь ласка, введіть коректні числові значення для віку.");
                return;
            }

            if (searchX > searchY)
            {
                MessageBox.Show("Вік 'ВІД' не може бути більшим за вік 'ДО'.");
                return;
            }

            this.searchX = TxtSearchX.Text;
            this.searchY = TxtSearchY.Text;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM igrashky", conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);


                    DataTable filteredTable = dt.Clone();


                    minPrice = float.MaxValue;


                    foreach (DataRow row in dt.Rows)
                    {
                        string ageRangeStr = row["ageRange"].ToString();
                        string[] parts = ageRangeStr.Split('-');

                        if (parts.Length == 2 && int.TryParse(parts[0], out int toyMinAge) && int.TryParse(parts[1], out int toyMaxAge))
                        {

                            if (toyMinAge <= searchY && toyMaxAge >= searchX)
                            {

                                filteredTable.ImportRow(row);

                                float currentPrice = Convert.ToSingle(row["price"]);
                                if (currentPrice < minPrice)
                                {
                                    minPrice = currentPrice;
                                    cheapestToyName = row["name"].ToString();
                                }
                            }
                        }
                    }


                    MainDataGrid.ItemsSource = filteredTable.DefaultView;
                    searchResult = filteredTable;

                    if (filteredTable.Rows.Count > 0)
                    {
                        MessageBox.Show($"Знайдено іграшок: {filteredTable.Rows.Count}.\nНайдешевша: {cheapestToyName} ({minPrice} грн).", "Результат пошуку");
                        BtnExportWord.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("За вашим запитом нічого не знайдено.");
                        BtnExportWord.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка пошуку: " + ex.Message);
            }
        }

        private void MenuSearchByAge_Click(object sender, RoutedEventArgs e)
        {
            PanelSearch.Visibility = Visibility.Visible;
        }

        private void BtnAbandon_Click(object sender, RoutedEventArgs e)
        {
            PanelSearch.Visibility = Visibility.Collapsed;
            BtnExportWord.Visibility = Visibility.Collapsed;

            TxtSearchX.Clear();
            TxtSearchY.Clear();
            searchResult = null;

            LoadData();
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            if (searchResult == null || searchResult.Rows.Count == 0)
            {
                MessageBox.Show("Пошук не був виконаний або результат був порожнім.Будь ласка, виконайте пошук", "Помилка");
                return;
            }

            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Звіт_Іграшки.docx");

                using (DocX doc = DocX.Create(filePath))
                {
                    var title = doc.InsertParagraph($"Результати пошуку іграшок для віку: Від {searchX} до {searchY} років");
                    title.Font("Times New Roman").FontSize(14).Bold().Alignment = Alignment.center;
                    doc.InsertParagraph("\n");

                    doc.InsertParagraph("Таблиця 1 - Знайдені іграшки").Font("Times New Roman").FontSize(12);

                    var table1 = doc.AddTable(searchResult.Rows.Count + 1, 4);
                    table1.Design = TableDesign.TableGrid; 

                    table1.Rows[0].Cells[0].Paragraphs[0].Append("Назва іграшки").Bold();
                    table1.Rows[0].Cells[1].Paragraphs[0].Append("Ціна (грн)").Bold();
                    table1.Rows[0].Cells[2].Paragraphs[0].Append("Кількість").Bold();
                    table1.Rows[0].Cells[3].Paragraphs[0].Append("Вікові межі").Bold();

                    int r = 1;
                    foreach (DataRow row in searchResult.Rows)
                    {
                        table1.Rows[r].Cells[0].Paragraphs[0].Append(row["name"].ToString());
                        table1.Rows[r].Cells[1].Paragraphs[0].Append(row["price"].ToString());
                        table1.Rows[r].Cells[2].Paragraphs[0].Append(row["amount"].ToString());
                        table1.Rows[r].Cells[3].Paragraphs[0].Append(row["ageRange"].ToString());
                        r++;
                    }
                    doc.InsertTable(table1);
                    doc.InsertParagraph("\n");

                    doc.InsertParagraph("Таблиця 2 - Найдешевша іграшка з асортименту знайдених").Font("Times New Roman").FontSize(12);

                    var table2 = doc.AddTable(2, 2);
                    table2.Design = TableDesign.TableGrid;
                    table2.Rows[0].Cells[0].Paragraphs[0].Append("Назва найдешевшої іграшки").Bold();
                    table2.Rows[0].Cells[1].Paragraphs[0].Append("Ціна (грн)").Bold();

                    table2.Rows[1].Cells[0].Paragraphs[0].Append(cheapestToyName);
                    table2.Rows[1].Cells[1].Paragraphs[0].Append(minPrice.ToString());

                    doc.InsertTable(table2);

                    doc.Save();
                }

                MessageBox.Show("Файл 'Звіт_Іграшки.docx' успішно збережено на Робочий стіл!", "Успіх");

                _ = Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка генерації файлу: " + ex.Message, "Помилка");
            }
        }
    }
}