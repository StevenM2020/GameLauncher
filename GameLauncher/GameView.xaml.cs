using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
        private string connectionUri = "";
        ObjectId gameId;
        string fileName = "game.zip";
        string downloadPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "game.zip");
        private string gameDownloadURL = "";

        public GameView(string gameId, Launcher launcher)
        {
            InitializeComponent();

            this.gameId = ObjectId.Parse(gameId);
            connectionUri = launcher.GetConnectionUri();

            try
            {
                // Connect to database
                MongoClient dbClient = new MongoClient(connectionUri);

                var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Games");
                var filter = Builders<BsonDocument>.Filter.Eq("_id", this.gameId);
                var result = dbList.Find(filter).ToList();

                // Check if game exists
                if (result.Count == 0)
                {
                    MessageBox.Show("Game not found");
                    launcher.GoTo(new Store(launcher));
                    return;
                }

                // Set game image
                try
                {

                    BitmapImage gameImage = new BitmapImage(new Uri(result[0]["images"][0].ToString()));
                    imgGame.Source = gameImage;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("ERROR: missing image \n" + e);
                }

                // Set game name
                try
                {
                    lblGameName.Content = result[0]["name"].ToString();
                }
                catch (Exception e)
                {
                    lblGameName.Content = "No name available";
                    Debug.WriteLine("ERROR: missing name \n" + e);
                }

                // Set game description
                try
                {
                    lblDescription.Text = result[0]["description"].ToString();
                }
                catch (Exception e)
                {
                    lblDescription.Text = "No description available";

                    Debug.WriteLine("ERROR: missing description \n" + e);
                }

                // Set game description
                try
                {
                    gameDownloadURL = result[0]["downloadURL"].ToString();
                }
                catch (Exception e)
                {
                    isInstalling = true;

                    btnInstall.Content = "No download available";
                    btnInstall.Width = 200;
                    btnInstall.Background = Brushes.Red;
                    btnInstall.Clip = new RectangleGeometry(new Rect(5, 0, 190, 29), 5.0, 5.0);
                    btnInstall.Margin = new Thickness(btnInstall.Margin.Left - 100, btnInstall.Margin.Top, btnInstall.Margin.Right, btnInstall.Margin.Bottom);




                    Debug.WriteLine("ERROR: missing description \n" + e);
                }

                // Set game properties
                try
                {
                    foreach (var item in result[0]["properties"].AsBsonDocument)
                    {
                        try
                        {
                            pnlGameProperties.Children.Add(StackPanelMaker(item.Name, item.Value.AsString));
                            pnlGameProperties.Children.Add(new Rectangle() { Height = 1, Fill = Brushes.White });
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("ERROR: missing property \n" + e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("ERROR: missing all properties \n" + e);
                    pnlGameProperties.Children.Add(StackPanelMaker("", "properties not available"));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: could not connect to database \n" + e);
                MessageBox.Show("Could not connect to database");
            }
        }

        bool isInstalling = false;
        private async void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (isInstalling)
            {
                return;
            }
            isInstalling = true;
            try
            {
                btnInstall.Content = "Installing...";
                btnInstall.Cursor = Cursors.Wait;

                MongoClient dbClient = new MongoClient(connectionUri);
                var database = dbClient.GetDatabase("GameLauncher");
                var collection = database.GetCollection<BsonDocument>("Games");
                var dbList = database.GetCollection<BsonDocument>("Games");
                var filter = Builders<BsonDocument>.Filter.Eq("_id", gameId);
                var result = dbList.Find(filter).ToList();

                try
                {
                    int intDownloads = int.Parse(result[0]["downloads"].ToString());
                    var update = Builders<BsonDocument>.Update.Set("downloads", intDownloads + 1);
                    collection.UpdateOne(filter, update);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: missing downloads \n" + ex);
                }
                //https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/updateOne/


                string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";
                string fileName2 = lblGameName.Content + ".zip";
                string eFolder = @"\" + lblGameName.Content;
                //string url = "https://public.bn.files.1drv.com/y4mqmZnSNqfe1t_FL2HzgAN5BD735Ln-8kgmCoUGe1Lx1Is_WIXvO4MdJl1-8y2UmAf9GItbkncblRpsGGz5Q730f5bKYszYdpyEG7DwWazVqvxXFr7fK6P6XVKZZq3tlBDu7r_GZBneKqeUFSYa3mAl3rLUc3BOj9ehmfCSXhZhtgwICaz2GsPR6Vbwhebe8SlQDuXJ4XPtfnWjruHyJJ-59-9Eq2Knog3lCCY9-Owpd0?AVOverride=1";
                
                string gameLauncherFilesPath = downloadsPath + "GameLauncherFiles\\";

                if (!Directory.Exists(gameLauncherFilesPath))
                {
                    Directory.CreateDirectory(gameLauncherFilesPath);
                }
                

                using (WebClient client = new WebClient())
                {
                    try
                    {
                        await client.DownloadFileTaskAsync(gameDownloadURL, gameLauncherFilesPath + fileName2);
                    }
                    catch (WebException)
                    {
                        MessageBox.Show("Download connection failed, please contact support.");
                    }

                    try
                    {
                        string zipPath = gameLauncherFilesPath + fileName2;
                        string extractPath = gameLauncherFilesPath + eFolder;

                        ZipFile.ExtractToDirectory(zipPath, extractPath);
                        File.Delete(zipPath);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("unzip failed");
                    }

                    try
                    {
                        // find the game exe
                        string[] files = Directory.GetFiles(gameLauncherFilesPath + eFolder, "*.exe", SearchOption.AllDirectories);
                        string gameExe = files[0];
                        // open the game exe
                        Process.Start(gameExe);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("could not find game file");
                    }
                    

                }

                btnInstall.Content = "Installed";
                btnInstall.Cursor = Cursors.Hand;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: could not connect to database \n" + ex);
                MessageBox.Show("Could not connect to database");
            }
        }
 
      
        // this creates a vertical stack panel with two labels
        private StackPanel StackPanelMaker(string strLabel1, string strLabel2)
        {
            StackPanel pnl = new StackPanel();
            Label lbl1 = new Label();
            Label lbl2 = new Label();
            lbl2.HorizontalContentAlignment = HorizontalAlignment.Right;
            pnl.Orientation = Orientation.Horizontal;
            lbl1.Content = strLabel1;
            lbl2.Content = strLabel2;
            lbl2.Foreground = Brushes.White;
            lbl1.Foreground = Brushes.White;
            lbl1.Width = 100;
            lbl2.Width = 150;
            pnl.Children.Add(lbl1);
            pnl.Children.Add(lbl2);
            return pnl;
        }
    }
}
