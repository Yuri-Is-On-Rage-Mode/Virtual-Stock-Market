using Kryptu_Vault.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using System.IO;
using Newtonsoft.Json;


namespace Kryptu_Vault
{
    public partial class KryptoMercat : Form
    {
        private CancellationTokenSource _cancellationTokenSource;

        public KryptoMercat()
        {
            InitializeComponent();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void KryptoMercat_Load(object sender, EventArgs e)
        {
            #region TESTING ASSIGN STOCK MARKET
            StockMarket.InitMarket("HAMZA KI STOCK MARKET", "HkSM", 90);
            #endregion

            #region INITIAL CONFIG
            dgv_real_transactions.AutoGenerateColumns = false;
            dgv_transactions.AutoGenerateColumns = false;

            dgv_companies.AutoGenerateColumns = false;
            dgv_stats_market.AutoGenerateColumns = false;

            mARKETNAMEToolStripMenuItem.Text = StockMarket.MarketName.ToString();
            market_current_price_toolstripmenoitem.Text = StockMarket.MarketLogo.ToString();

            UpdateDgvStats();


            TestSimulateStockMarket();
        }

        public void TestSimulateStockMarket()
        {
            timer1.Interval = 500; // 1 second
            timer1.Enabled = true;  // Start the timer
            timer1.Start();         // Alternative way to start the timer
        }

        #region Main Market Chart
        public void UpdateMainMarketChart(double amount, string action)
        {
            // Get the series for the chart
            Series series = chart_main_market.Series["SerMainChartMarket"];
            //series.ShadowOffset = 0;

            // Create a new point label for the chart
            string pointLabel = "@ " + (series.Points.Count + 1);

            // Determine point color and shadow color based on the action (earnings or losses)
            Color pointColor = action == "added++" ? Color.Green : Color.Red;
            series.ShadowColor = pointColor;

            // Add a new point to the series with the calculated label and amount
            series.Points.AddXY(pointLabel, amount);
            DataPoint newPoint = series.Points[series.Points.Count - 1];

            // Set the color of the new point
            newPoint.Color = pointColor;

            // Customize the appearance of the new point
            newPoint.MarkerStyle = MarkerStyle.Circle;
            newPoint.MarkerSize = 3;
            newPoint.Font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);

            // Update the stock market value
            StockMarket.Market_Value_Now = amount;

            // Remove older points if there are more than 169
            if (series.Points.Count > 69)
            {
                series.Points.RemoveAt(0); // Remove the first point (oldest)
            }

            // Update the Y-axis range based on the new data
            UpdateYAxisRange(series);
        }


        private void UpdateYAxisRange(Series series)
        {
            // Initialize min and max values
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            // Loop through data points to find the min and max Y-values
            foreach (DataPoint point in series.Points)
            {
                if (point.YValues[0] < minY)
                    minY = point.YValues[0];
                if (point.YValues[0] > maxY)
                    maxY = point.YValues[0];
            }

            // Ensure minY is less than maxY
            if (minY == maxY)
            {
                // If all values are the same, set a default range
                minY -= 1;
                maxY += 1;
            }
            else
            {
                // Add padding to the range
                double padding = (maxY - minY) * 0.1; // 10% padding
                minY -= padding;
                maxY += padding;
            }

            // Ensure the Y-axis range is valid
            if (minY < maxY)
            {
                var chartArea = chart_main_market.ChartAreas[0];
                chartArea.AxisY.Minimum = minY;
                chartArea.AxisY.Maximum = maxY;

                // Optional: Auto-scale the X-axis based on the number of points
                if (series.Points.Count > 0)
                {
                    chartArea.AxisX.Minimum = 0;
                    chartArea.AxisX.Maximum = series.Points.Count;
                }
            }
            else
            {
                // Log or handle the error
                MessageBox.Show("Invalid axis range: minY is not less than maxY");
            }
        }


        private void chart_main_market_Click(object sender, EventArgs e)
        {
            //UpdateMainMarketChart()
        }
        #endregion

        #region Stats Chart (Left)

        public static int HowManyTimesMarketCrashedAgain = 0;
        public static int HowManyTimesMarketBulledAgain = 0;
        public static int HowManyTimesMarketDeadAgain = 0;

