﻿using System.Security.Cryptography;
using System.Text;

public class AccountsLogic
{
    private List<AccountModel> _accounts = new();
    public static AccountModel? CurrentAccount = null;
    private static AccountsLogic accountsLogic = new AccountsLogic();

    public AccountsLogic()
    {
        LoadAccounts();
    }
    public void LoadAccounts()
    {
        _accounts = AccountsAccess.LoadAll();
    }


    public void SetCurrentAccount(AccountModel account) 
    {
        CurrentAccount = account;
    }

    public void UpdateList(AccountModel acc)
    {
        //Find if there is already an model with the same id
        int index = _accounts.FindIndex(s => s.Id == acc.Id);

        if (index != -1)
        {
            //update existing model
            _accounts[index] = acc;
        }
        else
        {
            //add new model
            _accounts.Add(acc);
        }
        AccountsAccess.WriteAll(_accounts);

    }

    public static void RemoveAcc(int id)
    {
        List<AccountModel> accounts = AccountsAccess.LoadAll();
        // finds if there is a movie with the same id
        int index = accounts.FindIndex(s => s.Id == id);

        // removes the movie with that id, and updates the json file
        accounts.Remove(accounts[index]);
        AccountsAccess.WriteAll(accounts);
    }
    public void RemoveAccAdmin(int id)
    {
        // Get the ID of the currently logged-in account
        int loggedInAccountId = CurrentAccount?.Id ?? 0;

        if (id == loggedInAccountId)
        {
            Console.Clear();
            Console.WriteLine("You cannot remove your own account.");
            Console.ReadLine();
            return;
        }

        // Find the index of the account with the specified ID
        int index = _accounts.FindIndex(s => s.Id == id);

        if (index != -1)
        {
            // Remove the account with the specified ID
            _accounts.RemoveAt(index);
            AccountsAccess.WriteAll(_accounts);
       
            OptionsMenu.FakeContinue("Account removed successfully.", "Account removed");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Account not found.");
            Console.ReadLine();
        }
    }

    public AccountModel? GetById(int id)
    {
       return _accounts.Find(i => i.Id == id);
    }

