namespace AiRagMovieApp;

public static class MovieRepository
{
    public static IEnumerable<Movie> GetMovies()
    {
        return
        [
            // Christopher Nolan — The Dark Knight Trilogy
            new Movie
            {
                Id = 1,
                Title = "Batman Begins",
                Description = "After training with his mentor, Batman begins his fight to free crime-ridden Gotham City from corruption.",
                Reference = "https://en.wikipedia.org/wiki/Batman_Begins",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 2,
                Title = "The Dark Knight",
                Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                Reference = "https://en.wikipedia.org/wiki/The_Dark_Knight",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 3,
                Title = "The Dark Knight Rises",
                Description = "Eight years after the Joker's reign of anarchy, Batman is forced from his exile by the mercenary Bane.",
                Reference = "https://en.wikipedia.org/wiki/The_Dark_Knight_Rises",
                Embedding = new float[768]
            },

            // Denis Villeneuve — Dune Duology
            new Movie
            {
                Id = 4,
                Title = "Dune: Part One",
                Description = "A brilliant and gifted young man born into a great destiny must travel to the most dangerous planet in the universe to ensure the future of his family and his people.",
                Reference = "https://en.wikipedia.org/wiki/Dune_(2021_film)",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 5,
                Title = "Dune: Part Two",
                Description = "Paul Atreides unites with Chani and the Fremen while seeking revenge against the conspirators who destroyed his family.",
                Reference = "https://en.wikipedia.org/wiki/Dune:_Part_Two",
                Embedding = new float[768]
            },

            // Quentin Tarantino — Crime Universe
            new Movie
            {
                Id = 6,
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster's wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                Reference = "https://en.wikipedia.org/wiki/Pulp_Fiction",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 7,
                Title = "Reservoir Dogs",
                Description = "When a simple jewelry heist goes horribly wrong, the surviving criminals begin to suspect that one of them is a police informant.",
                Reference = "https://en.wikipedia.org/wiki/Reservoir_Dogs",
                Embedding = new float[768]
            },

            // Martin Scorsese — Mob Dramas
            new Movie
            {
                Id = 8,
                Title = "Goodfellas",
                Description = "The story of Henry Hill and his life in the mafia, covering his relationship with his wife Karen Hill and his mob partners Jimmy Conway and Tommy DeVito.",
                Reference = "https://en.wikipedia.org/wiki/Goodfellas",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 9,
                Title = "Casino",
                Description = "A tale of greed, deception, money, power, and murder occur between two mob best friends and a trophy wife over a gambling empire.",
                Reference = "https://en.wikipedia.org/wiki/Casino_(1995_film)",
                Embedding = new float[768]
            },

            // Francis Ford Coppola — The Godfather Saga
            new Movie
            {
                Id = 10,
                Title = "The Godfather",
                Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                Reference = "https://en.wikipedia.org/wiki/The_Godfather",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 11,
                Title = "The Godfather Part II",
                Description = "The early life and career of Vito Corleone in 1920s New York City is portrayed, while his son, Michael, expands and tightens his grip on the family crime syndicate.",
                Reference = "https://en.wikipedia.org/wiki/The_Godfather_Part_II",
                Embedding = new float[768]
            },

            // Ridley Scott — Historic Epics
            new Movie
            {
                Id = 12,
                Title = "Gladiator",
                Description = "A former Roman General sets out to exact vengeance against the corrupt emperor who murdered his family and sent him into slavery.",
                Reference = "https://en.wikipedia.org/wiki/Gladiator_(2000_film)",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 13,
                Title = "Kingdom of Heaven",
                Description = "Balian of Ibelin travels to Jerusalem during the Crusades of the 12th century, and there he finds himself as the defender of the city and its people.",
                Reference = "https://en.wikipedia.org/wiki/Kingdom_of_Heaven_(film)",
                Embedding = new float[768]
            },

            // Sony Pictures Animation — Spider-Verse
            new Movie
            {
                Id = 14,
                Title = "Spider-Man: Into the Spider-Verse",
                Description = "Teen Miles Morales becomes the Spider-Man of his universe and must join with five spider-powered individuals from other dimensions to stop a threat for all realities.",
                Reference = "https://en.wikipedia.org/wiki/Spider-Man:_Into_the_Spider-Verse",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 15,
                Title = "Spider-Man: Across the Spider-Verse",
                Description = "Miles Morales catapults across the Multiverse, where he encounters a team of Spider-People charged with protecting its very existence.",
                Reference = "https://en.wikipedia.org/wiki/Spider-Man:_Across_the_Spider-Verse",
                Embedding = new float[768]
            },

            // James Cameron — Sci-Fi Blockbusters
            new Movie
            {
                Id = 16,
                Title = "The Terminator",
                Description = "A human soldier is sent from 2029 to 1984 to protect a woman whose unborn son will lead humanity in a war against malicious robots.",
                Reference = "https://en.wikipedia.org/wiki/The_Terminator",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 17,
                Title = "Terminator 2: Judgment Day",
                Description = "A cyborg, identical to the one who failed to kill Sarah Connor, must now protect her ten-year-old son John from a more advanced cyborg.",
                Reference = "https://en.wikipedia.org/wiki/Terminator_2:_Judgment_Day",
                Embedding = new float[768]
            },

            // David Fincher — Dark Thrillers
            new Movie
            {
                Id = 18,
                Title = "Se7en",
                Description = "Two detectives, a rookie and a veteran, hunt a serial killer who uses the seven deadly sins as his motives.",
                Reference = "https://en.wikipedia.org/wiki/Seven_(1995_film)",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 19,
                Title = "Fight Club",
                Description = "An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into much more.",
                Reference = "https://en.wikipedia.org/wiki/Fight_Club",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 20,
                Title = "Zodiac",
                Description = "Between 1968 and 1983, a San Francisco cartoonist becomes an amateur detective obsessed with tracking down the Zodiac Killer.",
                Reference = "https://en.wikipedia.org/wiki/Zodiac_(film)",
                Embedding = new float[768]
            },
        ];
    }
}
