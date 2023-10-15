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
using System.Windows.Shapes;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : Page
    {
        public GameView()
        {
            InitializeComponent();
        }

        private void btnStore_Click(object sender, RoutedEventArgs e)
        {
            //after clicking the store button, go to the store page
            //Store store = new Store();
            //this.NavigationService.Navigate(store);
            
        }
    }
}
