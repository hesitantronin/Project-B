using System.Text;

class MovieLogic
{
    private List<MovieModel> _movies = new();
    static public MovieModel CurrentMovie { get; private set; }

    public MovieLogic()
    {
        // uses the loadall function to load the json to the list
        _movies = MovieAccess.LoadAll();
    }


    public void UpdateList(MovieModel mov)
    {
        // finds if there is already a movie with the same id
        int index = _movies.FindIndex(s => s.Id == mov.Id);

        // if the index exists, itll update the movie, otherwhise itll add a new one
        if (index != -1)
        {
            // updates existing movie
            _movies[index] = mov;
        }
        else
        {
            //adds new movie
            _movies.Add(mov);
        }

        // writes the changed data to the json file
        MovieAccess.WriteAll(_movies);
    }

    public void RemoveMovie(int id)
    {
        // finds if there is a movie with the same id
        int index = _movies.FindIndex(s => s.Id == id);

        // removes the movie with that id, and updates the json file
        _movies.Remove(_movies[index]);
        MovieAccess.WriteAll(_movies);
    }

    public MovieModel GetById(int id)
    {
        // returns the movie data that matches the id
        return _movies.Find(i => i.Id == id);
    }

    private SortOrder GetOrder()
    {
        //Some settings for how the menu will look/act
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        // Prints some instructions for the user
        Console.WriteLine("Would you like to sort ascending or descending?");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nUse ⬆ and ⬇ to navigate and press Enter to select:");
        Console.ResetColor();

        // gets the cursor position and sets option to 1
        (int left, int top) = Console.GetCursorPosition();
        var option = 1;

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

            // prints the options and uses the decorator depending on what value 'option' has
            Console.WriteLine($"{(option == 1 ? decorator : "   ")}Ascending\u001b[0m");
            Console.WriteLine($"{(option == 2 ? decorator : "   ")}Descending\u001b[0m");

            // sees what key has been pressed
            key = Console.ReadKey(false);

            // a switch case that changes the value from 'option', depending on the key input
            switch (key.Key)
            {
                // moves one up
                case ConsoleKey.UpArrow:
                    option = option == 1 ? 2 : option - 1;
                    break;
                    
                // moves one down
                case ConsoleKey.DownArrow:
                    option = option == 2 ? 1 : option + 1;
                    break;

                // if enter is pressed, breaks out of the while loop
                case ConsoleKey.Enter:
                    isSelected = true;
                    break;
            }
        }

        // depending on the option that was chosen, it will clear the console and call the right function
        if (option == 1)
        {
            Console.Clear();
            return SortOrder.ASCENDING;
        }
        else if (option == 2)
        {
            Console.Clear();
            return SortOrder.DESCENDING;
        }

        // failsafe so code always returns something
        return SortOrder.ASCENDING;
    }

    enum SortOrder    
    {
        ASCENDING,
        DESCENDING
    }

    public void PrintMovies(List<MovieModel> to_print)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nMOVIES\n");
        Console.ResetColor();
        
        foreach (MovieModel movie in to_print)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(movie.Title);
            Console.ResetColor();

            Console.WriteLine($"* Genre:\n   {movie.Genre}");
            Console.WriteLine($"* Rating:\n   {movie.Rating}");
            Console.WriteLine($"* Description:\n   {MovieLogic.SpliceText(movie.Description)}");
            Console.WriteLine();
        }
    }

    public void PrintMovies() => PrintMovies(_movies);


//-------------------------------------------------------------------------------------------------

    public List<MovieModel> SortBy(string input)
    {
        SortOrder order = GetOrder();
        List<MovieModel> unsorted = _movies;

        // Check what to sort by per subject
        if (input.ToUpper() == "DATE")
        {
            //This is if the user is a customer, they cannot see movies that have already played anymore.
            DateTime currentDateTime = DateTime.Now;
            if (order == SortOrder.ASCENDING)
                return unsorted.Where(m => m.PublishDate >= currentDateTime).OrderBy(m => m.PublishDate).ToList();
            if (order == SortOrder.DESCENDING)
                return unsorted.Where(m => m.PublishDate >= currentDateTime).OrderByDescending(m => m.PublishDate).ToList();
        }
        else if (input.ToUpper() == "GENRE")
        {
            return (order == SortOrder.ASCENDING) ? unsorted.OrderBy(m => m.Genre).ToList() : unsorted.OrderByDescending(m => m.Genre).ToList();
        }
        else if (input.ToUpper() == "NAME")
        {
            return (order == SortOrder.ASCENDING) ? unsorted.OrderBy(m => m.Title).ToList() : unsorted.OrderByDescending(m => m.Title).ToList();
        }
        else if (input.ToUpper() == "RATING")
        {
            return (order == SortOrder.ASCENDING) ? unsorted.OrderBy(m => m.Rating).ToList() : unsorted.OrderByDescending(m => m.Rating).ToList();
        }
        else if (input.ToUpper() == "PUBLISH")
        {
            return (order == SortOrder.ASCENDING) ? unsorted.OrderBy(m => m.PublishDate).ToList() : unsorted.OrderByDescending(m => m.PublishDate).ToList();
        }
        return unsorted;
    }

    public List<MovieModel> FilterBy(string? genre, bool? mature)
    {
        // copies the original list
        List<MovieModel> filtered = _movies;

        if (genre != null)
            filtered = _movies.Where(movie => movie.Genre == genre).ToList();
        if (mature == true)
            filtered = filtered.Where(movie => movie.Age >= 18).ToList();

        return filtered;
    }

    public List<MovieModel> SearchBy(string query) 
    {
        List<MovieModel> searched = new();
        foreach(MovieModel m in _movies)
            if(m.Title.ToLower().Contains(query.ToLower()) || m.Description.ToLower().Contains(query.ToLower()))
                searched.Add(m);
        
        return searched;
    }

    public static string SpliceText(string inputText) 
    {
        int lineLength = 50;
        string[] stringSplit = inputText.Split(' ');
        int charCounter = 0;
        string finalString = "";
    
        for(int i=0; i < stringSplit.Length; i++)
        {
            finalString += stringSplit[i] + " ";
            charCounter += stringSplit[i].Length;
    
            if(charCounter > lineLength)
            {
                finalString += "\n   ";
                charCounter = 0;
            }
        }
        return finalString;
    }
}