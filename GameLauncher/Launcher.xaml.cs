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
using System.Diagnostics;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for Launcher.xaml
    /// </summary>
    public partial class Launcher : Window
    {
        private string currentPage;
        private string strId;
        private string connectionUri = "mongodb+srv://Steven:xEEJd79luZxta49Z@gamelauncherdata.loytk7b.mongodb.net/?retryWrites=true&w=majority";
        public Launcher(string strId, string strUsername)
        {
            InitializeComponent();

            // Set store page as default
            GoTo(new Store(this));
            currentPage = (new Store(this)).Title;
            somethingFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            lblUsername.Content = "Welcome, " + strUsername;
            //lblUsernameChar.Content = strUsername.First().ToString();

            //MessageBox.Show("Welcome, " + strId);
            this.strId = strId;

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

        private void DeleteAccount_Click(object sender, MouseButtonEventArgs e)
        {
            //string strCheckPassword = Microsoft.VisualBasic.Interaction.InputBox("Please enter your password to confirm", "Delete Account", "");
            //MessageBox.Show(strCheckPassword);


            bool blnConfirm = MessageBox.Show("Are you sure you want to delete your account?", "Delete Account", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            if (!blnConfirm)
            {
                return;
            }
            MongoClient dbClient = new MongoClient(connectionUri);
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Users");
            //convert string to ObjectId
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(strId));
            var result = dbList.Find(filter).ToList();
            if (result.Count > 0)
            {
                //MessageBox.Show("more?");
                dbList.DeleteOneAsync(filter);
                MessageBox.Show("Account deleted");
                this.Close();
            }

            //MessageBox.Show("broken");
            // var filter = Builders<Restaurant>.Filter
            // .Eq(r => r.Name, "Ready Penny Inn");
            //return await _restaurantsCollection.DeleteOneAsync(filter);

        }

        private void brdUserControls_MouseEnter(object sender, MouseEventArgs e)
        {
            brdUserControls.Height = 84;
            lblDelete.Visibility = Visibility.Visible;
            lblSupportEmail.Visibility = Visibility.Visible;
        }

        private void brdUserControls_MouseLeave(object sender, MouseEventArgs e)
        {
            brdUserControls.Height = 26;
            lblDelete.Visibility = Visibility.Hidden;
            lblSupportEmail.Visibility = Visibility.Hidden;
            
        }

    }
}
