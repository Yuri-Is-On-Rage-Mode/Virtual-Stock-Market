using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kryptu_Vault
{
    public class UserConf
    {
        public static int WhatUserTypeIsTheUserLoginedWith = new int();

        public static double MoneyIHave = new double();
        public static double MoneyICanLoan = new double();
        public static double MoneyIHaveInDepth = new double();
        public static double MoneyYouHaveMine = new double();

        public static double MoneyIHad = new double();

        public static string MyName = "";

        public static void PerformUserRedirection()
        {
            if (WhatUserTypeIsTheUserLoginedWith == 1)
            {
                MoneyIHave = 45_000_000;
                MoneyICanLoan = MoneyIHave / 69;
                MoneyIHaveInDepth = MoneyIHave / 225;
                MoneyYouHaveMine = MoneyIHave / 299;

                MyName = "ElonMusk Junior";
            }
            else if (WhatUserTypeIsTheUserLoginedWith == 2)
            {
                MoneyIHave = 245_000_000;
                MoneyICanLoan = MoneyIHave / 69;
                MoneyIHaveInDepth = MoneyIHave / 225;
                MoneyYouHaveMine = MoneyIHave / 299;

                MyName = "ElonMusk";
            }
            else if (WhatUserTypeIsTheUserLoginedWith == 3)
            {
                MoneyIHave = 1_000_000_000_000_000;
                MoneyICanLoan = MoneyIHave / 5;
                MoneyIHaveInDepth = MoneyIHave / 1;
                MoneyYouHaveMine = MoneyIHave / 1;

                MyName = "Nawaz Sharif (Worlds No:1 Criminal)";
            }
            else
            {
                MessageBox.Show("UserConf: You Entered The Wrong UserType!\n Try looking at the prodev password in the login form at your left!");
            }
            Vault vInterface = new Vault();
            vInterface.Show();
        }
    }
}
