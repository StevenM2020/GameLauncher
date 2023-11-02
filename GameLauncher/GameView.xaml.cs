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
using System.Windows.Shapes;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : Page
    {
        const string connectionUri =
            "mongodb+srv://Steven:xEEJd79luZxta49Z@gamelauncherdata.loytk7b.mongodb.net/?retryWrites=true&w=majority";
        ObjectId gameId;
        public GameView(string gameId)
        {
            InitializeComponent();

            this.gameId = ObjectId.Parse(gameId);

            MongoClient dbClient = new MongoClient(connectionUri);

            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Games");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", this.gameId);
            var result = dbList.Find(filter).ToList();

            for (int i = 0; i <= 4; i++)
            {
                //MessageBox.Show(dbList.Find(new BsonDocument()).ToList()[i]["_id"].ToString());
            }
            if (result.Count == 0)
            {
                //MessageBox.Show("Game not found, " + gameId);
                return;
            }
            //MessageBox.Show(result[0]["images"][0].ToString());
            BitmapImage gameImage = new BitmapImage(new Uri(result[0]["images"][0].ToString()));
            imgGame.Source = gameImage;
            lblGameName.Content = result[0]["name"].ToString();

            lblDescription.Text = result[0]["description"].ToString();


            pnlGameProperties.Children.Add(StackPanelMaker("Developer", "text"));
            pnlGameProperties.Children.Add(new Rectangle() { Height = 1, Fill = Brushes.White });
            pnlGameProperties.Children.Add(StackPanelMaker("Platform", "text"));
            pnlGameProperties.Children.Add(new Rectangle() { Height = 1, Fill = Brushes.White });
            pnlGameProperties.Children.Add(StackPanelMaker("Release Date", "text"));
            pnlGameProperties.Children.Add(new Rectangle() { Height = 1, Fill = Brushes.White });
            pnlGameProperties.Children.Add(StackPanelMaker("Genres", "text"));
            pnlGameProperties.Children.Add(new Rectangle() { Height = 1, Fill = Brushes.White });
            pnlGameProperties.Children.Add(StackPanelMaker("Features", "text"));
        }

        private void btnStore_Click(object sender, RoutedEventArgs e)
        {
            //after clicking the store button, go to the store page
            //Store store = new Store();
            //this.NavigationService.Navigate(store);
            
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            MongoClient dbClient = new MongoClient(connectionUri);
            var database = dbClient.GetDatabase("GameLauncher");
            var collection = database.GetCollection<BsonDocument>("Games");
            var dbList = database.GetCollection<BsonDocument>("Games");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", gameId);
            var result = dbList.Find(filter).ToList();

            int intDownloads = int.Parse(result[0]["downloads"].ToString());

            var update = Builders<BsonDocument>.Update.Set("downloads", intDownloads + 1);

            collection.UpdateOne(filter, update);


            //https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/updateOne/
        }
        // create a function that creates and returns a horizontal stacked panel with 2 labels on either side. The function should also accept the values for the labels

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
