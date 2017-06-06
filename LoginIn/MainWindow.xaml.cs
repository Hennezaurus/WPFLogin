using System;
using System.Collections.Generic;
using System.IO;
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
using System.Security;
using System.Security.Cryptography;

namespace LoginIn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Create new brush to colour grayed out text with
        SolidColorBrush disabledBrush = new SolidColorBrush();

        // List of all valid accounts which have been created
        List<LoginDetails> validLogins = new List<LoginDetails>();

        // Window constructor
        public MainWindow()
        {
            InitializeComponent();

            // Define custom Brush
            disabledBrush.Color = Color.FromArgb(100, 60, 60, 60);
            
            // Load as objects
            LoadSaveFiles();
        }

        // When the username box is first created
        private void UserName_Initialized(object sender, EventArgs e)
        {
            // Local reference to me
            TextBox me = (TextBox)sender;

            // Change to disabled brush by default
            me.Foreground = disabledBrush;
        }

        // When the username box is clicked
        private void UserName_GotFocus(object sender, RoutedEventArgs e)
        {
            // Store local copy of me
            TextBox me = (TextBox)sender;

            // Change brush colour to black
            UserName.Foreground = Brushes.Black;

            // Wipe text if it's placeholder
            if(me.Text == "Username...")
            {
                me.Text = "";
            }
        }

        // When the username box is un-clicked
        private void UserName_LostFocus(object sender, RoutedEventArgs e)
        {
            // Store local copy of me
            TextBox me = (TextBox)sender;

            // If nothing written, change to default text
            if(me.Text == "")
            {
                // Change brush to gray
                me.Foreground = disabledBrush;

                me.Text = "Username...";
            }

        }

        // Login button events
        private void login_Click(object sender, RoutedEventArgs e)
        {
            // Create attempt with new details
            LoginDetails attempt = new LoginDetails(UserName.Text, passwordBox.Password);

            // Check if it was a valid combo
            int successfulIndex;
            bool validLogin = MatchesAnyLogin(attempt, out successfulIndex);

            // Display valid, or invalid login message
            if(validLogin)
            {
                output.Text = string.Format("Success!!!\nYou're allowed in!");
            }
            else
            {
                output.Text = string.Format("Failure.\nNot a valid account");
            }
        }

        // Check to see if the login details match any valid accounts
        private bool MatchesAnyLogin(LoginDetails attempt, out int index)
        {
            // Check through all valid accounts
            for(int i = 0; i < validLogins.Count; ++i)
            {
                LoginDetails current = validLogins[i];

                // If we find a match
                if(attempt.Username == current.Username && GetHashString(attempt.Password) == current.Password)
                {
                    // Return the index and true
                    index = i;
                    return true;
                }
            }

            // No matches found
            index = -1;
            return false;
        }

        // Check if the username is unique
        private bool UsernameIsUnique(string username)
        {
            // Look through all logins, see if username already exists
            foreach(LoginDetails valid in validLogins)
            {
                if(valid.Username == username)
                {
                    return false;
                }
            }

            return true;
        }

        // Create a new account with the entered details
        private void createAccount_Click(object sender, RoutedEventArgs e)
        {
            // Is the username already in use?
            bool canCreate = UsernameIsUnique(UserName.Text);

            // If the username is free to use
            if(canCreate)
            {
                // Hash the password for security
                string passHash = GetHashString(passwordBox.Password);

                // Create a new login data, and add it to the list
                validLogins.Add(new LoginDetails(UserName.Text, passHash));

                // Create a csv line in form: username,MD5hashedPassword
                string text = UserName.Text + "," + passHash + Environment.NewLine;

                // Set a variable to the My Documents path.
                //string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                
                // Get current application directory and add our save folder
                string savePath = Directory.GetCurrentDirectory() + @"\SaveData";

                // Create sub-directory if needed
                Directory.CreateDirectory(savePath);

                // Create save file
                File.AppendAllText(savePath + @"\WriteFile.txt", text);
                
            }
            else
            {
                output.Text = string.Format("Sorry!\nThat username is already taken\nPlease try again.");
            }
        }

        // Get all valid logins from saved file, and convert them to LoginDetail objects
        private void LoadSaveFiles()
        {
            // Get current application directory and add our save folder
            string savePath = Directory.GetCurrentDirectory() + @"\SaveData\WriteFile.txt";

            // Don't try any of this if the file doesn't exist
            if (!File.Exists(savePath))
                return;

            // Create save file
            string[] saveFiles = File.ReadAllLines(savePath);

            // Convert each file line into a Login object
            foreach (string account in saveFiles)
            {
                // Split each line into two elements, username and password
                string[] details = account.Split(',');

                // Re-Create objects from elements
                LoginDetails loginAccount = new LoginDetails(details[0], details[1]);
                validLogins.Add(loginAccount);

            }
        }

        // Private Hash Helper
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        // Get string from hash
        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
