using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

class EmployeeMenu
{
    public static MovieLogic movie = new MovieLogic();
    public static CateringLogic food = new CateringLogic();
    public static AccountsLogic account = new AccountsLogic();

    protected static List<string> StartList = new List<string>()
    {
        "Movies and seat availability",
        "Catering",
        "Global seat data and heat map"
    };
    public static void StartEmployee()
    {
        while (true)
        {
            Console.Clear();

            // the necessary info gets used in the display method
            int option = OptionsMenu.DisplaySystem(StartList, "Editing menu", "Select what category you want to edit.", true, true);

            // depending on the option that was chosen, it will clear the console and call the right function
            if (option == 1)
            {
                Console.Clear();
                movie.EmployeeMovies();
            } 
            
            else if (option == 2)
            {
                food.EmployeeCatering();
            }

            else if (option == 3)
            {
                EditGlobalSeatData();
            }

            if (option == 4)
            {
                break;
            }
        }
    }

    public static void EditGlobalSeatData()
    {
        while (true)
        {
            List<string> ew = new() {"Global Seat Data", "Heat Map"};

            int weh = OptionsMenu.DisplaySystem(ew, "edit auditorium", "What would you like tho change?");
            
            if (weh == 1)
            {
                while (true)
                {
                    var SeatData = SeatAccess.LoadGlobalSeatData();

                    List<string> optionList = new List<string>()
                    {
                        $"Range 1: {SeatData[1].Item1} (+ {SeatData[1].Item2})",
                        $"Range 2: {SeatData[2].Item1} (+ {SeatData[2].Item2})",
                        $"Range 3: {SeatData[3].Item1} (+ {SeatData[3].Item2})"
                    };

                    int option = OptionsMenu.DisplaySystem(optionList, "edit seat info", "Select what category you want to edit.", true, true);

                    int key = option;

                    if (key == 4)
                    {
                        return;
                    }
                    else
                    {
                        List<string> change = new() {$"Name: {SeatData[key].Item1}", $"Price: {SeatData[key].Item2}"};
                        while (true)
                        {
                            int option2 = OptionsMenu.DisplaySystem(change, "edit seat info", "Select what field you want to edit.", true, true);
                            if (option2 == 1)
                            {
                                while (true)
                                {
                                    OptionsMenu.Logo("edit seat info");
                                    Console.Write("New name: ");
                                    string newName = Console.ReadLine();
                                    if (!string.IsNullOrEmpty(newName))
                                    {
                                        SeatData[key] = (newName, SeatData[key].Item2);
                                        break;
                                    }

                                    Console.WriteLine("\nThe name can't be empty.");
                                    
                                    // prints a fake return option hehe
                                    Console.WriteLine("\n > \u001b[32mContinue\u001b[0m");
                                
                                    // actually returns you to the main menu
                                    Console.ReadLine();
                                }
                            }
                            else if (option2 == 2)
                            {
                                while (true)
                                {
                                    double Price;
                                    OptionsMenu.Logo("edit seat info");
                                    Console.Write("New price: ");
                                    string newPrice = Console.ReadLine();

                                    if (double.TryParse(newPrice.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Price))
                                    {
                                        SeatData[key] = (SeatData[key].Item1, Price);

                                        break;
                                    }

                                    Console.WriteLine("Invalid price. Please enter a valid decimal number.");
                                    
                                    // prints a fake return option hehe
                                    Console.WriteLine("\n > \u001b[32mContinue\u001b[0m");
                                
                                    // actually returns you to the main menu
                                    Console.ReadLine();
                                }
                            }
                            else
                            {
                                break;
                            }

                            SeatAccess.WriteGlobalSeatData(SeatData);
                    
                            OptionsMenu.Logo("Seat data updated");

                            Console.WriteLine("Seat Data updated successfully.");
                            
                            // prints a fake return option hehe
                            Console.WriteLine("\n > \u001b[32mContinue\u001b[0m");
                        
                            // actually returns you to the main menu
                            Console.ReadLine();
                            break;
                        }
                    }
                }
            }
            else if (weh == 2)
            {
                List<string> range = new() {"Range 1", "Range 2", "Range 3"};

                while (true)
                {
                    // Regex pattern for checking the validity of the input ID later in the selection process
                    string pattern = @"^[a-l]([1-9]|1[0-4])$";

                    // List and string for keeping track of the selections
                    List<string> selectedChairs = new();
                    string currentlySelectedChair = string.Empty;

                    // A bool and a variable used to reuse the same loop for removing seats from your selection
                    bool removingMode = false;
                    string four = "4";

                    string pathToCsv = $@"DataSources/DefaultAuditorium/Auditorium.csv";

                    string[][] auditoriumArray = SeatAccess.LoadAuditorium(pathToCsv); // Initialise array for searching and updating values

                    // Looping the selection of the seats until the user has selected all seats they'd want
                    while (true)
                    {
                        // Visualisation of the menu
                        OptionsMenu.Logo("edit seats");
                        SeatAccess.PrintAuditorium(auditoriumArray);
                        SeatsMenu.SeatLegendDefault();

                        // Ask user for id of the seat
                        Console.WriteLine($"Selected seats: [{String.Join(", ", selectedChairs)}]");
                        Console.WriteLine($"Type in the ID of the seat you want to {(removingMode ? "remove from your selection" : "select")} (I.E. - A6)");

                        currentlySelectedChair = Console.ReadLine();

                        // If removing mode is on the 4 check in the csv will be negated so you can remove your own selections
                        // Otherwise if removing mode is off, the check will be on and you cannot select the seats you selected again
                        four = removingMode ? "" : "4";

                        // Checking the validity of the input ID and preventing any crashes
                        if (!string.IsNullOrEmpty(currentlySelectedChair))
                        {
                            if (Regex.IsMatch(currentlySelectedChair, pattern, RegexOptions.IgnoreCase))
                            {
                                if (SeatAccess.FindSeatValueInArray(auditoriumArray, currentlySelectedChair.ToUpper()) != four &&
                                    SeatAccess.FindSeatValueInArray(auditoriumArray, currentlySelectedChair.ToUpper()) != "")
                                {

                                    // Check if the user selected the option to remove a movie and change the logic based on that
                                    if (!removingMode)
                                    {
                                        selectedChairs.Add(currentlySelectedChair.ToUpper());

                                        // Update array to show which chairs are currently selected in the selection process
                                        foreach (string seat in selectedChairs)
                                        {
                                            SeatAccess.UpdateSeatValue(auditoriumArray, seat, "4");
                                        }
                                    }

                                    else
                                    {
                                        SeatAccess.UpdateSeatValue(auditoriumArray, currentlySelectedChair.ToUpper(), SeatAccess.FindDefaultSeatValueArray(currentlySelectedChair.ToUpper()));
                                        selectedChairs.Remove(currentlySelectedChair.ToUpper());
                                    }

                                    OptionsMenu.Logo("edit seats");
                                    SeatAccess.PrintAuditorium(auditoriumArray);
                                    SeatsMenu.SeatLegendDefault();
                        
                                    // Prepare option for use in checking if there are any seats selected
                                    int optionInLoop = 0;

                                    List<string> answerList = new List<string>()
                                    {
                                        "Yes",
                                        "Add more seats",
                                        "Remove seats"
                                    };
                                    // If there are no seats selected option 2 is automatically selected and the user is prompted to select a seat again
                                    if (!selectedChairs.Any()) optionInLoop = 2;
                                    
                                    else optionInLoop = OptionsMenu.DisplaySystem(answerList, "", $"You've Selected seat(s) [{String.Join(", ", selectedChairs)}], are you satisfied with these selections?", false, true);

                                    if (optionInLoop == 1)
                                    {
                                        break;
                                    }
                                    else if (optionInLoop == 2)
                                    {
                                        removingMode = false;
                                    }

                                    else if (optionInLoop == 3)
                                    {
                                        removingMode = true;
                                    }

                                    else if (optionInLoop == 4)
                                    {
                                        return;
                                    }
                                }

                                else
                                {
                                    List<string> EList = new List<string>(){"Continue"};
                                    int option = OptionsMenu.DisplaySystem(EList, "", "\nThat seat does not exist, or you've selected it already.", false, true);

                                    if (option == 1)
                                    {
                                        continue;
                                    }

                                    if (option == 2)
                                    {
                                        return;
                                    }
                                }

                            }

                            else
                            {
                                List<string> EList = new List<string>(){"Continue"};
                                int option = OptionsMenu.DisplaySystem(EList, "", "\nThat is not a seat ID", false, true);

                                if (option == 1)
                                {
                                    continue;
                                }

                                if (option == 2)
                                {
                                    return;
                                }
                            }
                        }

                        else
                        {
                            List<string> EList = new List<string>(){"Continue"};
                            int option = OptionsMenu.DisplaySystem(EList, "", "\nPlease fill in the ID of a seat", false, true);

                            if (option == 1)
                            {
                                continue;
                            }

                            if (option == 2)
                            {
                                return;
                            }
                        }
                    }
                    
                    OptionsMenu.Logo("edit seats");
                    SeatAccess.PrintAuditorium(auditoriumArray);
                    SeatsMenu.SeatLegendDefault();

                    int ran_opt = OptionsMenu.DisplaySystem(range, "edit seats", "What do you want the price range of the seat(s) to be?", false);

                    if (ran_opt != 4)
                    {
                        foreach (string seat in selectedChairs)
                        {
                            SeatAccess.UpdateSeatValue(auditoriumArray, seat, Convert.ToString(ran_opt));
                        }

                        SeatAccess.WriteToCSV(auditoriumArray, pathToCsv);

                        OptionsMenu.Logo("edit seats");
                        SeatAccess.PrintAuditorium(auditoriumArray);
                        SeatsMenu.SeatLegendDefault();

                        Console.WriteLine("The seats have been updated.");

                        // prints a fake return option hehe
                        Console.WriteLine("\n > \u001b[32mContinue\u001b[0m");
                    
                        // actually returns you to the main menu
                        Console.ReadLine();  
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                break;
            }
        }
        
    }

}
