using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Dairy.Models;
using Dairy.ViewModels;

namespace Dairy.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {

        private readonly HttpClient client = new HttpClient();
        private MainViewModel ViewModel;

        public MainView()
        {
            InitializeComponent();
            ViewModel = this.DataContext as MainViewModel;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dairies = await client.GetFromJsonAsync<List<DairyModel>>("http://localhost:5000/api/Dairies/getDairyList?page=1&pageSize=30");
        }
    }
}
