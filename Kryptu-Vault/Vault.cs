using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;

namespace Kryptu_Vault
{
    public partial class Vault : Form
    {
        public Vault()
        {
            InitializeComponent();
        }

        private void Vault_Load(object sender, EventArgs e)
        {
            UserConf.MoneyIHad = 245_000_000;
            UserConf.MoneyIHave = UserConf.MoneyIHad;
            UserConf.MoneyICanLoan = UserConf.MoneyIHave / 69;
            UserConf.MoneyIHaveInDepth = UserConf.MoneyIHave / 225;
            UserConf.MoneyYouHaveMine = UserConf.MoneyIHave / 299;

            UserConf.MyName = "ElonMusk";

            UpdateAllUserConfStuff();

            dgv_real_transactions.AutoGenerateColumns = false;

            // Adding columns to DataGridView
            dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "From", DataPropertyName = "Sender" });
            dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "To", DataPropertyName = "Receiver" });
            dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Amount", DataPropertyName = "Amount" });
            dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fee", DataPropertyName = "Fees" });
            dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status" });

            for (int i = 1; i < 3; i++)
            {
                double amountNum1 = 1600.00 * i - 227 + (i*Transaction.FeesConstant);
                double feetaken = amountNum1 * Transaction.FeesConstant;

                double amountNum = (1000.00 * i - 227 + (i * Transaction.FeesConstant))-feetaken;


                AddTransactionToDgv1("Matrix", "You", amountNum, "Completed");
                UserConf.MoneyIHave += amountNum;
                StatsTillNow.TotalTransactions += 1;
                StatsTillNow.TotalRecived += amountNum;
                UpdateAllUserConfStuff();

                //StatsTillNow.TotalFeesCharged += feetaken;
            }

            for (int i = 1; i < 2; i++)
            {
                double amountNum1 = 1000.00 * i - 227 + (i * Transaction.FeesConstant);
                double feetaken = amountNum1 * Transaction.FeesConstant;

                double amountNum = (1000.00 * i - 227 + (i * Transaction.FeesConstant)) - feetaken;


                AddTransactionToDgv1("You", "Matrix", amountNum, "Completed");
                UserConf.MoneyIHave -= amountNum;
                StatsTillNow.TotalTransactions += 1;
                StatsTillNow.TotalSended += amountNum;
                UpdateAllUserConfStuff();

                StatsTillNow.TotalFeesCharged += feetaken;
            }

            UserConf.MoneyIHave += UserConf.MoneyIHave - StatsTillNow.TotalFeesCharged;
            UserConf.MoneyIHave += UserConf.MoneyIHave - StatsTillNow.TotalSended;

            //MessageBox.Show($"TOTAL RECIEVED: {StatsTillNow.TotalRecived}\nTOTAL TRANSACTIONS: {StatsTillNow.TotalTransactions}\nTOTAL FEES CHARGED: {StatsTillNow.TotalFeesCharged}");

            // Adding columns to DataGridView
            //dgv_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Transaction Amount", DataPropertyName = "Amount of Transaction Money" });
            //dgv_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fees", DataPropertyName = "Charged Fees of Transacion" });
            //dgv_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Action", DataPropertyName = "Added or Deducted" });
            
            //InitChart();
        }

        public void InitChart()
        {
            // Set chart properties
            chart_transactions.ChartAreas.Add(new ChartArea("MainArea"));

            // Create a series for earnings and losses
            var series = new Series("Earnings and Losses")
            {
                ChartType = SeriesChartType.Line, // or use Bar, Column, etc.
                XValueType = ChartValueType.String, // X-axis is for actions like 'added++' or 'deducted--'
                YValueType = ChartValueType.Double // Y-axis is for amount
            };

            chart_transactions.Series.Add(series);
            chart_transactions.Legends.Add(new Legend("Legend"));
            chart_transactions.Series["Earnings and Losses"].Color = System.Drawing.Color.Blue;

        }
        public void AddTransactionToChart(double amount, string action)
        {
            // Get the series for the chart
            Series series = chart_transactions.Series["Earnings and Losses"];

            // Add the new point to the chart
            if (action == "added++")
            {
                series.Points.AddXY("Transaction " + (series.Points.Count + 1), amount);
                series.Points[series.Points.Count - 1].Color = System.Drawing.Color.Green; // Green for earnings
            }
            else
            {
                series.Points.AddXY("Transaction " + (series.Points.Count + 1), -amount);
                series.Points[series.Points.Count - 1].Color = System.Drawing.Color.Red; // Red for losses
            }

            // Customize the points' appearance (optional)
            series.Points[series.Points.Count - 1].MarkerStyle = MarkerStyle.Circle;
            series.Points[series.Points.Count - 1].MarkerSize = 10;
            series.Points[series.Points.Count - 1].Font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);

            // Remove older points if there are more than 25
            if (series.Points.Count > 25)
            {
                // Remove the oldest points
                for (int i = 0; i < series.Points.Count - 25; i++)
                {
                    series.Points.RemoveAt(0); // Remove the first point (oldest)
                }
            }
        }


        public void UpdateAllUserConfStuff()
        {
            // Update the username labels
            userconf_lbl_username.Text = UserConf.MyName + "'s Kryptu Account";
            lbl_user_name_dashboard.Text = UserConf.MyName + "'s Kryptu Account";

            // Update the bank balance
            lbl_bank_balance.Text = "$" + UserConf.MoneyIHave.ToString();

            // Update the dept, loan, and other fields, checking for negative values
            userconf_lbl_my_dept.Text = UserConf.MoneyIHaveInDepth >= 0 ? UserConf.MoneyIHaveInDepth.ToString() : "---";
            userconf_lbl_my_loan.Text = UserConf.MoneyYouHaveMine >= 0 ? UserConf.MoneyYouHaveMine.ToString() : "---";

            // Calculate losses and recovered losses
            var losses = UserConf.MoneyIHad - UserConf.MoneyIHave;
            var recoveredLosses = losses - UserConf.MoneyIHave;

            userconf_lbl_my_losses.Text = losses >= 0 ? losses.ToString() : "---";
            userconf_lbl_my_recovered_losses.Text = recoveredLosses >= 0 ? recoveredLosses.ToString() : "---";

            // Calculate and update earnings
            var earnings = UserConf.MoneyIHave - UserConf.MoneyIHad;
            userconf_lbl_my_earnings.Text = earnings >= 0 ? earnings.ToString() : "---";

            userconf_tans_fees.Text = StatsTillNow.TotalFeesCharged.ToString();
            userconf_trans_amount.Text = (StatsTillNow.TotalSended + StatsTillNow.TotalRecived).ToString();
            lbl_total_trans.Text = StatsTillNow.TotalTransactions.ToString();

            userconf_recived_trans.Text = StatsTillNow.TotalRecived.ToString();
            userconf_sended_trans.Text = StatsTillNow.TotalSended.ToString();

            userconf_result_trans_amount.Text = (StatsTillNow.TotalRecived - StatsTillNow.TotalSended).ToString();


            // send transaction stuff
            if (UserConf.MoneyIHave > 0)
            {
                trans_send_btn_send.Enabled = true;
            }
            else 
            {
                trans_send_btn_send.Enabled = false;
            }

            //trans_send_lbl_amount.Text = (10).ToString();
            trans_send_trackbar_amount.Minimum = 0;
            trans_send_trackbar_amount.Maximum = 100;
        }


        public void AddTransactionToDgv1(string from, string to, double amount, string status)
        {

            // Calculate fee (assuming 2% of the amount as an example)
            double fee = amount * Transaction.FeesConstant;

            // Check if columns already exist; only add them if they don't
            if (dgv_real_transactions.Columns.Count == 0)
            {
                dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "From", DataPropertyName = "Sender" });
                dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "To", DataPropertyName = "Receiver" });
                dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Amount", DataPropertyName = "Amount" });
                dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fee", DataPropertyName = "Fee" });
                dgv_real_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status" });
            }

            // Add the new row with the input data
            int rowIndex = dgv_real_transactions.Rows.Add(from, to, amount.ToString("F2"), fee.ToString("F2"), status);

            if (to.ToLower() == "you")
            {
                AddTransactionToDgv2(amount, "added++");
            }
            else 
            {
                AddTransactionToDgv2(amount, "deducted++");
            }

            // If the status is "Completed", set the text color to green
            //if (status == "Completed")
            //{
            //    dgv_real_transactions.Rows[rowIndex].Cells["Status"].Style.ForeColor = System.Drawing.Color.Green;
            //}
        }

        public void AddTransactionToDgv2(double amount, string action)
        {
            // Calculate fee (assuming 2% of the amount as an example)
            double fee = amount * Transaction.FeesConstant;

            // Check if columns already exist; only add them if they don't
            if (dgv_transactions.Columns.Count == 0)
            {
                dgv_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Transaction Amount", DataPropertyName = "Amount of Transaction Money" });
                dgv_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fees", DataPropertyName = "Charged Fees of Transaction" });
                dgv_transactions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Action", DataPropertyName = "Added or Deducted" });
            }

            // Add the new row with the input data
            int rowIndex = dgv_transactions.Rows.Add(amount.ToString("F2"), fee.ToString("F2"), action);

            // Highlight the row based on the action value
            if (action == "added++")
            {
                // Set row background color to bright light green for "added++"
                dgv_transactions.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                // Set row background color to red for any other action
                dgv_transactions.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            }

            // Set the font to bold and bright
            dgv_transactions.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font(dgv_transactions.DefaultCellStyle.Font, System.Drawing.FontStyle.Bold);
            dgv_transactions.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black; // Set the font color to bright yellow

            // Scroll to the last entry (the newly added row)
            dgv_transactions.FirstDisplayedScrollingRowIndex = rowIndex;

            // Optionally, select the newly added row (if you want the row to appear highlighted)
            dgv_transactions.ClearSelection();
            dgv_transactions.Rows[rowIndex].Selected = true;

            AddTransactionToChart(amount, action);
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void trans_send_btn_send_Click(object sender, EventArgs e)
        {
            // Try to parse the amount from the label's text
            if (double.TryParse(trans_send_lbl_amount.Text, out double amount))
            {
                // Ensure the amount is valid
                if (amount >= 0 && amount <= UserConf.MoneyIHave)
                {
                    if ((UserConf.MoneyIHave - amount) <= 0)
                    {
                        // Handle invalid amount (e.g., negative amount or more than current balance)
                        MessageBox.Show("Insfficient amount. Please check the amount and try again.");
                    }
                    else 
                    {
                        // Deduct the amount from the user's balance
                        UserConf.MoneyIHave -= amount;

                        // Update all user-related information
                        UpdateAllUserConfStuff();
                        AddTransactionToDgv1("You", "Matrix", amount, "Completed");
                    }
                }
                else
                {
                    // Handle invalid amount (e.g., negative amount or more than current balance)
                    MessageBox.Show("Insufficient amount. Please check the amount and try again.");
                }
            }
            else
            {
                // Handle parsing errors
                MessageBox.Show("Invalid amount format.");
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Handle changes in the percentage TextBox
            if (double.TryParse(trans_send_txtr_percent.Text, out double percentage))
            {
                if (percentage >= 0 && percentage <= 100)
                {
                    // Calculate the amount based on the percentage
                    double amount_of_percent_x = (percentage / 100) * UserConf.MoneyIHave;

                    // Update the amount label
                    trans_send_lbl_amount.Text = amount_of_percent_x.ToString("F9"); // Formatting to 9 decimal places

                    trans_send_trackbar_amount.Value = (int)(percentage);
                }
                else
                {
                    // Handle invalid percentage values
                    MessageBox.Show("Invalid Percentage");
                }
            }
            else
            {
                // Handle non-numeric input
                MessageBox.Show("Invalid Input");
            }
        }

        private void trans_send_lbl_amount_TextChanged(object sender, EventArgs e)
        {
            // Handle changes in the amount label
            if (double.TryParse(trans_send_lbl_amount.Text, out double amount))
            {
                if (amount >= 0 && amount <= UserConf.MoneyIHave)
                {
                    // Calculate the percentage based on the amount
                    double amount_of_percent_x = (amount / UserConf.MoneyIHave) * 100;

                    // Update the percentage TextBox
                    trans_send_txtr_percent.Text = amount_of_percent_x.ToString("F9"); // Formatting to 9 decimal places

                    trans_send_trackbar_amount.Value = (int)(amount_of_percent_x);
                }
                else
                {
                    // Handle invalid amount values
                    MessageBox.Show("Invalid Amount");
                }
            }
            else
            {
                // Handle non-numeric input
                MessageBox.Show("Invalid Input");
            }
        }

        private void trans_send_trackbar_amount_Scroll(object sender, EventArgs e)
        {
            // Handle changes in the TrackBar
            int trackBarValue = trans_send_trackbar_amount.Value;

            // Assuming TrackBar has a range from 0 to 100 (representing percentage)
            double percentage = trackBarValue;

            // Update the percentage TextBox
            trans_send_txtr_percent.Text = percentage.ToString("F9"); // Formatting to 9 decimal places

            // Calculate the amount based on the TrackBar value (percentage)
            double amount_of_percent_x = (percentage / 100) * UserConf.MoneyIHave;

            // Update the amount label
            trans_send_lbl_amount.Text = amount_of_percent_x.ToString("F9"); // Formatting to 9 decimal places
        }

        private void chart_transactions_Click(object sender, EventArgs e)
        {

        }
    }
}
