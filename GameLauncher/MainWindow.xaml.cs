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

            MongoClient dbClient = new MongoClient(connectionUri);
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", txtUsername.Text);
            var result = dbList.Find(filter).ToList();
            //MessageBox.Show(result[0]["password"].ToString());

            if (result.Count > 0)
            {
                if (result[0]["password"].ToString() == txtPassword.Text)
                {
                    MessageBox.Show("Login Successful");
                    // code to login the user will go here
                }
                else
                {
                    MessageBox.Show("Incorrect Password");
                }
            }
            else
            {
                MessageBox.Show("Incorrect Username");

            }













        }
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        { 
            MessageBox.Show("Sign Up not coded");
        }

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

        private void lblOption_MouseEnter(object sender, MouseEventArgs e)
        {
            lblOption.FontWeight = FontWeights.Bold;
        }

        private void lblOption_MouseLeave(object sender, MouseEventArgs e)
        {
            lblOption.FontWeight = FontWeights.Regular;
        }
    }
}
