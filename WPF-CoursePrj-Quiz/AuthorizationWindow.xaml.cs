using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_CoursePrj_Quiz
{
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private List<User> users;
        private User user;
        private string users_path = "Users.bin";
        private int qty_attempts;

        // constructor the set default parameters
        public AuthorizationWindow()
        {
            InitializeComponent();
            users = new List<User>();
            if (FileCheck())
                users = GetUsers();
            qty_attempts = 3;
        }

        // checking if the file with ranking table is present, if no - it will created
        private bool FileCheck()
        {
            FileInfo file = new FileInfo(users_path);
            if (!file.Exists)
            {
                file.Create();
                MessageBox.Show("File with user's ranking system was not exist.\nUser's ranking table with new file provided.", "Warning window", MessageBoxButton.OK);
                return false;
            }
            else return true;
        }

        // loading list of users
        private List<User> GetUsers()
        {
            if (new FileInfo(users_path).Length > 0)
                using (FileStream fs = new FileStream(users_path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (BinaryReader br = new BinaryReader(fs, Encoding.Unicode))
                    {
                        User user = null;
                        while (br.PeekChar() > -1)
                        {
                            user = new User();
                            user.SetRankingPlace(br.ReadInt32());
                            user.SetName(br.ReadString());
                            user.SetPassword(br.ReadString());
                            user.SetPoints(br.ReadInt32());
                            user.SetAveragePoint(br.ReadDouble());
                            users.Add(user);
                        }
                    }
                }
            return users;
        }

        // checking login and password -> authorisating user or creating user
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (Authorization.IsChecked == true && LoginCheck(Login.Text) && PasswordCheck(Password.Text))
            {
                if (!CheckAccount(Login.Text, Password.Text))
                {
                    MessageBox.Show($"Dear User, You have {qty_attempts - 1} attempt/ts to enter to account", "Warning window", MessageBoxButton.OK);
                    qty_attempts--;
                }
                else
                {
                    Account.Login = user.GetName();
                    Account.Password = user.GetPassword();
                    Account.Status = 1;
                    AccountCabinet accountCabinet = new AccountCabinet();
                    accountCabinet.Show();
                    this.Close();
                }
                if (qty_attempts == 0)
                {
                    MessageBox.Show("Dear User, You wrote incorrect login or password 3 times, program will be terminated", "Warning window", MessageBoxButton.OK);
                    Application.Current.Shutdown();
                };
                return;
            }
            if (Registration.IsChecked == true && LoginCheck(Login.Text) && PasswordCheck(Password.Text))
            {
                if (CheckNewName(Login.Text))
                {
                    User user = new User(Login.Text, Password.Text);
                    Account.Login = user.GetName();
                    Account.Password = user.GetPassword();
                    Account.Status = 0;
                    AccountCabinet accountCabinet = new AccountCabinet();
                    accountCabinet.Show();
                    this.Close();
                }
            }
        }

        // logic for login checking
        bool LoginCheck(string text)
        {
            Regex regex = new Regex(@"^([A-Z]{1})([a-zA-Z]+)\S*$");
            return regex.IsMatch(text);
        }

        // logic for password checking
        bool PasswordCheck(string text)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]{4}$");
            return regex.IsMatch(text);
        }

        // closing program
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // logic for checking presence of the account with ranking table int the file
        bool CheckAccount(string name, string password)
        {
            User user1 = new User();
            bool coicidence = false;
            if (new FileInfo(users_path).Length > 0 && users != null)
            {
                foreach (User user in users)
                    if (user.GetName() == name)
                    {
                        user1 = user;
                        coicidence = true;
                        break;
                    }
                if (coicidence && user1.GetPassword() == password)
                {
                    this.user = user1;
                    return true;
                }
            }
            return false;
        }

        // checking name/login of the new account. it should be unique
        bool CheckNewName(string name)
        {
            if (new FileInfo(users_path).Length > 0 && users != null)
            {
                foreach (User user in users)
                    if (user.GetName() == name)
                    {
                        MessageBox.Show("Dear User, Your login is presented in the table. Pick a new one.", "Warning window", MessageBoxButton.OK);
                        return false;
                    }
            }
            return true;
        }

    }
}
