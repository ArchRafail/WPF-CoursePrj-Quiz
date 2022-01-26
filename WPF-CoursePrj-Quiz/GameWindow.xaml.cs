using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPF_CoursePrj_Quiz
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        List<Quiz> quizzes;
        List<User> users;
        User user;
        Random random = new Random();
        int countDown;                      // time for answers in seconds
        DispatcherTimer timer;
        TimeSpan timeSpan;                  // set time into timer for countdown timing
        bool answerSelected;
        string quizzes_path = "Quiz And Answers.txt";
        string users_path = "Users.bin";
        int question_count;                 // counting questions to prevent overanswering
        int total_questions;
        SolidColorBrush ButtonBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDDDDD"));
        int points;
        int difficult;
        int coefficient;                    // coefficient that inversely proportional to difficult level
        TextBlock answerHall = null;

        // constructor with setting default properties, loading quizzes and list of users
        public GameWindow()
        {
            InitializeComponent();
            quizzes = new List<Quiz>();
            User user1 = new User();
            this.users = new List<User> { };
            this.users = GetUsers();
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
            this.quizzes = Loading_quizzes();
            for (int i = 0; i < quizzes.Count; i++)
            {
                Quiz tmp = quizzes[i];
                quizzes.RemoveAt(i);
                quizzes.Insert(random.Next(quizzes.Count), tmp);
            }
            
            MainButton.Content = "Start Game";
            MainButton.Background = Brushes.LightGreen;

            countDown = 0;
            question_count = 0;
            points = 0;
            coefficient = 0;

            Answer1.IsEnabled = false;
            Answer2.IsEnabled = false;
            Answer3.IsEnabled = false;
            Answer4.IsEnabled = false;
        }

        // loading ranking table with list of users from the file
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

        // loading quizzes from the file
        private List<Quiz> Loading_quizzes()
        {
            List<Quiz> quizzes = new List<Quiz> { };
            using (StreamReader sr = new StreamReader(quizzes_path, System.Text.Encoding.Default))
            {
                string line;
                int i = 0;
                Quiz quiz = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (i == 0)
                        quiz = new Quiz();
                    switch (i)
                    {
                        case 0:
                            quiz.SetQuestion(line);
                            break;
                        case 1:
                            quiz.SetAnswer1(line);
                            break;
                        case 2:
                            quiz.SetAnswer2(line);
                            break;
                        case 3:
                            quiz.SetRightAnswer(int.Parse(line));
                            break;
                        case 4:
                            quiz.SetRightString(line);
                            break;
                    }
                    i++;
                    if (i == 5)
                    {
                        quizzes.Add(quiz);
                        i = 0;
                    }
                }
            }
            return quizzes;
        }

        // method for saving progress with ranking table into the file
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
            MessageBox.Show("File with user's ranking system was updated.", "Information window", MessageBoxButton.OK);
        }

        // Start game button and all logic of the game
        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            if (coefficient == 0)
            {
                if (Difficult1.IsChecked == true)
                {
                    difficult = 1;
                    coefficient = 3;
                    total_questions = 10;
                }
                else if (Difficult2.IsChecked == true)
                {
                    difficult = 2;
                    coefficient = 2;
                    total_questions = 20;
                }
                else
                {
                    difficult = 3;
                    coefficient = 1;
                    total_questions = 30;
                }

                GameDifficult.IsEnabled = false;
                GameDifficult.Visibility = Visibility.Collapsed;

                Variant50_50.IsEnabled = true;
                HallHelp.IsEnabled = true;

                MainButton.Content = "Next Question";
                MainButton.Background = ButtonBrush;
            }
            countDown = 60 * coefficient / 3;
            answerSelected = false;
            question_count++;
            SetButtonStatus();

            if (answerHall != null)
                answerHall.Visibility = Visibility.Collapsed;

            if (question_count > total_questions)
                SavingProgress();

            ButtonsClear();
            QuestionsBlock.Text = quizzes[question_count].ToString();
            string[] words = (quizzes[question_count].GetAnswer1()).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int k = 0;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "2.")
                {
                    k = i;
                    break;
                }
                else
                    Answer1.Content = Answer1.Content + words[i] + " ";
            }
            Answer1.Content = Answer1.Content.ToString().Remove(Answer1.Content.ToString().LastIndexOf(" "));
            for (int i = k; i < words.Length; i ++)
                Answer2.Content = Answer2.Content + words[i] + " ";
            Answer2.Content = Answer2.Content.ToString().Remove(Answer2.Content.ToString().LastIndexOf(" "));
            words = null;
            words = (quizzes[question_count].GetAnswer2()).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "4.")
                {
                    k = i;
                    break;
                }
                else
                    Answer3.Content = Answer3.Content + words[i] + " ";
            }
            Answer3.Content = Answer3.Content.ToString().Remove(Answer3.Content.ToString().LastIndexOf(" "));
            for (int i = k; i < words.Length; i++)
                Answer4.Content = Answer4.Content + words[i] + " ";
            Answer4.Content = Answer4.Content.ToString().Remove(Answer4.Content.ToString().LastIndexOf(" "));
            MainButton.IsEnabled = false;
            timeSpan = TimeSpan.FromSeconds(countDown);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        // checking answer, checkin timer
        private void Timer_Tick(object sender, EventArgs e)
        {
            Timer.Text = timeSpan.ToString("c");
            if (timeSpan.Seconds <= 10)
                Timer.Foreground = Brushes.Red;
            else
                Timer.Foreground = Brushes.Black;
            if (timeSpan == TimeSpan.Zero)
            {
                (sender as DispatcherTimer).Stop();
                MessageBox.Show($"Dear {user.GetName()}, your time is off.");
                if (points == 0)
                {
                    AccountCabinet accountCabinet = new AccountCabinet();
                    accountCabinet.Show();
                    this.Close();
                }
                else
                    SavingProgress();
            }
            else
            {
                if (answerSelected == true)
                    (sender as DispatcherTimer).Stop();
                else
                    timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
            }
        }

        // setting 4 answers buttons to default state
        private void ButtonsClear()
        {
            Answer1.Background = ButtonBrush;
            Answer2.Background = ButtonBrush;
            Answer3.Background = ButtonBrush;
            Answer4.Background = ButtonBrush;
            Answer1.Content = "";
            Answer2.Content = "";
            Answer3.Content = "";
            Answer4.Content = "";
        }

        // 4 methods: lightning of the correct answer - Light Green color if no - Orange Red color
        private void Answer1_Click(object sender, RoutedEventArgs e)
        {
            LockingButton();
            answerSelected = true;
            if (quizzes[question_count].GetRightAnswer() == 1)
            {
                Answer1.Background = Brushes.LightGreen;
                points += 10;
            }
            else
            {
                Answer1.Background = Brushes.OrangeRed;
                SetCorrectAnswer();
            }
            MainButton.IsEnabled = true;
        }

        private void Answer2_Click(object sender, RoutedEventArgs e)
        {
            LockingButton();
            answerSelected = true;
            if (quizzes[question_count].GetRightAnswer() == 2)
            {
                Answer2.Background = Brushes.LightGreen;
                points += 10;
            }
            else
            {
                Answer2.Background = Brushes.OrangeRed;
                SetCorrectAnswer();
            }
            MainButton.IsEnabled = true;
        }

        private void Answer3_Click(object sender, RoutedEventArgs e)
        {
            LockingButton();
            answerSelected = true;
            if (quizzes[question_count].GetRightAnswer() == 3)
            {
                Answer3.Background = Brushes.LightGreen;
                points += 10;
            }
            else
            {
                Answer3.Background = Brushes.OrangeRed;
                SetCorrectAnswer();
            }
            MainButton.IsEnabled = true;
        }

        private void Answer4_Click(object sender, RoutedEventArgs e)
        {
            LockingButton();
            answerSelected = true;
            if (quizzes[question_count].GetRightAnswer() == 4)
            {
                Answer4.Background = Brushes.LightGreen;
                points += 10;
            }
            else
            {
                Answer4.Background = Brushes.OrangeRed;
                SetCorrectAnswer();
            }
            MainButton.IsEnabled = true;
        }

        // lightning of the correct answer - light green color
        private void SetCorrectAnswer()
        {
            switch (quizzes[question_count].GetRightAnswer())
            {
                case 1:
                    Answer1.Background = Brushes.LightGreen;
                    break;
                case 2:
                    Answer2.Background = Brushes.LightGreen;
                    break;
                case 3:
                    Answer3.Background = Brushes.LightGreen;
                    break;
                case 4:
                    Answer4.Background = Brushes.LightGreen;
                    break;
            }
        }

        // buttons have to be locked due to user can pick up 2 and more qustions
        private void LockingButton()
        {
            Answer1.IsEnabled = false;
            Answer2.IsEnabled = false;
            Answer3.IsEnabled = false;
            Answer4.IsEnabled = false;
        }

        // the status of the buttons will setted each move
        private void SetButtonStatus()
        {
            Answer1.IsEnabled = true;
            Answer2.IsEnabled = true;
            Answer3.IsEnabled = true;
            Answer4.IsEnabled = true;

            Answer1.Visibility = Visibility.Visible;
            Answer2.Visibility = Visibility.Visible;
            Answer3.Visibility = Visibility.Visible;
            Answer4.Visibility = Visibility.Visible;

            Answer1.Background = Brushes.LightGray;
            Answer2.Background = Brushes.LightGray;
            Answer3.Background = Brushes.LightGray;
            Answer4.Background = Brushes.LightGray;
        }

        // 50% from the wrong answers will disappear
        private void Variant50_50_Click(object sender, RoutedEventArgs e)
        {
            int[] answers = new int[] { 1, 2, 3, 4 };
            for (int i = 0; i < answers.Length; i++)
                if (answers[i] == quizzes[question_count].GetRightAnswer())
                {
                    int j = answers[i];
                    answers[i] = answers[answers.Length - 1];
                    answers[answers.Length - 1] = j;
                    break;
                }
            int[] random_answer = new int[2] { 0, 0 };
            random_answer[0] = answers[random.Next(0, 2)];
            do
            {
                random_answer[1] = answers[random.Next(0, 2)];
            } while (random_answer[0] == random_answer[1]);
            for (int i = 0; i < random_answer.Length; i++)
                switch (random_answer[i])
                {
                    case 1:
                        Answer1.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        Answer2.Visibility = Visibility.Collapsed;
                        break;
                    case 3:
                        Answer3.Visibility = Visibility.Collapsed;
                        break;
                    case 4:
                        Answer4.Visibility = Visibility.Collapsed;
                        break;
                }
            Variant50_50.IsEnabled = false;
        }

        // random answer as from Hall for the questions. user have to press as for him/her mind!
        private void HallHelp_Click(object sender, RoutedEventArgs e)
        {
            answerHall = new TextBlock();
            answerHall.FontSize = 24;
            answerHall.FontFamily = new FontFamily("Gabriola");
            answerHall.Margin = new Thickness(10, 10, 0, 0);
            Options.Children.Add(answerHall);
            int[] answers = new int[] { 1, 2, 3, 4 };
            int random_answer = answers[random.Next(0, 3)];
            answerHall.Text = $"We think that correct answer is {random_answer}. Yeah, it is right!";
            HallHelp.IsEnabled = false;
        }

        // saving the progress to the ranking table
        private void SavingProgress()
        {
            user.SetPoints(points);
            double averagepoint = difficult * ((question_count-1) * 10 * 0.6 + points * 0.4) / (total_questions);
            user.SetAveragePoint(averagepoint);

            bool checkinsert = false;
            if (users.Count != 0)
            {
                foreach (User user in users)
                {
                    if (this.user.GetName() == user.GetName())
                    {
                        user.SetPoints(this.user.GetPoints());
                        user.SetAveragePoint(this.user.GetAveragePoints());
                        checkinsert = true;
                        break;
                    }
                }
            }
            if (!checkinsert)
                users.Add(this.user);
            users.Sort((a, b) => -a.GetAveragePoints().CompareTo(b.GetAveragePoints()));
            int j = 1;
            foreach (User user in users)
                user.SetRankingPlace(j++);
            SaveUsers(this.users);
            Account.Status = 1;
            AccountCabinet accountCabinet = new AccountCabinet();
            accountCabinet.Show();
            this.Close();
        }

        // returning to the Account Window with saving progress
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (points == 0)
            {
                AccountCabinet accountCabinet = new AccountCabinet();
                accountCabinet.Show();
                this.Close();
            }
            else 
                SavingProgress();
        }

        // closing programm
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
