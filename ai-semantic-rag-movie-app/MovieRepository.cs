namespace AiSemanticRagMovieApp;

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
                Year = 2005,
                Description = "Released in 2005 and directed by Christopher Nolan, this superhero origin film reboots the franchise, starring Christian Bale, Michael Caine, Liam Neeson, and Gary Oldman as Gotham fights structural corruption.",
                Reference = "https://en.wikipedia.org/wiki/Batman_Begins",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 2,
                Title = "The Dark Knight",
                Year = 2008,
                Description = "Released in 2008 and directed by Christopher Nolan, this psychological superhero film stars Christian Bale, Heath Ledger, and Aaron Eckhart as Gotham falls into chaos engineered by the Joker.",
                Reference = "https://en.wikipedia.org/wiki/The_Dark_Knight",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 3,
                Title = "The Dark Knight Rises",
                Year = 2012,
                Description = "Released in 2012 and directed by Christopher Nolan, this trilogy conclusion stars Christian Bale, Tom Hardy, Anne Hathaway, and Marion Cotillard as a revolutionary mercenary forces Batman from exile.",
                Reference = "https://en.wikipedia.org/wiki/The_Dark_Knight_Rises",
                Embedding = new float[768]
            },

            // Denis Villeneuve — Dune Duology
            new Movie
            {
                Id = 4,
                Title = "Dune: Part One",
                Year = 2021,
                Description = "Released in 2021 and directed by Denis Villeneuve, this epic space opera adapts Frank Herbert's novel, starring Timothée Chalamet, Rebecca Ferguson, Oscar Isaac, and Josh Brolin on a dangerous desert planet.",
                Reference = "https://en.wikipedia.org/wiki/Dune_(2021_film)",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 5,
                Title = "Dune: Part Two",
                Year = 2024,
                Description = "Released in 2024 and directed by Denis Villeneuve, this sci-fi sequel follows the rise of a mythical messiah, starring Timothée Chalamet, Zendaya, Rebecca Ferguson, and Austin Butler.",
                Reference = "https://en.wikipedia.org/wiki/Dune:_Part_Two",
                Embedding = new float[768]
            },

            // Quentin Tarantino — Crime Universe
            new Movie
            {
                Id = 6,
                Title = "Pulp Fiction",
                Year = 1994,
                Description = "Released in 1994 and directed by Quentin Tarantino, this landmark neo-noir crime film features intertwined Los Angeles criminal storylines starring John Travolta, Samuel L. Jackson, Uma Thurman, and Bruce Willis.",
                Reference = "https://en.wikipedia.org/wiki/Pulp_Fiction",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 7,
                Title = "Reservoir Dogs",
                Year = 1992,
                Description = "Released in 1992 and directed by Quentin Tarantino, this independent heist thriller charts a diamond robbery gone wrong, starring Harvey Keitel, Tim Roth, Michael Madsen, and Steve Buscemi.",
                Reference = "https://en.wikipedia.org/wiki/Reservoir_Dogs",
                Embedding = new float[768]
            },

            // Martin Scorsese — Mob Dramas
            new Movie
            {
                Id = 8,
                Title = "Goodfellas",
                Year = 1990,
                Description = "Released in 1990 and directed by Martin Scorsese, this biographical mob masterpiece chronicles the rise and fall of Henry Hill, starring Robert De Niro, Ray Liotta, Joe Pesci, and Lorraine Bracco.",
                Reference = "https://en.wikipedia.org/wiki/Goodfellas",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 9,
                Title = "Casino",
                Year = 1995,
                Description = "Released in 1995 and directed by Martin Scorsese, this epic crime drama tracks mob enforcers operating a Las Vegas gambling empire, starring Robert De Niro, Sharon Stone, and Joe Pesci.",
                Reference = "https://en.wikipedia.org/wiki/Casino_(1995_film)",
                Embedding = new float[768]
            },

            // Francis Ford Coppola — The Godfather Saga
            new Movie
            {
                Id = 10,
                Title = "The Godfather",
                Year = 1972,
                Description = "Released in 1972 and directed by Francis Ford Coppola, this legendary mafia chronicle tracks a patriarch handing over control to his son, starring Marlon Brando, Al Pacino, James Caan, and Diane Keaton.",
                Reference = "https://en.wikipedia.org/wiki/The_Godfather",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 11,
                Title = "The Godfather Part II",
                Year = 1974,
                Description = "Released in 1974 and directed by Francis Ford Coppola, this parallel prequel-sequel follows the family expansion, starring Al Pacino, Robert De Niro, Robert Duvall, and Diane Keaton.",
                Reference = "https://en.wikipedia.org/wiki/The_Godfather_Part_II",
                Embedding = new float[768]
            },

            // Ridley Scott — Historic Epics
            new Movie
            {
                Id = 12,
                Title = "Gladiator",
                Year = 2000,
                Description = "Released in 2000 and directed by Ridley Scott, this historical epic follows a betrayed Roman General seeking vengeance in the Colosseum, starring Russell Crowe, Joaquin Phoenix, Connie Nielsen, and Oliver Reed.",
                Reference = "https://en.wikipedia.org/wiki/Gladiator_(2000_film)",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 13,
                Title = "Kingdom of Heaven",
                Year = 2005,
                Description = "Released in 2005 and directed by Ridley Scott, this epic historical drama centers on the defense of Jerusalem during the Crusades, starring Orlando Bloom, Eva Green, Ghassan Massoud, and Jeremy Irons.",
                Reference = "https://en.wikipedia.org/wiki/Kingdom_of_Heaven_(film)",
                Embedding = new float[768]
            },

            // Sony Pictures Animation — Spider-Verse
            new Movie
            {
                Id = 14,
                Title = "Spider-Man: Into the Spider-Verse",
                Year = 2018,
                Description = "Released in 2018 and directed by Bob Persichetti, Peter Ramsey, and Rodney Rothman, this animated comic book milestone stars Shameik Moore, Jake Johnson, Hailee Steinfeld, and Mahershala Ali.",
                Reference = "https://en.wikipedia.org/wiki/Spider-Man:_Into_the_Spider-Verse",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 15,
                Title = "Spider-Man: Across the Spider-Verse",
                Year = 2023,
                Description = "Released in 2023 and directed by Joaquim Dos Santos, Kemp Powers, and Justin K. Thompson, this multiverse animated sequel stars Shameik Moore, Hailee Steinfeld, Oscar Isaac, and Jason Schwartzman.",
                Reference = "https://en.wikipedia.org/wiki/Spider-Man:_Across_the_Spider-Verse",
                Embedding = new float[768]
            },

            // James Cameron — Sci-Fi Blockbusters
            new Movie
            {
                Id = 16,
                Title = "The Terminator",
                Year = 1984,
                Description = "Released in 1984 and directed by James Cameron, this gritty sci-fi action thriller follows a cyborg assassin hunting its target, starring Arnold Schwarzenegger, Michael Biehn, and Linda Hamilton.",
                Reference = "https://en.wikipedia.org/wiki/The_Terminator",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 17,
                Title = "Terminator 2: Judgment Day",
                Year = 1991,
                Description = "Released in 1991 and directed by James Cameron, this revolutionary action sequel utilizes breakthrough visual effects, starring Arnold Schwarzenegger, Linda Hamilton, Edward Furlong, and Robert Patrick.",
                Reference = "https://en.wikipedia.org/wiki/Terminator_2:_Judgment_Day",
                Embedding = new float[768]
            },

            // David Fincher — Dark Thrillers
            new Movie
            {
                Id = 18,
                Title = "Se7en",
                Year = 1995,
                Description = "Released in 1995 and directed by David Fincher, this dark neo-noir psychological thriller follows detectives hunting a sin-obsessed serial killer, starring Brad Pitt, Morgan Freeman, and Gwyneth Paltrow.",
                Reference = "https://en.wikipedia.org/wiki/Seven_(1995_film)",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 19,
                Title = "Fight Club",
                Year = 1999,
                Description = "Released in 1999 and directed by David Fincher, this counter-culture satire explores societal isolation via underground fighting leagues, starring Brad Pitt, Edward Norton, and Helena Bonham Carter.",
                Reference = "https://en.wikipedia.org/wiki/Fight_Club",
                Embedding = new float[768]
            },
            new Movie
            {
                Id = 20,
                Title = "Zodiac",
                Year = 2007,
                Description = "Released in 2007 and directed by David Fincher, this analytical true-crime procedural charts the decades-long hunt for a serial killer, starring Jake Gyllenhaal, Mark Ruffalo, and Robert Downey Jr.",
                Reference = "https://en.wikipedia.org/wiki/Zodiac_(film)",
                Embedding = new float[768]
            },
        ];
    }
}
