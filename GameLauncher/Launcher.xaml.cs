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
        string strStore = "";

        List<Game> games = new List<Game>();
        private int[] intSearchindex; 
        public Launcher(string strId, string strUsername)
        {
            InitializeComponent();

            // Set store page as default
            GoTo(new Store(this));
            currentPage = (new Store(this)).Title;
            strStore = currentPage;
            somethingFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            lblUsername.Content = "Welcome, " + strUsername;
            //lblUsernameChar.Content = strUsername.First().ToString();

            //MessageBox.Show("Welcome, " + strId);
            this.strId = strId;


            // Get games from database
            MongoClient dbClient = new MongoClient(connectionUri);
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Games");
            var filter = Builders<BsonDocument>.Filter.Empty;
            var result = dbList.Find(filter).ToList();

            //MessageBox.Show(result.Count.ToString());
    
            // fill games list with games from database using a foreach 
            foreach (var game in result)
            {
                //MessageBox.Show(game["name"].ToString());
                games.Add(new Game(game["_id"].ToString(), game["name"].ToString()));
            }
        }

        // Replace column 1 with the new page
        public void GoTo(Page page)
        {
            if (currentPage != strStore && page.Title == strStore || page.Title != strStore)
            {
                somethingFrame.NavigationService.Navigate(page);
                somethingFrame.NavigationService.RemoveBackEntry();
                currentPage = page.Title;
                //MessageBox.Show("Page changed, " + page.Title);
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
            // https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/deleteOne/

            //MessageBox.Show("broken");
            // var filter = Builders<Restaurant>.Filter
            // .Eq(r => r.Name, "Ready Penny Inn");
            //return await _restaurantsCollection.DeleteOneAsync(filter);

        }

        private void brdUserControls_MouseEnter(object sender, MouseEventArgs e)
        {
            brdUserControls.Height = 96;
            lblDelete.Visibility = Visibility.Visible;
            lblSupportEmail.Visibility = Visibility.Visible;
            lblEULA.Visibility = Visibility.Visible;
        }

        private void brdUserControls_MouseLeave(object sender, MouseEventArgs e)
        {
            brdUserControls.Height = 26;
            lblDelete.Visibility = Visibility.Hidden;
            lblSupportEmail.Visibility = Visibility.Hidden;
            lblEULA.Visibility = Visibility.Hidden;

        }

        private void txtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = (TextBox)sender;
            if (txtBox.Name == "txtSearch" && txtBox.Text == "Search")
                txtBox.Text = "";

        }

        // adds the places holder text when the user leaves the textbox
        private void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = (TextBox)sender;
            if (txtBox.Name == "txtSearch" && txtBox.Text == "")
                txtBox.Text = "Search";
        }

        private void txtSearch_OnTextChange(object sender, TextChangedEventArgs e)
        {
            
            if (txtSearch.Text == "Search" || txtSearch.Text.Trim() == "" || txtSearch.Text.Length < 1)
            {
                pnlSearch.Height = 26;
                return;
            }
            lstSearch.Items.Clear();

            foreach (Game game in games)
            {
                // if the game name contains the search text then add it to the list and keep track of the index

                if (game.Name.ToLower().Contains(txtSearch.Text.ToLower().Trim()))
                {
                    lstSearch.Items.Add(game.Name);
                    if (lstSearch.Items.Count == 1)
                    {
                        intSearchindex = new int[1];
                    }
                    else
                    {
                        Array.Resize(ref intSearchindex, lstSearch.Items.Count);
                    }   
                    intSearchindex[lstSearch.Items.Count-1] = (games.IndexOf(game));
                }
            }

            pnlSearch.Height = 26 + (lstSearch.Items.Count * 26);
            
        }

        public struct Game
        {
            private string name;
            private string id;

            public Game(string id, string name)
            {
                this.name = name;
                this.id = id;
            }

            public string Name { get => name; }
            public string Id { get => id; }
        }

        private void Search_Selected(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Search" || txtSearch.Text.Trim() == "" || txtSearch.Text.Length < 1)
            {
                return;
            }

            GoTo(new GameView(gameId: games[intSearchindex[lstSearch.SelectedIndex]].Id ));
            txtSearch.Text = "Search";
            lstSearch.Items.Clear();
            lstSearch.SelectedIndex = -1;
        }
    }
}
