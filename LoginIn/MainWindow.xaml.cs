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

namespace LoginIn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<int> testTurd = new List<int>();
        SolidColorBrush disabledBrush = new SolidColorBrush();

        List<LoginDetails> validLogins = new List<LoginDetails>();

        public MainWindow()
        {
            InitializeComponent();

            // Define custom Brush
            disabledBrush.Color = Color.FromArgb(100, 60, 60, 60);
            
            // Load as objects
            LoadSaveFiles();
        }


        // Username box events
        // When the username box is first created
        private void UserName_Initialized(object sender, EventArgs e)
        {
            TextBox me = (TextBox)sender;

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

            if(validLogin)
            {
                output.Text = string.Format("Success!!!\nYou're allowed in!");
            }
            else
            {
                output.Text = string.Format("Failure.\nNot a valid account");
            }
        }

        private bool MatchesAnyLogin(LoginDetails attempt, out int index)
        {
            // Check through all valid accounts
            for(int i = 0; i < validLogins.Count; ++i)
            {
                LoginDetails current = validLogins[i];

                // If we find a match
                if(attempt.Username == current.Username && attempt.Password == current.Password)
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

        private bool UsernameIsUnique(string username)
        {
            foreach(LoginDetails valid in validLogins)
            {
                if(valid.Username == username)
                {
                    return false;
                }
            }

            return true;
        }

        private void createAccount_Click(object sender, RoutedEventArgs e)
        {
            bool canCreate = UsernameIsUnique(UserName.Text);

            if(canCreate)
            {
                validLogins.Add(new LoginDetails(UserName.Text, passwordBox.Password));

                // Create a string array with the lines of text
                string text = UserName.Text + "," + passwordBox.Password + Environment.NewLine;

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
    }
}
