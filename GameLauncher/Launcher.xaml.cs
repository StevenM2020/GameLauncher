using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Navigation;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for Launcher.xaml
    /// </summary>
    public partial class Launcher : Window
    {
        private string currentPage;
        public Launcher()
        {
            InitializeComponent();

            // Set store page as default
            GoTo(new Store(this));
            currentPage = (new Store(this)).Title;
            somethingFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        // Replace column 1 with the new page
        public void GoTo(Page page)
        {
            if (currentPage != page.Title)
            {
                somethingFrame.NavigationService.Navigate(page);
                somethingFrame.NavigationService.RemoveBackEntry();
                currentPage = page.Title;
                MessageBox.Show("Page changed, " + page.Title);
            }


        }

        private void btnStore_Click(object sender, RoutedEventArgs e)
        {
           GoTo(new Store(this));
        }
    }
}
