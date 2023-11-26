using ATMapp.Domain.Entities;
using ATMapp.Domain.Enums;
using ATMapp.Domain.Interfaces;
using ATMapp.UI;
using ConsoleTables;

namespace ATMapp.App
{
    public class ATMapp : IUserLogIn, IUserAccount, ITransaction
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> listOfTransactions;
        private const decimal minimumKeptAmount = 500;
        private readonly AppScreen screen;

        public ATMapp()
        {
            screen = new AppScreen();
        }

        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            while (true)
            {
                AppScreen.DisplayAppMenu();
                ProcessMenuOption();
            }
        }

        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount{Id = 1,FullName = "Soner Abduramanov", AccountNumber = 987654, CardNumber = 123123,CardPin = 999111, AccountBalance = 40000.00m,IsLocked = false},
                new UserAccount{Id = 2,FullName = "Mence Angelova", AccountNumber = 123456, CardNumber = 321321,CardPin = 123123, AccountBalance = 50000.00m,IsLocked = false},
            };

            listOfTransactions = new List<Transaction>();
        }

        public void CheckUserCardNumAndPassword()
        {
            bool isCorrectLogin = false;

            while (isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();

                foreach (UserAccount account in userAccountList)
                {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogIn++;

                        if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = account;
                            if (selectedAccount.IsLocked || selectedAccount.TotalLogIn > 3)
                            {
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogIn = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }

                        if (isCorrectLogin == false)
                        {
                            Utility.PrintMessage("\nInvalid card number or PIN.", false);
                            selectedAccount.IsLocked = selectedAccount.TotalLogIn == 3;
                            if (selectedAccount.IsLocked)
                            {
                                AppScreen.PrintLockScreen();
                            }
                        }
                        Console.Clear();
                    }
                }
            }
        }


        private void ProcessMenuOption()
        {
            Console.WriteLine();
            switch (Validator.Convert<int>("an option:"))
            {
                case (int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)AppMenu.MakeWithdrawal:
                    MakeWithDrawal();
                    break;
                case (int)AppMenu.InternalTransfer:
                    var internalTransfer = screen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;
                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogOutProgress();
                    Utility.PrintMessage("You have successfully logged out.Please collect your ATM card.");
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option. ", false);
                    break;
            }
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
            Console.WriteLine($"\nOnly multiples of 500 and 1000 dollars allowed.\n");
            var transactionAmount = Validator.Convert<int>($"amount {AppScreen.currency}");

            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            if (transactionAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be greather than zero. Try again.", false);
                return;
            }
            else if (transactionAmount % 500 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiples of 500 of 1000. Try again.", false);
                return;
            }
            else if (PreviewBankNotesCount(transactionAmount) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
            }

            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transactionAmount, "");

            //update account balance
            selectedAccount.AccountBalance += transactionAmount;

            //print success message
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transactionAmount)} was successful.");
        }


        public void MakeWithDrawal()
        {
            var transationAmount = 0;
            int _selectedAmount = AppScreen.SelectAmount();

            if (_selectedAmount == -1)
            {
                //Calling again!
                MakeWithDrawal();
                return;
            }
            else if (_selectedAmount != 0)
            {
                transationAmount = _selectedAmount;
            }
            else
            {
                //0.Other
                transationAmount = Validator.Convert<int>($"amount {AppScreen.currency}");
            }


            if (transationAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be greather than zero! Try again.", false);
            }
            if (transationAmount % 500 != 0)
            {
                Utility.PrintMessage("You can only withdraw amount in multiplies of 500 or 1000 dollars. Try again.", false);
                return;
            }


            //Business logic validations
            if (transationAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawal failed. Your balance is too low to withdraw {Utility.FormatAmount(transationAmount)}", false);
                return;
            }

            if ((selectedAccount.AccountBalance - transationAmount) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }


            //bind withdrawal details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, transationAmount, "");

            //update account balance
            selectedAccount.AccountBalance -= transationAmount;

            //success message.
            Utility.PrintMessage($"You have successfully withdrawn {Utility.FormatAmount(transationAmount)} ", true);
        }

        private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fiveHundredNotesCount = (amount % 1000) / 500;

            Console.WriteLine("\nSummary");
            Console.WriteLine("------");
            Console.WriteLine($"{AppScreen.currency} 1000 X {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.currency} 500 X {fiveHundredNotesCount} = {500 * fiveHundredNotesCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);
        }

        public void InsertTransaction(long _userBankAccountId, TransactionType _transactionType, decimal _transactionAmount, string _desc)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _userBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionAmount = _transactionAmount,
                TransactionType = _transactionType,
                Description = _desc
            };

            //add transaction object to the list
            listOfTransactions.Add(transaction);
        }

        public void ViewTransaction()
        {
            var filteredTransactionList = listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();

            //check if there's a transaction
            if (filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have not transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descriptions", "Amount" + AppScreen.currency);
                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
            }

        }


        //internal transfer.
        private void ProcessInternalTransfer(InternalTransfer internalTransfer)
        {
            //check
            if (internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero.Try again.", false);
                return;
            }

            //check sender's account balance
            if (internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed.You do not have enough balance, to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}", false);
                return;
            }


            //check the minimum kept amount
            if ((selectedAccount.AccountBalance - minimumKeptAmount) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Transfer failed.Your account needs to have minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;

            }

            //check receiver's account is valid
            var selectedBankAccountReceiver = (from userAcc in userAccountList
                                               where userAcc.AccountNumber == internalTransfer.RecipientBankAccountNumber
                                               select userAcc).FirstOrDefault();

            if (selectedBankAccountReceiver == null)
            {
                Utility.PrintMessage("Transfer failed. Reciever bank account number is invalid.", false);
                return;
            }

            //check receiver's name
            if (selectedBankAccountReceiver.FullName != internalTransfer.RecipientBankAccountName)
            {
                Utility.PrintMessage("Transfer failed. Recipient's bank account name is not match.", false);
                return;
            }

            //add transaction to transactions record - sender
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, -internalTransfer.TransferAmount, "Transfered to " +
                $"{selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");

            //update sender's account balance
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            //add transaction record-recivier
            InsertTransaction(selectedBankAccountReceiver.Id, TransactionType.Transfer, internalTransfer.TransferAmount, "Transfered from" +
                $"{selectedAccount.AccountNumber}({selectedAccount.FullName})");

            //update receiver's account balance
            selectedBankAccountReceiver.AccountBalance += internalTransfer.TransferAmount;

            //print success message.
            Utility.PrintMessage($"You have successfully transfered {Utility.FormatAmount(internalTransfer.TransferAmount)} to " +
                $"{internalTransfer.RecipientBankAccountName}", true);


        }
    }
}