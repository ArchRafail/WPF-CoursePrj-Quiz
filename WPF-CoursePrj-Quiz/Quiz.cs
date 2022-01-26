using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CoursePrj_Quiz
{
    class Quiz
    {
        string question;
        string[] answers;
        int right_answer;
        string right_string;


        public Quiz()
        {
            question = null;
            answers = new string[2] { "", "" };
            right_answer = 0;
            right_string = null;
        }

        public Quiz(Quiz quiz)
        {
            this.question = quiz.question;
            this.answers = quiz.answers;
            this.right_answer = quiz.right_answer;
            this.right_string = quiz.right_string;
        }

        public void SetQuestion(string text) { question = text; }
        public void SetAnswer1(string text) { answers[0] = text; }
        public void SetAnswer2(string text) { answers[1] = text; }
        public string GetAnswer1() => answers[0];
        public string GetAnswer2() => answers[1];
        public void SetRightAnswer(int number) { right_answer = number; }
        public void SetRightString(string text) { right_string = text; }
        public int GetRightAnswer() => right_answer;
        public string GetRightString() => right_string;

        public override string ToString()
        {
            return $"{question}\n";
        }
    }
}
