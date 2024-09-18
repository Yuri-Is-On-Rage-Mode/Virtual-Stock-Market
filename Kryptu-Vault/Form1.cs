using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kryptu_Vault
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PositionTheLoginGrpBoxAtTheCenter();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PositionTheLoginGrpBoxAtTheCenter();
        }

        public void PositionTheLoginGrpBoxAtTheCenter()
        {
            {
                Point point = new Point();

                int pX = this.Height / (3 / 1);
                int pY = this.Width / (3/ 1);

                point.X = pX - 200;
                point.Y = pY - 200;

                //textBox1.Text = point.ToString();

                Login_Grp_Box.Top = pX;
                Login_Grp_Box.Left = pY;

                Point ActualHaW = new Point();
                ActualHaW.X = this.Height;
                ActualHaW.Y = this.Width;

                //textBox2.Text = ActualHaW.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("tst124pro1234dev"))
            {
                UserConf.WhatUserTypeIsTheUserLoginedWith = 1;
                this.Hide();
                UserConf.PerformUserRedirection();
                //this.Close();
            }
            else if (textBox1.Text.Equals("elonmusk"))
            {
                UserConf.WhatUserTypeIsTheUserLoginedWith = 2;
                this.Hide();
                UserConf.PerformUserRedirection();
                //this.Close();
            }
            else if (textBox1.Text.Equals("nawazsharif"))
            {
                UserConf.WhatUserTypeIsTheUserLoginedWith = 3;
                this.Hide();
                UserConf.PerformUserRedirection();
                //this.Close();
            }
            else
            {
                MessageBox.Show("You Entered The Wrong Password!\n Try looking at the prodev password at the left!");
            }
        }
    }
}
