﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static GameLauncher.Launcher;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for Store.xaml
    /// </summary>
    public partial class Store : Page
    {
        private string connectionUri = "";

        List<FeaturedGames> featuredGames = new List<FeaturedGames>();
        private Launcher launcher;
        public Store(Launcher launcher1)
        {
            InitializeComponent();
            launcher = launcher1;
            connectionUri = launcher.GetConnectionUri();
            MongoClient dbClient = new MongoClient(connectionUri);
            // Get the collection of users and filter by username
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Games");


            int intNumGames = dbList.Find(new BsonDocument()).ToList().Count;

            int intGamesLeft = intNumGames;

            for (int j = 0; j < (int)Math.Ceiling((float)intNumGames / 4); j++)
            {

                // how many columns will bb in this row
                int intNumCol = intGamesLeft >= 4 ? 4 : intGamesLeft;
                intGamesLeft -= 4;

                // define the rows
                RowDefinition row0 = new RowDefinition();
                row0.Height = new GridLength(100);

                // add more rows to the grid
                grdStore.RowDefinitions.Add(row0);

                List<string> gameUris = new List<string>();
                List<string> gameNames = new List<string>();
                List<string> gameDescriptions = new List<string>();
                List<string> gameIDs = new List<string>();



                for (int i = 0; i < intNumCol; i++)
                {
                    string imageUri, gameName, gameDescription, gameID;
                    BitmapImage gameImage;

                    // get game data from database
                    try
                    {
                        gameImage = new BitmapImage(
                            new Uri(dbList.Find(new BsonDocument()).ToList()[i + j * 4]["images"][0]
                                .ToString())); // validates image
                        gameName = dbList.Find(new BsonDocument()).ToList()[i + j * 4]["name"].ToString();
                        //gameDescription = dbList.Find(new BsonDocument()).ToList()[i + j * 4]["description"].ToString();
                        gameID = dbList.Find(new BsonDocument()).ToList()[i + j * 4]["_id"].ToString();
                    }
                    catch (Exception e)
                    {
                        continue;

                    }

                    StackPanel sp = GameImageMaker(gameImage, gameName, gameID);
                    grdStore.Children.Add(sp);
                    Grid.SetRow(sp, j);
                    Grid.SetColumn(sp, i);

                    //System.Windows.Controls.Image img = new System.Windows.Controls.Image();

                    ////return;
                    //img.Source = gameImage;
                    //img.Tag = gameID;
                    //img.Margin = new Thickness(5);
                    //img.Height = 100;
                    //img.Width = 200;
                    //img.Stretch = Stretch.Fill;
                    ////clip the images corners using a rectangle with a radius of 10 
                    //RectangleGeometry myRectangleGeometry = new RectangleGeometry();
                    //myRectangleGeometry.RadiusX = 10;
                    //myRectangleGeometry.RadiusY = 10;
                    //myRectangleGeometry.Rect = new Rect(0, 0, 160, 90);
                    //img.Clip = myRectangleGeometry;
                    //img.MouseLeftButtonDown += new MouseButtonEventHandler(SelectGame);
                    //img.Cursor = Cursors.Hand;


                    //grdStore.Children.Add(img);
                    //Grid.SetRow(img, j * 2);
                    //Grid.SetColumn(img, i);

                    ////add a label under each image
                    //Label lbl = new Label();
                    //lbl.Content = gameName;
                    //lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
                    //lbl.Foreground = Brushes.White;
                    //grdStore.Children.Add(lbl);
                    //Grid.SetRow(lbl, j * 2 + 1);
                    //Grid.SetColumn(lbl, i);

                    ////MessageBox.Show(gameName);


                }
            }


        }

        public void SetPage(Page page)
        {
            this.Content = page;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ChangeFeaturedGame()
        {
            while (true)
            {
                await Task.Delay(5000);

            }

            return;
        }
        //https://stackoverflow.com/questions/43951923/wpf-async-await

        private void SelectGame(object sender, RoutedEventArgs e)
        {
            var imgGame = (System.Windows.Controls.Image)sender;

            
            launcher.GoTo(new GameView(gameId: imgGame.Tag.ToString(), launcher));

        }

        struct FeaturedGames
        {
            private string Title;
            private string Image;
            private string Description;
            private string ID;
        }
        //https://www.tutorialspoint.com/csharp/csharp_struct.htm#:~:text=To%20define%20a%20structure%2C%20you%20must%20use%20the,author%3B%20public%20string%20subject%3B%20public%20int%20book_id%3B%20%7D%3B

        private StackPanel GameImageMaker(BitmapImage gameImage, string strName, string strId)
        {
            StackPanel sp = new StackPanel();
            Label lbl = new Label();
            lbl.Content = strName;

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            // create the game thumbnail
            img.Source = gameImage;
            img.Tag = strId;
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

            // add a label under each image with the name
            lbl.Content = strName;
            lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
            lbl.Foreground = Brushes.White;

            // add the image and label to the stack panel
            sp.Children.Add(img);
            sp.Children.Add(lbl);
            return sp;
        }
    }
}