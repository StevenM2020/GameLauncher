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
using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for Launcher.xaml
    /// </summary>
    public partial class Launcher : Window
    {
        private string currentPage;
        private string strId;

        private string connectionUri =
            "mongodb+srv://Steven:xEEJd79luZxta49Z@gamelauncherdata.loytk7b.mongodb.net/?retryWrites=true&w=majority";

        string strStore = "";

        List<Game> games = new List<Game>();
        List<DownloadedGames> downloadedGames = new List<DownloadedGames>();
        private int[] intSearchindex;
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\GameLauncherFiles\\";
        string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\GameLauncherFiles\\", "gamelauncherinfo.json");

        public Launcher(string strId, string strUsername)
        {
            InitializeComponent();
            // Set store page as default
            GoTo(new Store(this));
            currentPage = (new Store(this)).Title;
            strStore = currentPage;
            somethingFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            lblUsername.Content = "Welcome, " + strUsername;

            this.strId = strId;


            // Get games from database
            MongoClient dbClient = new MongoClient(connectionUri);
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Games");
            var filter = Builders<BsonDocument>.Filter.Empty;
            var result = dbList.Find(filter).ToList();


            foreach (var game in result)
            {
                games.Add(new Game(game["_id"].ToString(), game["name"].ToString()));
            }



            // Check if the directory exists
            if (!Directory.Exists(folderPath))
            {
                // If the directory does not exist, create it
                Directory.CreateDirectory(folderPath);
            }

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // If the file exists, read it into downloadedGames
                string json = File.ReadAllText(filePath);
                // check if json is empty
                if (json != "")
                {
                    downloadedGames = JsonConvert.DeserializeObject<List<DownloadedGames>>(json);
                }
            }
            else
            {
                // If the file does not exist, create it
                File.Create(filePath).Close();
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
                scrLauncher.ScrollToTop();
                //MessageBox.Show("Page changed, " + page.Title);
            }


        }

        private void btnStore_Click(object sender, RoutedEventArgs e)
        {
            GoTo(new Store(this));
        }

        private void DeleteAccount_Click(object sender, MouseButtonEventArgs e)
        {


            bool blnConfirm =
                MessageBox.Show("Are you sure you want to delete your account?", "Delete Account",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes;
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

                    intSearchindex[lstSearch.Items.Count - 1] = (games.IndexOf(game));
                }
            }

            pnlSearch.Height = 26 + (lstSearch.Items.Count * 26);

        }


        public List<DownloadedGames> GetDownloadedGames()
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // If the file does not exist, return an empty list
                return new List<DownloadedGames>();
            }

            // Read the file into downloadedGames
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<DownloadedGames>>(json) ?? new List<DownloadedGames>();
        }

        public void UpdateDownloadedGames()
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // If the file does not exist, assign an empty list to downloadedGames
                downloadedGames = new List<DownloadedGames>();
                return;
            }

            // Read the file into downloadedGames
            string json = File.ReadAllText(filePath);
            downloadedGames = JsonConvert.DeserializeObject<List<DownloadedGames>>(json) ?? new List<DownloadedGames>();
        }

        public void AddDownloadedGame(string id, string name, string exeLocation)
        {
            downloadedGames.Add(new DownloadedGames(id, name, exeLocation));
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // If the file does not exist, create it
                File.Create(filePath).Close();
            }

            // Write downloadedGames to the file
            string json = JsonConvert.SerializeObject(downloadedGames);
            File.WriteAllText(filePath, json);
        }

        // removes a downloaded game from the list
        public void RemoveDownloadedGame(string id)
        {
            downloadedGames.RemoveAll(x => x.Id == id);
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // If the file does not exist, create it
                File.Create(filePath).Close();
            }

            // Write downloadedGames to the file
            string json = JsonConvert.SerializeObject(downloadedGames);
            File.WriteAllText(filePath, json);
        }

        // goes to the selected game page
        private void Search_Selected(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Search" || txtSearch.Text.Trim() == "" || txtSearch.Text.Length < 1)
            {
                return;
            }

            GoTo(new GameView(gameId: games[intSearchindex[lstSearch.SelectedIndex]].Id, this));
            txtSearch.Text = "Search";
            lstSearch.Items.Clear();
            lstSearch.SelectedIndex = -1;
        }

        public string GetConnectionUri()
        {
            return connectionUri;
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

            public string Name
            {
                get => name;
            }

            public string Id
            {
                get => id;
            }
        }

        public struct DownloadedGames
        {
            private string name;
            private string id;
            private string exeLocation;

            public DownloadedGames(string id, string name, string exeLocation)
            {
                this.name = name;
                this.id = id;
                this.exeLocation = exeLocation;
            }

            public string Name
            {
                get => name;
            }

            public string Id
            {
                get => id;
            }

            public string ExeLocation
            {
                get => exeLocation;
            }
        }
    }
}
