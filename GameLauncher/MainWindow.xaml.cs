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
using MongoDB.Driver;
using MongoDB.Bson;
using System.Windows.Interop;
using System.IO;
using System.Windows.Markup;
using System.Xml;



namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string connectionUri = "mongodb+srv://Steven:xEEJd79luZxta49Z@gamelauncherdata.loytk7b.mongodb.net/?retryWrites=true&w=majority";
        bool blnLogin = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Launcher launcher = new Launcher();
            launcher.Left = this.Left;
            launcher.Top = this.Top;
            launcher.Show();
            this.Close();
            return;

            // Connect to the database
            MongoClient dbClient = new MongoClient(connectionUri);
            // Get the collection of users and filter by username
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", txtUsername.Text);
            var result = dbList.Find(filter).ToList();
            //MessageBox.Show(result[0]["password"].ToString());

            if (result.Count > 0) // check if the username is found
            {
                if (result[0]["password"].ToString() == txtPassword.Text) // check if the password is correct
                {
                    MessageBox.Show("Login Successful");
                    // code to login the user will go here
                }
                else
                {
                    MessageBox.Show("Incorrect Password");
                    return;
                }
            }
            else // if the username is not found
            {
                MessageBox.Show("Incorrect Username");
                return;
            }

            // Create a new launcher window and close the current window



        }
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            // Connect to the database
            MongoClient dbClient = new MongoClient(connectionUri);
            // Get the collection of users and filter by username
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", txtUsername.Text);
            var result = dbList.Find(filter).ToList();

            if(result.Count > 0) // check if the username is found
            {
                MessageBox.Show("Username already exists");
                return;
            }

            // Create a new document and insert it into the database
            var database = dbClient.GetDatabase("GameLauncher");
            var collection = database.GetCollection<BsonDocument>("Users");

            
            var document = new BsonDocument
            {
                {"username", txtUsername.Text},
                {"password", txtPassword.Text},
                {"developer", false}
            };
          
            collection.InsertOne(document);

            MessageBox.Show("Sign Up Successful");
        }


        // just for testing purposes
            private void hold()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://Steven:UBdlX3HpQqXqHiNi@gamelauncherdata.loytk7b.mongodb.net/?retryWrites=true&w=majority");
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("Usernam", txtUsername.Text);
            var result = dbList.Find(filter).ToList();
            MessageBox.Show(result[0]["Password"].ToString());





            var database = dbClient.GetDatabase("GameLauncher");
            var collection = database.GetCollection<BsonDocument>("Users");






            var document = new BsonDocument
            {
                {"username", txtUsername.Text},
                {"password", txtPassword.Text},
                {"developer", false}
            };

            var document2 = new BsonDocument { { "student_id", 10000 }, {
                            "scores",
                            new BsonArray {
                            new BsonDocument { { "type", "exam" }, { "score", 88.12334193287023 } },
                            new BsonDocument { { "type", "quiz" }, { "score", 74.92381029342834 } },
                            new BsonDocument { { "type", "homework" }, { "score", 89.97929384290324 } },
                            new BsonDocument { { "type", "homework" }, { "score", 82.12931030513218 } }
                            }
                            }, { "class_id", 480 }
                    };
            collection.InsertOne(document2);
            MessageBox.Show("User Created");


        }


            // just for testing purposes
        private void ping()
        {
            const string connectionUri = "mongodb+srv://Steven:xEEJd79luZxta49Z@gamelauncherdata.loytk7b.mongodb.net/?retryWrites=true&w=majority";
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            // Set the ServerApi field of the settings object to Stable API version 1
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // Create a new client and connect to the server
            var client = new MongoClient(settings);
            // Send a ping to confirm a successful connection
            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                MessageBox.Show("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }

        // change the text of the label and the button when the user clicks on it
        private void lblOption_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (blnLogin) {
                btnContinue.Click -= btnLogin_Click;
                btnContinue.Click += btnSignUp_Click;
                lblOption.Content = "Have an account? Login";
                btnContinue.Content = "Sign Up";
                lblTitle.Content = "Make a new account";
            }
            else
            {
                btnContinue.Click -= btnSignUp_Click;
                btnContinue.Click += btnLogin_Click;
                lblOption.Content = "Don't have an account? Sign Up";
                btnContinue.Content = "Login";
                lblTitle.Content = "Login to Game Launcher ";
            }
            blnLogin = !blnLogin;
        }

        // change the color of the label when the user hovers over it
        private void lblOption_MouseEnter(object sender, MouseEventArgs e)
        {
            lblOption.Foreground = new SolidColorBrush(Colors.White);
            
        }
        
        // change the color of the label when the user leaves it
        private void lblOption_MouseLeave(object sender, MouseEventArgs e)
        {
            lblOption.Foreground = new SolidColorBrush(Colors.Gray);
        }


        private void MouseOvertext(object sender, MouseEventArgs e)
        {
            lblOption.FontWeight = FontWeights.Bold;
        }

        private void MouseOffText(object sender, MouseEventArgs e)
        {
            lblOption.FontWeight = FontWeights.Regular;
        }



        // code for speech
        const string ConnectionString = "connection string goes here";

        private void SignUpButton(object sender, RoutedEventArgs e)
        {
            MongoClient dbClient = new MongoClient(ConnectionString);
            var dbList = 
                dbClient.GetDatabase("MyDataBase").GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", txtUsername.Text);
            var result = dbList.Find(filter).ToList();

            if (result.Count > 0)
            {
                MessageBox.Show("Username already exists");
                return;
            }


            var document = new BsonDocument
            {
                {"username", txtUsername.Text},
                {"password", txtPassword.Text},
                {"developer", false}
            };



            var database = dbClient.GetDatabase("MyDataBase");
            var collection = database.GetCollection<BsonDocument>("Users");
            collection.InsertOne(document);

            MessageBox.Show("Sign Up Successful");

        }
    }
}
