using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AiSemanticRagMovieApp;

public class MoviePlugin
{
    [KernelFunction, Description("Get the current date and time.")]
    public string Now()
    {
        Console.WriteLine("---------------Now called------------------");
        return DateTime.Now.ToString("f");
    }

    [KernelFunction, Description("Compare two movies by title and return their details.")]
    public async Task<string> CompareMovies(
        [Description("source movie title")] string sourceMovieTitle, [Description("destination movie title")] string destinationMovieTitle)
    {
        Console.WriteLine("---------------CompareMovies called------------------");
        var movies = MovieRepository.GetMovies().ToList();
        await Task.Delay(1000);
        var sourceMovie = movies.FirstOrDefault(x => x.Title.Equals(sourceMovieTitle, StringComparison.OrdinalIgnoreCase));
        var destinationMovie = movies.FirstOrDefault(x => x.Title.Equals(destinationMovieTitle, StringComparison.OrdinalIgnoreCase));

        return $"""
                Comparison between {sourceMovieTitle} and {destinationMovieTitle}:
                Title: {sourceMovie?.Title} vs {destinationMovie?.Title}
                Year: {sourceMovie?.Year} vs {destinationMovie?.Year}
                Description: {sourceMovie?.Description} vs {destinationMovie?.Description}
                """;
    }
}
