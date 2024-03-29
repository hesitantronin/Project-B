using System.Text.Json.Serialization;

public class MovieModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("genre")]
    public string Genre { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("viewing_date")]
    public DateTime ViewingDate { get; set; }

    [JsonPropertyName("publish_date")]
    public DateTime PublishDate { get; set; }

    [JsonPropertyName("runtime")]
    public string RunTime { get; set; }

    [JsonPropertyName("movie_price")]
    public double MoviePrice { get; set; }

    [JsonPropertyName("timeslot")]
    public List<int> TimeSlot { get; set; }

    public MovieModel(int id, string title, string genre, double rating, string description, int age, DateTime viewingdate, DateTime publishdate, string runtime, List<int> timeslot, double movieprice = 10.99)
    {
        Id = id;
        Title = title;
        Genre = genre;
        Rating = rating;
        Description = description;
        Age = age;
        ViewingDate = viewingdate;
        PublishDate = publishdate;
        RunTime = runtime;
        TimeSlot = timeslot;
        MoviePrice = movieprice;
    }
}