    // Return false if either of the parameters are empty or null
    public static bool IsLoginValid(string email, string password)
    {
        return (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password));
    }

    public void Login()
    {
        Console.CursorVisible = true;

        string email = "";
        string password = "";

        while (true)
        {
            OptionsMenu.Logo("login");

            Console.WriteLine("Email address: ");
            email = Console.ReadLine() + "";
            Console.WriteLine("\nPassword: ");
            password = accountsLogic.GetMaskedPassword();

            //Return authorized accountmodel that matches both credentials. If no matching account is found, return an empty unauthorized account
            AccountModel currentAccount;
            AccountModel? accountModel = _accounts.Find(a => a.EmailAddress == email && a.Password == HashPassword(password));

            if(IsLoginValid(email, password) && accountModel != null) 
            { 
                accountModel.Authorize();
                currentAccount = accountModel;

                if(currentAccount.Authorized == true && currentAccount.Type == AccountModel.AccountType.ADMIN)
                {
                    accountsLogic.SetCurrentAccount(currentAccount);
                }
                else if (currentAccount.Authorized == true && currentAccount.Type == AccountModel.AccountType.EMPLOYEE)
                {
                    accountsLogic.SetCurrentAccount(currentAccount);
                }
                else if (currentAccount.Authorized == true && currentAccount.Type == AccountModel.AccountType.CUSTOMER)
                {
                    accountsLogic.SetCurrentAccount(currentAccount);
                }
                
                List<string> DList = new List<string>(){"Continue"};

                OptionsMenu.DisplaySystem(DList, "welcome page", $"Welcome back, {accountModel.FullName}.\nYour email address is: {accountModel.EmailAddress}", true, false);

                // In case a reservation was previously started but unfinished, the previous data will be removed so that a fresh new reservation can be made
                OptionsMenu.RemoveUnfinishedReservation();
                
                break;
            }
            else
            {
                int option = OptionsMenu.DisplaySystem(MovieLogic.ContinueList, "", "\nNo account found with these credentials.", false, true);
                
                currentAccount = new AccountModel(0, email, password, string.Empty);

                if (option == 2)
                {
                    break;
                }
            }
        }
        Console.CursorVisible = false;
    }

    public static void Register(bool isEmployeeRegistration = false, bool isAdminRegistration = false)
    {
        Console.CursorVisible = true;

        string email = string.Empty;

        while(true) 
        {
            OptionsMenu.Logo(isEmployeeRegistration ? "Employee account registration" : isAdminRegistration ? "Admin account registration" : "Registration");

            Console.WriteLine("Email Address:");
            email = Console.ReadLine() + "";

            if (!accountsLogic.IsEmailValid(email))
            {

                int option = OptionsMenu.DisplaySystem(MovieLogic.ContinueList, "", "\nInvalid email, please try again.", false, true);

                if (option == 2)
                {
                    return;
                }
            }
            else if(accountsLogic.IsEmailInUse(email))
            {
                int option = OptionsMenu.DisplaySystem(MovieLogic.ContinueList, "", "\nThis email is already in use, please try again.", false, true);

                if (option == 2)
                {
                    return;
                }
            }
            else
            {
                break;
            }
        }

        string password = string.Empty;
        string confirmedPassword = "no match";

        while (password != confirmedPassword)
        {

            OptionsMenu.Logo("registration");
            Console.WriteLine("Password:");

            password = accountsLogic.GetMaskedPassword();
            if (accountsLogic.IsPasswordValid(password))
            {
                OptionsMenu.Logo("registration");

                Console.WriteLine("Confirm Password:");
                confirmedPassword = accountsLogic.GetMaskedPassword();

                if (password != confirmedPassword)
                {

                    int option = OptionsMenu.DisplaySystem(MovieLogic.ContinueList, "", "\nPasswords do not match, please try again.", false, true);

                    if (option == 2)
                    {
                        return;
                    }
                }
            }
            else
            {
                int option = OptionsMenu.DisplaySystem(MovieLogic.ContinueList, "", "\nPassword must be between 8 and 32 characters long and contain atleast one number, one capital letter and one special character", false, true);
            
                if (option == 2)
                {
                    return;
                }
            }
        }

        OptionsMenu.Logo("registration");
        Console.WriteLine("Name:");
        string fullName = Console.ReadLine() + "";

        AccountModel acc = RegisterLogic(isEmployeeRegistration, isAdminRegistration, email, password, fullName);

        if (!isEmployeeRegistration && !isAdminRegistration) accountsLogic.SetCurrentAccount(acc);

        List<string> DList = new List<string>(){"Continue"};

        OptionsMenu.DisplaySystem(DList, "welcome page", isEmployeeRegistration || isAdminRegistration ? $"Succesfully created {acc.Type} account: {acc.FullName}!":$"Account created successfully!\nWelcome, {fullName}.", true, false);
        if (!isAdminRegistration && !isEmployeeRegistration)
        {
            MovieMenu.Start();     
        }
        
        Console.CursorVisible = false;
    }

    public static AccountModel RegisterLogic(bool isEmployeeRegistration, bool isAdminRegistration, string email, string password, string fullName)
    {
        
        AccountModel.AccountType accountType = isEmployeeRegistration ? AccountModel.AccountType.EMPLOYEE : isAdminRegistration ? AccountModel.AccountType.ADMIN : AccountModel.AccountType.CUSTOMER;
        AccountModel acc = new AccountModel(accountsLogic.GetNextId(), email, AccountsLogic.HashPassword(password), fullName, accountType);
        accountsLogic.UpdateList(acc);
        
        return acc;
    }

    // Returns true if the email is not empty, contains an '@', contains a '.' and does not contain any white space.
    public bool IsEmailValid(string email) => (!string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Contains(".") && !email.Contains(" "));

    // Return true if an account with this email is found in the JSON.
    public bool IsEmailInUse(string email) => _accounts.Any(i => string.Equals(i.EmailAddress, email, StringComparison.OrdinalIgnoreCase));

    // Return true if the given password matches the criteria
    public bool IsPasswordValid(string password)
    {
        return ((password.Length >= 8) && (password.Length <= 32)
                && password.Any(char.IsDigit)
                && password.Any(char.IsUpper)
                && password.Any(c => !char.IsLetterOrDigit(c)));
    }

    // Returns the nextID
    public int GetNextId() 
    {
        int maxId = 0;

        foreach (var acc in _accounts)
            if (acc.Id > maxId)
                maxId = acc.Id;

        return maxId + 1;
    }

    // Masks passwords in the terminal
    public string GetMaskedPassword()
    {
        string password = "";

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            // This code checks on the user input including space, and special symbols and converts them to asterisks in the terminal
            if (keyInfo.Key == ConsoleKey.Enter)
            {            
                break;
            }
            // This code block removes the asterisks if backspace is used in the code
            else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
            // This code checks the ascii table for special characters and converts them to asterisks
            else if (keyInfo.KeyChar >= 32 && keyInfo.KeyChar <= 126)
            {
                password += keyInfo.KeyChar;
                Console.Write("*");
            }
        }

        Console.WriteLine();
        return password;
    }

    public static string HashPassword(string raw) 
    {
        // Using statement to ensure proper disposal of SHA256 instance.
        // Create SHA256 instance to generate hash
        using (SHA256 hash = SHA256.Create()) 
        {
            // Combine the hash bytes into a single string, compute the hash of the input ('raw')
            // And convert each byte to a string representation of its hex value.
            return String.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(raw)).Select(i => i.ToString("x2")));
        }
    }   

    public static void Guest()
    {
        AccountsLogic accountsLogic = new AccountsLogic();
        AccountModel guestAccount = new AccountModel(accountsLogic.GetNextId(), "", "", "", AccountModel.AccountType.GUEST);
        CurrentAccount = guestAccount;

        List<AccountModel> accounts = AccountsAccess.LoadAll();
        accounts.Add(guestAccount);
        AccountsAccess.WriteAll(accounts);

        MovieMenu.Start();
    }
}