using LengSchool11.Pages;
using System.Windows;

namespace LengSchool11
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Navigate(new ClientList());
        }

        private void frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                ClientList pg = (ClientList)e.Content;
            }
            catch { };
        }
    }
}
