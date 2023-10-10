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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for Launcher.xaml
    /// </summary>
    public partial class Launcher : Window
    {
        const string connectionUri = "mongodb+srv://Steven:xEEJd79luZxta49Z@gamelauncherdata.loytk7b.mongodb.net/?retryWrites=true&w=majority";
        public Launcher()
        {
            InitializeComponent();

            MongoClient dbClient = new MongoClient(connectionUri);
            // Get the collection of users and filter by username
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Games");
            
            //MessageBox.Show(dbList.Find(new BsonDocument()).ToList().Count.ToString());

            int intNumGames = dbList.Find(new BsonDocument()).ToList().Count;
            for (int i = 0; i < 4; i++)
            {
                string imageUri = dbList.Find(new BsonDocument()).ToList()[i]["images"][0].ToString();
                string gameName = dbList.Find(new BsonDocument()).ToList()[i]["name"].ToString();

                //MessageBox.Show(imageUri);
                if (imageUri == "")
                {
                    MessageBox.Show("No image found");
                    continue;
                }
                Image img = new Image();
               
                //return;
                img.Source = new BitmapImage(new Uri(imageUri));
                img.Margin = new Thickness(5);
                img.Height = 100;
                img.Width = 200;

                grdStore.Children.Add(img);
                Grid.SetRow(img, 0 );
                Grid.SetColumn(img, i);

                //add a label under each image
                Label lbl = new Label();
                lbl.Content = gameName;
                lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                grdStore.Children.Add(lbl);
                Grid.SetRow(lbl, 1);
                Grid.SetColumn(lbl, i);




            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
