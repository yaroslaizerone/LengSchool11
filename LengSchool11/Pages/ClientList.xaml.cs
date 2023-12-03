using LengSchool11.Classes;
using LengSchool11.Entity;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LengSchool11.Windows;

namespace LengSchool11.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientList.xaml
    /// </summary>
    public partial class ClientList : Page
    {

        private int page = 0;
        private int count = 0;
        string fnd = "";
        SchoolEntities db = Helper.GetContext();
        public ClientList()
        {
            InitializeComponent();
            sortByCount.ItemsSource = CountList;
            sortByGender.ItemsSource = FilterList;
            sortByParametrs.ItemsSource = SortingList;
            Load();
        }

        public string[] FilterList { get; set; } =
        {
            "Все",
            "Мужчины",
            "Женщины"
        };

        public string[] CountList { get; set; } =
        {
            "Все",
            "10",
            "50",
            "200"
        };

        public string[] SortingList { get; set; } =
        {
            "Без сортировки",
            "По фамилии (Возрастание)",
            "По фамилии (Убывание)",
            "По дате последнего посещения (Возрастание)",
            "По дате последнего посещения (Убывание)",
            "По количеству посещений (Возрастание)",
            "По количеству посещений (Убывание)"
        };

        // Метод для проверки, открыто ли окно указанного типа
        public bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        public void Load()
        {
            try
            {
                var client = db.Client.ToList();
                AllCountRow.Text = client.Count().ToString();
                if (sortByParametrs.SelectedIndex == 1) client = client.OrderBy(x => x.FirstName).ToList();
                if (sortByParametrs.SelectedIndex == 2) client = client.OrderByDescending(x => x.FirstName).ToList();
                if (sortByParametrs.SelectedIndex == 3) client = client.OrderBy(x => x.LastService).ToList();
                if (sortByParametrs.SelectedIndex == 4) client = client.OrderByDescending(x => x.LastService).ToList();
                if (sortByParametrs.SelectedIndex == 5) client = client.OrderBy(x => x.CountService).ToList();
                if (sortByParametrs.SelectedIndex == 6) client = client.OrderByDescending(x => x.CountService).ToList();

                if (checkDataBH.IsChecked == true) client = client.Where(x => x.Birthday.HasValue && x.Birthday.Value.Month == DateTime.Now.Month).ToList();

                if (sortByGender.SelectedIndex != 0) client = client.Where(x => x.GenderCode == sortByGender.SelectedIndex).ToList();
                client = client.Where(c => c.FirstName.ToLower().Contains(fnd.ToLower())
                              || c.LastName.ToLower().Contains(fnd.ToLower())
                              || c.Patronymic.ToLower().Contains(fnd.ToLower())
                              || c.Email.ToLower().Contains(fnd.ToLower())
                              || c.Phone.ToLower().Contains(fnd.ToLower())).ToList();


                count = client.Count();
                if (sortByCount.SelectedIndex != 0 && sortByCount.SelectedItem != null) client = client.Skip(page * Int32.Parse(sortByCount.SelectedValue.ToString()))
                        .Take(Int32.Parse(sortByCount.SelectedValue.ToString())).ToList();

                DataClientList.ItemsSource = client;
                CountRow.Text = count.ToString();
                DataContext = this;

                int ost = 0;
                int pag = 1;
                if (sortByCount.SelectedIndex != 0 && sortByCount.SelectedItem != null)
                {
                    ost = count % Int32.Parse(sortByCount.SelectedValue.ToString());
                    pag = (count - ost) / Int32.Parse(sortByCount.SelectedValue.ToString());
                }
                if (ost > 0) pag++;
                pagin.Children.Clear();
                for (int i = 0; i < pag; i++)
                {
                    Button myButton = new Button();
                    myButton.Height = 20;
                    myButton.Content = i + 1;
                    myButton.Background = new SolidColorBrush(Colors.White);
                    myButton.BorderBrush = new SolidColorBrush(Colors.White);
                    myButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF9C1A"));
                    myButton.Cursor = Cursors.Hand;
                    myButton.HorizontalAlignment = HorizontalAlignment.Center;
                    myButton.Tag = i;
                    myButton.Click += new RoutedEventHandler(paginButton_Click);
                    pagin.Children.Add(myButton);
                }
                turnButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void turnButton()
        {
            try
            {
                if (page == 0) { back.IsEnabled = false; }
                else { back.IsEnabled = true; };
                if (sortByCount.SelectedIndex != 0 && sortByCount.SelectedItem != null)
                {
                    if ((page + 1) * Int32.Parse(sortByCount.SelectedValue.ToString()) >= count) { forward.IsEnabled = false; }
                    else { forward.IsEnabled = true; }
                }
                else { forward.IsEnabled = false; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void paginButton_Click(object sender, RoutedEventArgs e)
        {
            page = Convert.ToInt32(((Button)sender).Tag.ToString());
            Load();
        }

        private void UpdateClient_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Load();
        }

        private void AddClient_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WorkWithClient wb = new WorkWithClient(null, db, this);
            wb.ShowDialog();
        }

        private void DeleteClient_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataClientList.SelectedItems.Count > 0)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить {DataClientList.SelectedItems.Count} агента(ов)?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes);
                try
                {
                    var selectedClients = DataClientList.SelectedItems.Cast<Client>().ToList();
                    int countDeleteClient = 0;
                    foreach (var client in selectedClients)
                    {
                        if (client.ClientService.Count > 0) // Проверка на сервисы
                        {
                            MessageBox.Show($"Клианет {client.ID} не может быть удалён. Присутсвует информация о посещениях");
                        }
                        else
                        {
                            foreach (Tag tag in client.Tag)
                            {
                                db.Tag.Remove(tag);
                            }
                            db.Client.Remove(client);
                            countDeleteClient++;
                            db.SaveChanges();
                        }
                    }
                    if (countDeleteClient != 0)
                    {
                        MessageBox.Show($"Удалено агентов: {countDeleteClient}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    Load();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void forward_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (sortByCount.SelectedIndex != 0 && sortByCount.SelectedItem != null)
                {
                    if (page < db.Client.Count() / Int32.Parse(sortByCount.SelectedValue.ToString()) - 1)
                    {
                        page++;
                        Load();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (page >= 0)
            {
                page--;
                Load();
            }
        }

        private void checkDataBH_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void sortByParametrs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void sortByGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void sortByCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void FindClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            page = 0;
            fnd = ((TextBox)sender).Text;
            Load();
        }

        private void EditButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsWindowOpen<WorkWithClient>())
            {
                MessageBox.Show("Окно Редактирования уже открыто.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                WorkWithClient wb = new WorkWithClient(DataClientList.SelectedItem as Client, db, this);
                wb.Show();
            }
        }

        private void checkDataBH_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            page = 0;
            Load();
        }

        private void BtnOpenServices_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                ServiceList dlg = new ServiceList(DataClientList.SelectedItem as Client, db);
                dlg.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
