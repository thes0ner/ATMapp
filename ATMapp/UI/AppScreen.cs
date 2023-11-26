using ATMapp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMapp.UI
{
    public class AppScreen
    {
        internal const string currency = "$ ";
        internal static void Welcome()
        {
            //Clears the console screen.
            Console.Clear();
            //sets the title of the consol window.
            Console.Title = "My ATM App";
            //sets the text or foreground color white.
            Console.ForegroundColor = ConsoleColor.White;


            Console.WriteLine("\n-------------------Welcome to My ATM App-------------------\n");

            // prompt the user to insert atm card.
            Console.WriteLine("Note: Actual ATM machine will accept and validate a physical ATM card,read" +
                " card number and validate it.");
            Console.WriteLine("Please insert your ATM card.");

            Utility.PressEnterToContinue();
        }
        internal static UserAccount UserLoginForm()
        {
            UserAccount tempUserAccount = new UserAccount();
            tempUserAccount.CardNumber = Validator.Convert<long>("your card number.");
            tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your card PIN"));
            return tempUserAccount;
        }
        internal static void LoginProgress()
        {
            Console.WriteLine("\nChecking card number and PIN...");
            Utility.PrintDotAnimation();
        }
        internal static void PrintLockScreen()
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked.Please go to the nearest branch to unlock your account.", true);
            Utility.PressEnterToContinue();
            Environment.Exit(1);
        }

        internal static void WelcomeCustomer(string fullName)
        {
            Console.WriteLine($"Welcome back, {fullName}");
            Utility.PressEnterToContinue();
        }

        internal static void DisplayAppMenu()
        {
            Console.Clear();
            Console.WriteLine("-------------- ATM MENU --------------");
            Console.WriteLine(":\t                                      :");
            Console.WriteLine(":\t1. Account Balance                    :");
            Console.WriteLine(":\t2. Cash Deposit                       :");
            Console.WriteLine(":\t3. Withdrawal                         :");
            Console.WriteLine(":\t4. Transfer                           :");
            Console.WriteLine(":\t5. Transactions                       :");
            Console.WriteLine(":\t6. Logout                             :");

        }

        internal static void LogOutProgress()
        {
            Console.WriteLine("Thank you for using ATM...");
            Utility.PrintDotAnimation();
            Console.Clear();
        }

        internal static int SelectAmount()
        {
            Console.Clear();
            Console.WriteLine("Select amount you want to withdraw: ");
            Console.WriteLine(":1.{0}500 \t\t5.{0}10,000", currency);
            Console.WriteLine(":2.{0}1000\t\t6.{0}15,000", currency);
            Console.WriteLine(":3.{0}2000\t\t7.{0}20,000", currency);
            Console.WriteLine(":4.{0}5000\t\t8.{0}40,000", currency);
            Console.WriteLine(":0.Other");
            Console.WriteLine("");

            int selectedAmount = Validator.Convert<int>("option:");
            switch (selectedAmount)
            {
                case 1:
                    return 500;
                    break;
                case 2:
                    return 1000;
                    break;
                case 3:
                    return 2000;
                    break;
                case 4:
                    return 5000;
                    break;
                case 5:
                    return 10000;
                    break;
                case 6:
                    return 15000;
                    break;
                case 7:
                    return 20000;
                    break;
                case 8:
                    return 40000;
                case 0:
                    return 0;
                    break;
                default:
                    Utility.PrintMessage("Invalid input. Try again.", false);
                    return -1;
                    break;

            }
        }

        internal InternalTransfer InternalTransferForm()
        {
            var internalTransfer = new InternalTransfer();
            internalTransfer.RecipientBankAccountNumber = Validator.Convert<long>("recipient's account number :");
            internalTransfer.TransferAmount = Validator.Convert<decimal>($"amount {currency}:");
            internalTransfer.RecipientBankAccountName = Utility.GetUserInput("recipient's name: ");
            return internalTransfer;
        }

    }
}