        // Add variables to track all time high and low
        private static double allTimeHigh = double.MinValue;
        private static double allTimeLow = double.MaxValue;
        public void UpdateDgvStats()
        {
            // Check if columns already exist; only add them if they don't
            if (dgv_stats_market.Columns.Count == 0)
            {
                dgv_stats_market.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "k", DataPropertyName = "Item" });
                dgv_stats_market.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "v", DataPropertyName = "Value" });
            }

            if (dgv_output.Columns.Count == 0)
            {
                dgv_output.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "@", DataPropertyName = "What is this?" });
                dgv_output.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Message", DataPropertyName = "Message" });
            }

            // Add the new row with the input data
            dgv_stats_market.Rows.Clear();
            dgv_output.Rows.Clear();

            Dictionary<string, string> statItems = new Dictionary<string, string>();
            Dictionary<string, string> Messages = new Dictionary<string, string>();

            statItems["Market Cap"] = StockMarket.Market_Value_Now.ToString();

            // Determine market status
            string value_Of_Stat = "";
            if ((StockMarket.Market_Value_Now / StockMarket.UniversalStockMarketCapRate) >= StockMarket.UniversalStockMarketBullCapPrice)
            {
                value_Of_Stat = "Bull";
            }
            else if ((StockMarket.Market_Value_Now * StockMarket.UniversalStockMarketCapRate) <= StockMarket.UniversalStockMarketCapPrice)
            {
                value_Of_Stat = "Bear";
            }
            else
            {
                value_Of_Stat = "Stable";
            }

            statItems["Market Stat"] = value_Of_Stat;

            // Track market crashes and bulls
            long amount_till_market_crash = (long)(StockMarket.Market_Value_Now * StockMarket.UniversalStockMarketCapRate);
            if (amount_till_market_crash <= StockMarket.UniversalStockMarketCapPrice)
            {
                if (amount_till_market_crash < 0)
                {
                    statItems["Amount t.m.c"] = amount_till_market_crash.ToString();
                    HowManyTimesMarketDeadAgain++;
                    Messages["FIRETRUCK"] = $"Market was killed {HowManyTimesMarketDeadAgain} time! at a price of {StockMarket.Market_Value_Now}, just {((StockMarket.Market_Value_Now) - StockMarket.UniversalStockMarketCapPrice)} from the red line!";
                }
                else
                {
                    statItems["Amount t.m.c"] = amount_till_market_crash.ToString();
                    HowManyTimesMarketCrashedAgain++;
                    Messages["CRITICAL"] = $"Market Crashed {HowManyTimesMarketCrashedAgain} time! at a price of {StockMarket.Market_Value_Now}, just {(StockMarket.Market_Value_Now) - StockMarket.UniversalStockMarketCapPrice} from the red line!";
                }
            }
            else
            {
                statItems["Amount t.m.c"] = amount_till_market_crash.ToString();
            }

            long amount_till_market_bull = (long)(StockMarket.Market_Value_Now / StockMarket.UniversalStockMarketCapRate);
            if (amount_till_market_bull >= StockMarket.UniversalMatrixBugStockMarketCapPrice)
            {
                statItems["Amount t.mat.b"] = amount_till_market_bull.ToString();
                Messages["MARKET BULL"] = $"Market was Bull {StockMarket.UniversalMatrixBugStockMarketCapPrice} with a price of {StockMarket.Market_Value_Now}";
            }
            else
            {
                statItems["Amount t.mat.b"] = amount_till_market_bull.ToString();
            }

            // Track all-time high and low
            if (StockMarket.Market_Value_Now > StockMarket.AllTimeHigh)
            {
                StockMarket.AllTimeHigh = StockMarket.Market_Value_Now;
                Messages["NEW HIGH"] = $"New All Time High: {StockMarket.AllTimeHigh}";
            }

            if (StockMarket.Market_Value_Now < StockMarket.AllTimeLow)
            {
                StockMarket.AllTimeLow = StockMarket.Market_Value_Now;
                Messages["NEW LOW"] = $"New All Time Low: {StockMarket.AllTimeLow}";
            }

            // Take loan if market price is 0
            if (StockMarket.Market_Value_Now <= (25/100 * StockMarket.Market_StarterPrice))
            {
                // Example loan amount; adjust as necessary
                double loanAmount = StockMarket.LoanLimit;
                StockMarket.Loans.Add(new Loan { Amount = loanAmount, DateTaken = DateTime.Now });
                Messages["LOAN TAKEN"] = $"Loan of {loanAmount} taken because market price hit 0!";
            }
            // Repay loan if market price >= 120% of Market_StarterPrice
            else if (StockMarket.Market_Value_Now >= 1.2 * StockMarket.Market_StarterPrice)
            {
                // Repay all loans
                double totalRepayAmount = StockMarket.Loans.Sum(loan => loan.Amount);
                StockMarket.Loans.Clear(); // Clear all loans
                Messages["LOAN REPAID"] = $"Loan of {totalRepayAmount} repaid because market price reached {StockMarket.Market_Value_Now}!";
            }

            foreach (var item in statItems)
            {
                dgv_stats_market.Rows.Add(item.Key, item.Value);
            }

            foreach (var message in Messages)
            {
                dgv_output.Rows.Add(message.Key, message.Value);
            }
        }

        #endregion

        #region DGV

        public void AddTransactionToDgv1(string from, string to, double amount, string status)
        {
            if (amount <= 0)
            {
                if (amount == 0)
                {
                    //amount = StockMarket.Market_Value_Now * 1 / 100;
                }
                else if (amount <= 0)
                {
                    //amount = Math.Abs(amount);
                }
            }

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

            dgv_real_transactions.Rows[rowIndex].Height += 10;

            if (from.ToLower().Equals("matrix"))
            {
                if (to.ToLower() == "matrix")
                {
                    // AddTransactionToDgv2(amount, "added++");
                    // dgv_real_transactions.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    AddTransactionToDgv2(amount, "added++");
                    // dgv_real_transactions.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Green;
                }
            }
            else if (from.ToLower().Equals("you"))
            {
                if (to.ToLower() == "you")
                {
                    // AddTransactionToDgv2(amount, "added++");
                    // dgv_real_transactions.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    AddTransactionToDgv2(amount, "deducted--");
                    // dgv_real_transactions.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                if (to.ToLower() == to)
                {
                    // AddTransactionToDgv2(amount, "added++");
                    // dgv_real_transactions.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Green;
                }
                else if (to.ToLower() == "matrix")
                {
                    AddTransactionToDgv2(amount, "deducted--");
                    // dgv_real_transactions.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    AddTransactionToDgv2(amount, "deducted--");
                    // dgv_real_transactions.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Red;
                }
            }
            // Scroll to the last entry (the newly added row)
            dgv_real_transactions.FirstDisplayedScrollingRowIndex = rowIndex;

            // Optionally, select the newly added row (if you want the row to appear highlighted)
            dgv_real_transactions.ClearSelection();
            dgv_real_transactions.Rows[rowIndex].Selected = true;

            // If the status is "Completed", set the text color to green
            // if (status == "Completed")
            // {
            //     dgv_real_transactions.Rows[rowIndex].Cells["Status"].Style.ForeColor = System.Drawing.Color.Green;
            // }
        }

        public void AddTransactionToDgv2(double amount, string action)
        {
            if (amount <= 0)
            {
                if (amount == 0)
                {
                    //amount = StockMarket.Market_Value_Now * 1/100;
                }
                else if (amount <= 0)
                {
                    //amount = amount;
                }
            }

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
                StockMarket.Market_Value_Now = StockMarket.Market_Value_Now + amount;
            }
            else
            {
                StockMarket.Market_Value_Now = StockMarket.Market_Value_Now - amount;
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

            UpdateMainMarketChart(StockMarket.Market_Value_Now, action);

            UpdateDgvStats();
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                Task.Run(() => RunBackgroundTask(_cancellationTokenSource.Token));
            }
        }
        // Background task logic
        private async Task RunBackgroundTask(CancellationToken token)
        {
            try
            {
                // Generate the timeline
                List<double> timeLine = StockPriceAlgorithm.THE_REAL_ALGO2(StockMarket.Market_Value_Now, 50);

                // Process the timeline
                for (int i = 1; i < timeLine.Count; i++)
                {
                    double currentPrice = timeLine[i];
                    double previousPrice = timeLine[i - 1];

                    // Handle negative prices
                    if (currentPrice < 0 || previousPrice < 0)
                    {
                        currentPrice = previousPrice;
                    }

                    // Update market value
                    StockMarket.Market_Value_Now = currentPrice;

                    // Use Invoke to update UI from a background thread
                    this.Invoke(new Action(() =>
                    {
                        if (currentPrice > previousPrice)
                        {
                            AddTransactionToDgv1("Matrix", "Some One Else", currentPrice, "Completed");
                            // Uncomment if you need to update the chart
                            // UpdateMainMarketChart(StockMarket.Market_Value_Now, "added++");
                        }
                        else
                        {
                            AddTransactionToDgv1("Some One Else", "Matrix", currentPrice, "Completed");
                            // Uncomment if you need to update the chart
                            // UpdateMainMarketChart(StockMarket.Market_Value_Now, "subtracted--");
                        }
                    }));

                    // Optional: Add a delay between updates to reduce CPU usage
                    await Task.Delay(200, token); // Use 200 ms delay for smoother UI

                    // Save timeline to JSON file
                    //////////////////string fileName = $"./timelines/{Guid.NewGuid()}_timeline.json";
                    //////////////////File.WriteAllText(fileName, JsonConvert.SerializeObject(timeLine));
                }

                // Refresh the UI after processing
                this.Invoke(new Action(() => this.Refresh()));
            }
            catch (TaskCanceledException)
            {
                // Handle task cancellation gracefully
            }
            catch (Exception ex)
            {
                // Log or show the exception if something goes wrong
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Method to stop the background task
        private void StopBackgroundTask()
        {
            _cancellationTokenSource.Cancel();
        }

        // Make sure to stop the background task when the form is closing or stopping
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopBackgroundTask();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void trk_br_amount_spot_Scroll(object sender, EventArgs e)
        {
           // lbl_per_val.Text = (int)(trk_br_amount_spot.Value * )
        }
    }
}
#endregion