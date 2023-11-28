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
        private string strPassword = "";

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

            var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(strPassword, salt, iterations, hashAlgorithm, keySize);

            if (result.Count > 0) // check if the username is found
            {
                if (result[0]["password"].ToString() == Convert.ToHexString(hashedPassword)) // check if the password is correct
                {
                    //MessageBox.Show("Login Successful");
                }
                else
                {
                    MessageBox.Show("Incorrect Password, " + Convert.ToHexString(hashedPassword));
                    return;
                }
            }
            else // if the username is not found
            {
                MessageBox.Show("Incorrect Username");
                return;
            }

            // Create a new launcher window and close the current window

            Launcher launcher = new Launcher(result[0]["_id"].ToString(), result[0]["username"].ToString());
            launcher.Left = this.Left;
            launcher.Top = this.Top;
            launcher.Show();
            this.Close();
            return;

        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if(blnEye)
                strPassword = txtPassword.Text;
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

            canEULA.Visibility = Visibility.Visible;
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

        private void txtPass_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Text == "Password")
            {
                txtPassword.Text = "";
            }
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

        private void txtPass_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Text == "")
            {
                txtPassword.Text = "Password";
            }
        }

        private bool ValidInput()
        {
            // check if the user entered a username and password
            if (txtUsername.Text == "Username" || txtPassword.Text == "Password")
            {
                MessageBox.Show("Please enter a username and password");
                return true;
            }

            //check if the user entered anything but letters and numbers
            if (txtUsername.Text.Any(c => !char.IsLetterOrDigit(c)))
            {
                MessageBox.Show("Please enter a valid username");
                return true;
            }

            if (strPassword.Any(c => !char.IsLetterOrDigit(c)))
            {
                MessageBox.Show("Please enter a valid password");
                return true;
            }

            // https://stackoverflow.com/questions/76413133/char-check-that-contains-letters-and-digits
            // https://learn.microsoft.com/en-us/dotnet/api/system.char.isletterordigit?view=net-7.0
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions
            return false;
        }

        // controls the password textbox and password string
        // while hidden , the password is stored in a string and the textbox is filled with *
        private void TxtPassword_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPassword.Text != "Password" && !blnEye)
            {
                if (txtPassword.Text.Length < strPassword.Length && strPassword != "")
                {
                    strPassword = strPassword.Remove(strPassword.Length - 1);
                }
                else if (txtPassword.Text.Length > strPassword.Length)
                {
                    strPassword = strPassword + txtPassword.Text.Last();
                    txtPassword.Text = new string('*', txtPassword.Text.Length);
                    txtPassword.CaretIndex = txtPassword.Text.Length;
                }
            }


        }
        
        // controls the eye icon that shows the password
        private bool blnEye = false;
        private void Eye_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (blnEye)
            {
                imgEye.Source = new BitmapImage(new Uri("pack://application:,,,/hiddenEye2.png"));
                strPassword = txtPassword.Text;
                txtPassword.Text = new string('*', txtPassword.Text.Length);
            }
            else
            {
                imgEye.Source = new BitmapImage(new Uri("pack://application:,,,/eye.png"));
                txtPassword.Text = strPassword;

            }
            blnEye = !blnEye;
        }

        private void btnAgree_Click(object sender, RoutedEventArgs e)
        {
            MongoClient dbClient = new MongoClient(connectionUri);

            // Create a new document and insert it into the database
            var database = dbClient.GetDatabase("GameLauncher");
            var collection = database.GetCollection<BsonDocument>("Users");

            // hash the password
            var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(strPassword, salt, iterations, hashAlgorithm, keySize);

            // https://code-maze.com/csharp-hashing-salting-passwords-best-practices/

            // the document to be inserted into the database
            var document = new BsonDocument
            {
                {"username", txtUsername.Text},
                {"password", Convert.ToHexString(hashedPassword)},
                {"developer", false}
            };

            collection.InsertOne(document);

            MessageBox.Show("Sign Up Successful");
            canEULA.Visibility = Visibility.Hidden;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            canEULA.Visibility = Visibility.Hidden;
        }
    }
}
