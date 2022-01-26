using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CoursePrj_Quiz
{
    static public class Account
    {
        public static string Login { get; set; }
        public static string Password { get; set; }

        public static int status;         //status: 0-new 1-old
        public static int Status {
            get => status;
            set
            {
                if (value == 0)
                    status = value;
                else
                    status = 1;
            }
        }
    }
}
