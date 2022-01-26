using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CoursePrj_Quiz
{
    class User
    {
        private int ranking_place;
        private string name;
        private string password;
        private int points;
        private double average_points;

        public User()
        {
            ranking_place = 0;
            name = null;
            password = null;
            points = 0;
            average_points = 0;
        }

        public User(string name, string password)
        {
            ranking_place = 0;
            this.name = name;
            this.password = password;
            points = 0;
            average_points = 0;
        }

        public void SetRankingPlace(int number) { ranking_place = number; }
        public void SetName(string text) { name = text; }
        public void SetPassword(string text) { password = text; }
        public void SetPoints(int number) { points = number; }
        public void SetAveragePoint(double number) { average_points = number; }
        public int GetRankingPlace() => ranking_place;
        public string GetPassword() => password;
        public string GetName() => name;
        public int GetPoints() => points;
        public double GetAveragePoints() => average_points;
        public override string ToString()
        {
            return $"{ranking_place}\t\t{name}\t\t{points}\t\t{average_points}\n";
        }


    }
}
