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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

        // Hashing variables
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        byte[] salt = new byte[32] { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 };


        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            if (ValidInput())
                return;

            // Connect to the database
            MongoClient dbClient = new MongoClient(connectionUri);
            // Get the collection of users and filter by username
            var dbList = dbClient.GetDatabase("GameLauncher").GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", txtUsername.Text);
            var result = dbList.Find(filter).ToList();
            //MessageBox.Show(result[0]["password"].ToString());

            var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(txtPassword.Text, salt, iterations, hashAlgorithm, keySize);

            if (result.Count > 0) // check if the username is found
            {
                if (result[0]["password"].ToString() == Convert.ToHexString(hashedPassword)) // check if the password is correct
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

            Launcher launcher = new Launcher();
            launcher.Left = this.Left;
            launcher.Top = this.Top;
            launcher.Show();
            this.Close();
            return;

        }
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (ValidInput())
                return;

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


            //var salt = RandomNumberGenerator.GetBytes(32);
            // reliable salt
            //for (int i = 0; i < 32; i++)
            //{
            //    salt[i] = 0;
            //}
            
            var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(txtPassword.Text, salt, iterations, hashAlgorithm, keySize);
            //MessageBox.Show(Convert.ToHexString(hashedPassword));

            // https://code-maze.com/csharp-hashing-salting-passwords-best-practices/
            
            var document = new BsonDocument
            {
                {"username", txtUsername.Text},
                {"password", Convert.ToHexString(hashedPassword)},
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





        // removes the places holder text when the user clicks on the textbox
        private void txtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = (TextBox)sender;
            if (txtBox.Name == "txtUsername" && txtBox.Text == "Username")
                txtBox.Text = "";
            else if(txtBox.Name == "txtPassword" && txtBox.Text == "Password")
                txtBox.Text = "";
            
        }

        // adds the places holder text when the user leaves the textbox
        private void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = (TextBox)sender;
            if (txtBox.Name == "txtUsername" && txtBox.Text == "")
                txtBox.Text = "Username";
            else if (txtBox.Name == "txtPassword" && txtBox.Text == "")
                txtBox.Text = "Password";
        }

        private bool ValidInput()
        {
            // check if the user entered a username and password
            if (txtUsername.Text == "Username" || txtPassword.Text == "Password")
            {
                MessageBox.Show("Please enter a username and password");
                return true;
            }

            //check if the user entered anything but letters and
            if (txtUsername.Text.Any(c => !char.IsLetterOrDigit(c)))
            {
                MessageBox.Show("Please enter a valid username");
                return true;
            }

            if (txtPassword.Text.Any(c => !char.IsLetterOrDigit(c)))
            {
                MessageBox.Show("Please enter a valid password");
                return true;
            }

            // https://stackoverflow.com/questions/76413133/char-check-that-contains-letters-and-digits
            // https://learn.microsoft.com/en-us/dotnet/api/system.char.isletterordigit?view=net-7.0
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions
            return false;
        }
    }
}
