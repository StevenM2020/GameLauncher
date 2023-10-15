using MongoDB.Bson;
using MongoDB.Driver;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for Store.xaml
    /// </summary>
    public partial class Store : Page
    {
        const string connectionUri = "mongodb+srv://Steven:xEEJd79luZxta49Z@gamelauncherdata.loytk7b.mongodb.net/?retryWrites=true&w=majority";
        List<FeaturedGames> featuredGames = new List<FeaturedGames>();

        public Store()
        {
            InitializeComponent();

            MongoClient dbClient = new MongoClient(connectionUri);
            // Get the collection of users and filter by username
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Games");

            //MessageBox.Show(dbList.Find(new BsonDocument()).ToList().Count.ToString());

            int intNumGames = dbList.Find(new BsonDocument()).ToList().Count;

            int intGamesLeft = intNumGames;
            for (int j = 0; j <= intNumGames / 4; j++)
            {
                // how many columns will bb in this row
                int intNumCol = intGamesLeft >= 4 ? 4 : intGamesLeft;
                intGamesLeft -= 4;

                // define the rows
                RowDefinition row0 = new RowDefinition();
                row0.Height = new GridLength(100);
                RowDefinition row1 = new RowDefinition();
                row1.Height = new GridLength(25);

                // add more rows to the grid
                grdStore.RowDefinitions.Add(row0);
                grdStore.RowDefinitions.Add(row1);


                for (int i = 0; i < intNumCol; i++)
                {
                    string imageUri, gameName, gameDescription, gameID;
                    BitmapImage gameImage;

                    // get game data from database
                    try
                    {
                        gameImage = new BitmapImage(new Uri(dbList.Find(new BsonDocument()).ToList()[i + j * 4]["images"][0].ToString())); // validates image
                        gameName = dbList.Find(new BsonDocument()).ToList()[i + j * 4]["name"].ToString();
                        //gameDescription = dbList.Find(new BsonDocument()).ToList()[i + j * 4]["description"].ToString();
                        gameID = dbList.Find(new BsonDocument()).ToList()[i + j * 4]["_id"].ToString();
                    }
                    catch (Exception e)
                    {
                        continue;
                    }



                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();

                    //return;
                    img.Source = gameImage;
                    img.Tag = gameID;
                    img.Margin = new Thickness(5);
                    img.Height = 100;
                    img.Width = 200;
                    img.Stretch = Stretch.Fill;
                    //clip the images corners using a rectangle with a radius of 10 
                    RectangleGeometry myRectangleGeometry = new RectangleGeometry();
                    myRectangleGeometry.RadiusX = 10;
                    myRectangleGeometry.RadiusY = 10;
                    myRectangleGeometry.Rect = new Rect(0, 0, 160, 90);
                    img.Clip = myRectangleGeometry;
                    img.MouseLeftButtonDown += new MouseButtonEventHandler(SelectGame);
                    img.Cursor = Cursors.Hand;


                    grdStore.Children.Add(img);
                    Grid.SetRow(img, j * 2);
                    Grid.SetColumn(img, i);

                    //add a label under each image
                    Label lbl = new Label();
                    lbl.Content = gameName;
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                    lbl.Foreground = Brushes.White;
                    grdStore.Children.Add(lbl);
                    Grid.SetRow(lbl, j * 2 + 1);
                    Grid.SetColumn(lbl, i);

                    //MessageBox.Show(gameName);


                }


                // Get the featured games
                var filter = Builders<BsonDocument>.Filter.Eq("Featured", true);
                var result = dbList.Find(filter).ToList();

                if (result.Count == 0)
                {
                    MessageBox.Show("no featured games");
                    return;
                }

                foreach (var game in result)
                {

                }

                //this.Content = new SignUp();
                // https://stackoverflow.com/questions/10196445/switch-views-in-same-window-wpf-without-creating-new-instances-of-the-pages#:~:text=Much%20easier%20way%20would%20be%3A%201%20Use%20MainWindow,instances%29.%204%20Use%20MainWindow.SetPage%20%28Pages.First%29%20to%20change%20pages.
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ChangeFeaturedGame()
        {
            while (true)
            {
                await Task.Delay(5000);
                btnStore.Content = "Loading...";
                await Task.Delay(5000);
                btnStore.Content = "Loading..";
                await Task.Delay(5000);
                btnStore.Content = "Loading.";
                await Task.Delay(5000);
                btnStore.Content = "Loading";

            }
            return;
        }
        //https://stackoverflow.com/questions/43951923/wpf-async-await

        private void SelectGame(object sender, RoutedEventArgs e)
        {
            var imgGame = (System.Windows.Controls.Image)sender;
            //MessageBox.Show(imgGame.Tag.ToString());
            this.Content = new GameView();
        }

        struct FeaturedGames
        {
            private string Title;
            private string Image;
            private string Description;
            private string ID;
        }
        //https://www.tutorialspoint.com/csharp/csharp_struct.htm#:~:text=To%20define%20a%20structure%2C%20you%20must%20use%20the,author%3B%20public%20string%20subject%3B%20public%20int%20book_id%3B%20%7D%3B
    }
}
