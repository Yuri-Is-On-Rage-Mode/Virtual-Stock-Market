using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kryptu_Vault.classes
{
    public class Trading_Firm
    {
        public static string MY_NAME = "";
        public static double INITIAL_MONEY = 0.00;

        public static double CURRENT_MONEY = 0.00;
        public static double LAST_INCREASE_PERCENT = 0.00;

        public static double MARKET_INVESTMENT_TOTAL = 0.00;
        public static double MARKET_INVESTMENT_TOTAL_PERCENT = 0.00;
        public static void InitFirm(string MY_NAMES, double INITIAL_MONEYS)
        {
            MY_NAME = MY_NAMES;
            INITIAL_MONEY = INITIAL_MONEYS;

            CURRENT_MONEY = INITIAL_MONEY;
            LAST_INCREASE_PERCENT = 100;
        }

        public static void Invest(double AMOUNT_TO_INVEST)
        {
            if (AMOUNT_TO_INVEST <= INITIAL_MONEY)
            {

                CURRENT_MONEY -= AMOUNT_TO_INVEST;

                MARKET_INVESTMENT_TOTAL_PERCENT += AMOUNT_TO_INVEST / StockMarket.Market_Value_Now * 100;
                MARKET_INVESTMENT_TOTAL += AMOUNT_TO_INVEST;
            }
        }

        public static void Withdrawl(double AMOUNT_TO_INVEST)
        {
            if (AMOUNT_TO_INVEST <= INITIAL_MONEY)
            {

                //CURRENT_MONEY -= AMOUNT_TO_INVEST;

                double abcd = AMOUNT_TO_INVEST / MARKET_INVESTMENT_TOTAL * 100;

                MARKET_INVESTMENT_TOTAL_PERCENT -= StockMarket.Market_Value_Now * abcd;
                MARKET_INVESTMENT_TOTAL -= AMOUNT_TO_INVEST;
            }
        }
    }
}
