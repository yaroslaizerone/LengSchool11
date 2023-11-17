using LengSchool11.Entity;
using LengSchool11.Pages;
using System.Windows;

namespace LengSchool11.Windows
{
    /// <summary>
    /// Логика взаимодействия для ServiceList.xaml
    /// </summary>
    public partial class ServiceList : Window
    {
        public ServiceList(Client client, SchoolEntities db)
        {
            InitializeComponent();
            frame.Navigate(new ServiceClient(client, db));
        }

        private void frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                ServiceClient pg = (ServiceClient)e.Content;
            }
            catch { };
        }
    }
}
