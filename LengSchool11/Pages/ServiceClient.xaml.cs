using LengSchool11.Entity;
using System.Windows;
using System.Windows.Controls;

namespace LengSchool11.Pages
{
    public partial class ServiceClient : Page
    {
        Client client = new Client();
        SchoolEntities db = new SchoolEntities();

        public ServiceClient(Client client, SchoolEntities db)
        {
            InitializeComponent();
            this.client = client;
            this.db = db;
            DataContext = this.client;
            TbClientInfo.Text = $"{client.FirstName} {client.LastName} {client.Patronymic}({client.ID})";
            if (client.ServiceList.Count > 0)
            {
                LViewService.ItemsSource = this.client.ServiceList;
            }
            else
            {
                LViewService.Visibility = Visibility.Collapsed;
                spServiceInfo.Children.Clear();
                TextBlock tb = new TextBlock();
                tb.Text = "У данного клиента нет посещений";
                tb.FontSize = 22;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                spServiceInfo.Children.Add(tb);
            }
        }
    }
}
