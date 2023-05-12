using System.Text;

static class OptionsMenu
{ 
    public static void Start()
    {
        while (true)
        {
            Console.Clear();

            // list of options that will be displayed
            List<string> StartList = new List<string>()
            {
                "Login",
                "Register",
                "Guest",
                "Info",
            };

            // the necessary info gets used in the display method
            int option = OptionsMenu.DisplaySystem(StartList, "START", "Use ⬆ and ⬇ to navigate and press Enter to select:", true, true, "Exit");
            
            // depending on the option that was chosen, it will clear the console and call the right function
            if (option == 1)
            {
                AccountsLogic accLogic = new AccountsLogic();
                accLogic.Login();
            }
            else if (option == 2)
            {
                AccountsLogic.Register();
            }
            else if (option == 3)
            {
                AccountsLogic.Guest();
            }
            else if (option == 4)
            {
                InfoPage();
            }
            else if (option == 5)
            {
                break;
            }
        }
    }

    static public void Logo(string title = "")
    {
        // prints logo in red
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($@"      ,----.      _ __     .=-.-.  _,.----.            _,.----.    .=-.-. .-._            ,----.         ___     ,---.  ™");
        Console.WriteLine($@"   ,-.--` , \  .-`.' ,`.  /==/_ /.' .' -   \         .' .' -   \  /==/_ //==/ \  .-._  ,-.--` , \ .-._ .'=.'\  .--.'  \");
        Console.WriteLine($@"  |==|-  _.-` /==/, -   \|==|, |/==/  ,  ,-'        /==/  ,  ,-' |==|, | |==|, \/ /, /|==|-  _.-`/==/ \|==|  | \==\-/\ \");
        Console.WriteLine($@"  |==|   `.-.|==| _ .=. ||==|  ||==|-   |  .        |==|-   |  . |==|  | |==|-  \|  | |==|   `.-.|==|,|  / - | /==/-|_\ |");
        Console.WriteLine($@" /==/_ ,    /|==| , '=',||==|- ||==|_   `-' \       |==|_   `-' \|==|- | |==| ,  | -|/==/_ ,    /|==|  \/  , | \==\,   - \");
        Console.WriteLine($@" |==|    .-' |==|-  '..' |==| ,||==|   _  , |       |==|   _  , ||==| ,| |==| -   _ ||==|    .-' |==|- ,   _ | /==/ -   ,|");
        Console.WriteLine($@" |==|_  ,`-._|==|,  |    |==|- |\==\.       /       \==\.       /|==|- | |==|  /\ , ||==|_  ,`-._|==| _ /\   |/==/-  /\ - \");
        Console.WriteLine($@" /==/ ,     //==/ - |    /==/. / `-.`.___.-'         `-.`.___.-' /==/. / /==/, | |- |/==/ ,     //==/  / / , /\==\ _.\=\.-'");
        Console.WriteLine($@" `--`-----`` `--`---'    `--`-`                                  `--`-`  `--`./  `--``--`-----`` `--`./  `--`  `--`");
        Console.ResetColor();

        // prints a title if one is given
        if (title != "")
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n{title.ToUpper()}\n");
            Console.ResetColor();
        }
    }

    static public void InfoPage()
    {
        Console.Clear();
        Console.CursorVisible = false;

        // prints logo & title
        Logo("CINEMA INFO");

        // location info
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("Location");
        Console.ResetColor();
        Console.WriteLine("Wijnhaven 107, 3011WN, Rotterdam");

        // contact info
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\nContact");
        Console.ResetColor();
        Console.WriteLine("06 56745873, info@epic.cinema.nl");

        // opening hours
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\nOpening Hours");
        Console.ResetColor();

        Console.WriteLine("Monday: Closed");
        Console.WriteLine("Tuesday: 3:00 to 3:00.0001");
        Console.WriteLine("Wednesday: Closed");
        Console.WriteLine("Thursday: Closed");
        Console.WriteLine("Friday: Closed");
        Console.WriteLine("Saturday: Closed");
        Console.WriteLine("Sunday: Closed");

        // prints a fake return option hehe
        Console.WriteLine("\n > \u001b[32mReturn\u001b[0m");
        
        // actually returns you to the main menu
        Console.ReadLine();

        Console.CursorVisible = true;
    }

    static public int DisplaySystem(List<string> list, string title, string question = "", bool showlogo = true, bool showreturn = true, string returntext = "Return")
    {
        // makes the cursor invisible
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;

        // prints the banner and the title
        if (showlogo)
        {
            OptionsMenu.Logo(title);
        }

        // adds extra info if provided
        if (question != "")
        {
            Console.WriteLine($"{question}\n");
        }

        // gets the cursor position and sets option to 1
        (int left, int top) = Console.GetCursorPosition();
        int option = 1;
        int returncount = 0;

        // this is the decorator that will help you see where the cursor is at
        var decorator = " > \u001b[32m";

        // sets a variable for 'key' that will be used later
        ConsoleKeyInfo key;

        // the loop in which an option is chosen from a list
        bool isSelected = false;
        while (!isSelected)
        {
            // sets the cursor to the previously determined location
            Console.SetCursorPosition(left, top);

            // prints the options one by one
            for (int i = 0; i < list.Count(); i++)
            {
                //writes the option and gives it a number
                Console.WriteLine($"{(option == i + 1 ? decorator : "   ")}{list[i]}\u001b[0m");
            }

            // this will show the return button
            if (showreturn)
            {
                Console.WriteLine($"\n{(option == list.Count() + 1 ? decorator : "   ")}{returntext}\u001b[0m");
                returncount = 1;
            }

            // sees what key has been pressed
            key = Console.ReadKey(false);

            // a switch case that changes the value from 'option', depending on the key input
            switch (key.Key)
            {
                // moves one up
                case ConsoleKey.UpArrow:
                    option = option == 1 ? list.Count() + returncount : option - 1;
                    break;
                    
                // moves one down
                case ConsoleKey.DownArrow:
                    option = option == list.Count() + returncount ? 1 : option + 1;
                    break;

                // if enter is pressed, breaks out of the while loop
                case ConsoleKey.Enter:
                    isSelected = true;
                    break;
            }
        }

        Console.CursorVisible = true;
        return option;
    }

    static public int MovieDisplaySystem(List<MovieModel> list, string title, string question = "", bool showlogo = true, bool showreturn = true)
    {
        // makes the cursor invisible
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;

        // prints the banner and the title
        if (showlogo)
        {
            OptionsMenu.Logo(title);
        }

        // adds extra info if provided
        if (question != "")
        {
            Console.WriteLine($"{question}\n");
        }

        // gets the cursor position and sets option to 1
        (int left, int top) = Console.GetCursorPosition();
        int option = 1;
        int returncount = 0;

        // this is the decorator that will help you see where the cursor is at
        var decorator = " > \u001b[32m";

        // sets a variable for 'key' that will be used later
        ConsoleKeyInfo key;

        // the loop in which an option is chosen from a list
        bool isSelected = false;
        while (!isSelected)
        {
            // sets the cursor to the right position
            Console.SetCursorPosition(left, top);

            // prints the movies one by one
            for (int i = 0; i < list.Count(); i++)
            {
                // writes movie title in red
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{(option == i + 1 ? decorator : "   ")}{list[i].Title}\u001b[0m");
                Console.ResetColor();

                // prints the description
                Console.WriteLine($"    Description:\n    {MovieLogic.SpliceText(list[i].Description, "    ")}\n");
            }

            // this will show the return button
            if (showreturn)
            {
                Console.WriteLine($"\n{(option == list.Count() + 1 ? decorator : "   ")}Return\u001b[0m");
                returncount = 1;
            }

            // sees what key has been pressed
            key = Console.ReadKey(false);

            // a switch case that changes the value from 'option', depending on the key input
            switch (key.Key)
            {
                // moves one up
                case ConsoleKey.UpArrow:
                    option = option == 1 ? list.Count() + returncount : option - 1;
                    break;
                    
                // moves one down
                case ConsoleKey.DownArrow:
                    option = option == list.Count() + returncount ? 1 : option + 1;
                    break;

                // if enter is pressed, breaks out of the while loop
                case ConsoleKey.Enter:
                    isSelected = true;
                    break;
            }
        }

        Console.CursorVisible = true;
        return option;
    }

    static public int CateringDisplaySystem(List<CateringModel> list, string title, string question = "", bool showlogo = true, bool showreturn = true)
    {
        // makes the cursor invisible
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;

        // prints the banner and the title
        if (showlogo)
        {
            OptionsMenu.Logo(title);
        }

        // adds extra info if provided
        if (question != "")
        {
            Console.WriteLine($"{question}\n");
        }

        // gets the cursor position and sets option to 1
        (int left, int top) = Console.GetCursorPosition();
        int option = 1;
        int returncount = 0;

        // this is the decorator that will help you see where the cursor is at
        var decorator = " > \u001b[32m";

        // sets a variable for 'key' that will be used later
        ConsoleKeyInfo key;

        // the loop in which an option is chosen from a list
        bool isSelected = false;
        while (!isSelected)
        {
            // sets the cursor to the right position
            Console.SetCursorPosition(left, top);

            // prints the movies one by one
            for (int i = 0; i < list.Count(); i++)
            {
                // writes movie title in red
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{(option == i + 1 ? decorator : "   ")}{list[i].Name}\u001b[0m");
                Console.ResetColor();

                // prints the description & price
                Console.WriteLine($"    Description:\n    {MovieLogic.SpliceText(list[i].Description, "    ")}");
                Console.WriteLine($"    Price:\n    ${(list[i].Price)}\n");
            }

            // this will show the return button
            if (showreturn)
            {
                Console.WriteLine($"\n{(option == list.Count() + 1 ? decorator : "   ")}Return\u001b[0m");
                returncount = 1;
            }

            // sees what key has been pressed
            key = Console.ReadKey(false);

            // a switch case that changes the value from 'option', depending on the key input
            switch (key.Key)
            {
                // moves one up
                case ConsoleKey.UpArrow:
                    option = option == 1 ? list.Count() + returncount : option - 1;
                    break;
                    
                // moves one down
                case ConsoleKey.DownArrow:
                    option = option == list.Count() + returncount ? 1 : option + 1;
                    break;

                // if enter is pressed, breaks out of the while loop
                case ConsoleKey.Enter:
                    isSelected = true;
                    break;
            }
        }

        Console.CursorVisible = true;
        return option;
    }
}