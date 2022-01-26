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
    /// Interaction logic for AccountCabinet.xaml
    /// </summary>
    public partial class AccountCabinet : Window
    {
        private List<User> users;
        private User user;
        private string users_path = "Users.bin";

        public AccountCabinet()
        {
            InitializeComponent();
            User user1 = new User();
            users = new List<User>();
            users = GetUsers();
            if (Account.Status == 1)
            {
                foreach (User user in users)
                    if (user.GetName() == Account.Login)
                    {
                        user1 = user;
                        break;
                    }
                this.user = user1;
            }
            else
                this.user = new User(Account.Login, Account.Password);
            ShowTable();
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

        // method for showing ranking table on the window. if there is no table - mathod shows message
        private void ShowTable()
        {
            RankingTable.Text = "";
            string newtext = "";
            if (users.Count > 0)
                foreach (User i in users)
                    newtext = newtext + i.ToString();
            if (newtext == null)
            {
                RankingTable.Text = $"Dear {this.user.GetName()} no any records in the table.\nPlease, get fun and play a new game.";
            }
            else
            {
                RankingTable.Text += newtext;
            }
        }

        // updating table
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            ShowTable();
        }

        // method for changing password of the user
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (Account.Status == 0)
            {
                MessageBox.Show($"Dear {user.GetName()}, the password can not be changed. Firstly play the game!", "Warning information", MessageBoxButton.OK);
                OldPassword.Text = "";
                NewPassword.Text = "";
                return;
            }
            if (OldPassword.Text == user.GetPassword())
            {
                if (PasswordCheck(NewPassword.Text))
                {
                    foreach (User user in users)
                        if (user.GetName() == this.user.GetName())
                        {
                            this.user.SetPassword(NewPassword.Text);
                            user.SetPassword(NewPassword.Text);
                            break;
                        }
                    SaveUsers(users);
                    Account.Password = user.GetPassword();
                    MessageBox.Show($"Dear {user.GetName()}, the password was updated.", "Warning information", MessageBoxButton.OK);
                }
                else
                    MessageBox.Show($"Dear {user.GetName()}, the new password does not correspond to the rules.", "Warning information", MessageBoxButton.OK);
            }
            else
                MessageBox.Show($"Dear {user.GetName()}, the old password is incorrect.", "Warning information", MessageBoxButton.OK);
            OldPassword.Text = "";
            NewPassword.Text = "";
        }

        // logic for password checking
        bool PasswordCheck(string text)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]{4}$");
            return regex.IsMatch(text);
        }

        // method for saving new password with ranking table into the file
        private void SaveUsers(List<User> users)
        {
            using (FileStream fs = new FileStream(users_path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Unicode))
                {
                    foreach (User user in users)
                    {
                        bw.Write(user.GetRankingPlace());
                        bw.Write(user.GetName());
                        bw.Write(user.GetPassword());
                        bw.Write(user.GetPoints());
                        bw.Write(user.GetAveragePoints());
                    }
                }
            }
        }

        // method for starting a new game
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            GameWindow game = new GameWindow();
            game.Show();
            this.Close();
        }

        // closing program
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
