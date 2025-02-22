using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PopulationGame.Interfaces;
using PopulationGame.Models;
using PopulationGame.Repositories;

namespace PopulationGame;

public class GameRunner
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IUserRepository _userRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IUserGameLogRepository _userGameLogRepository;
    private readonly Random _rand = new Random();

    public User CurrentUser { get; private set; }

    public GameRunner(
        IDbConnectionFactory connectionFactory,
        IUserRepository userRepository,
        ICountryRepository countryRepository,
        IUserGameLogRepository userGameLogRepository)
    {
        _connectionFactory = connectionFactory;
        _userRepository = userRepository;
        _countryRepository = countryRepository;
        _userGameLogRepository = userGameLogRepository;
    }

    public void Run()
    {
        // Logga in användaren först
        LoginUser();
        if (CurrentUser == null)
        {
            Console.WriteLine("Något gick fel med inloggningen. Avslutar...");
            return;
        }

        // Visa huvudmenyn
        MainMenu();
    }

    private void MainMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine($"Välkommen {CurrentUser.Username}!");
            Console.WriteLine("Välj ett alternativ:");
            Console.WriteLine("1. Starta nytt spel");
            Console.WriteLine("2. Visa spelhistorik");
            Console.WriteLine("3. Avsluta");

            var key = Console.ReadKey(intercept: true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    RunGameLoop();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    ShowUserGameHistory();
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                case ConsoleKey.Escape:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Ogiltigt val, försök igen...");
                    Thread.Sleep(1000);
                    break;
            }
        }
    }
    private void RunGameLoop()
    {
        string difficulty = AskForDifficulty();
        var allCountries = _countryRepository.GetAllCountries().ToList();
        // Välj initialt ett par länder utan population (de visas först med bara namn)
        var (leftCountry, rightCountry) = SelectCountryPair(allCountries, difficulty);

        int currentStreak = 0;
        int totalGuesses = 0;
        string feedbackMessage = string.Empty;
        int globalHighscore = _userGameLogRepository.GetGlobalHighScore();
        DateTime startTime = DateTime.Now;
        bool exitGameLoop = false;

        while (!exitGameLoop)
        {
            // 1. Rita UI:t med aktuella ländernamn (utan population)
            ConsoleUI.RenderUI(
                username: CurrentUser?.Username ?? "Okänd",
                currentStreak: currentStreak,
                highscore: globalHighscore,
                leftCountryName: leftCountry.Name,
                rightCountryName: rightCountry.Name
            );
            // Visa instruktioner utanför boxen
            Console.SetCursorPosition(0, ConsoleUI.BoxHeight + 1);
            Console.WriteLine("Tryck vänster eller höger pil för att gissa, ESC för att avsluta spelet...");

            // 2. Vänta på användarens inmatning
            var keyInfo = Console.ReadKey(intercept: true);
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                exitGameLoop = true;
                break;
            }
            bool isLeftSelected = keyInfo.Key == ConsoleKey.LeftArrow;
            totalGuesses++;

            // 3. Spara det aktuella paret för att visa population och feedback
            var oldLeft = leftCountry;
            var oldRight = rightCountry;

            // 4. Utvärdera gissningen
            Country correctCountry = oldLeft.Population > oldRight.Population ? oldLeft : oldRight;
            bool isCorrect = (isLeftSelected && correctCountry == oldLeft) ||
                             (!isLeftSelected && correctCountry == oldRight);

            if (isCorrect)
            {
                currentStreak++;
                feedbackMessage = "Rätt!";
                // Om gissningen är korrekt uppdatera endast det icke-valda landet,
                // men behåll det gissade paret (oldLeft/oldRight) för att visa population.
                if (isLeftSelected)
                    rightCountry = GetRandomCountryByDifficulty(allCountries, oldLeft, difficulty);
                else
                    leftCountry = GetRandomCountryByDifficulty(allCountries, oldRight, difficulty);

                if (currentStreak > globalHighscore)
                    globalHighscore = currentStreak;
            }
            else
            {
                feedbackMessage = "Fel!";
                currentStreak = 0;
                // Vid fel väljer vi ett helt nytt par,
                // men visar population för det gamla paret först.
                (leftCountry, rightCountry) = SelectCountryPair(allCountries, difficulty);
            }

            // 5. Visa UI med population för det gissade paret och feedback.
            // Använder de sparade värdena oldLeft och oldRight.
            ConsoleUI.RenderUI(
                username: CurrentUser?.Username ?? "Okänd",
                currentStreak: currentStreak,
                highscore: globalHighscore,
                leftCountryName: oldLeft.Name,
                rightCountryName: oldRight.Name,
                leftPopulation: oldLeft.Population.ToString("N0"),
                rightPopulation: oldRight.Population.ToString("N0")
            );
            // Placera feedback utanför boxen (du kan justera radposition om du vill)
            Console.SetCursorPosition(0, ConsoleUI.BoxHeight + 1);
            Console.WriteLine(feedbackMessage);
            Console.WriteLine("Tryck på mellanslag (Space) för att fortsätta...");

            // Vänta tills användaren trycker på Space
            while (Console.ReadKey(intercept: true).Key != ConsoleKey.Spacebar) { }
        }

        DateTime endTime = DateTime.Now;
        var gameLog = new UserGameLog
        {
            UserId = CurrentUser.UserId,
            StartTime = startTime,
            EndTime = endTime,
            Streak = currentStreak,
            Guesses = totalGuesses,
            Difficulty = difficulty
        };
        _userGameLogRepository.CreateUserGameLog(gameLog);

        Console.Clear();
        Console.WriteLine("Spelet avslutas. Tryck på en tangent för att återgå till huvudmenyn...");
        Console.ReadKey(true);
    }
    public void LoginUser()
    {
        Console.WriteLine("Ange ditt användarnamn:");
        string username = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Användarnamn får inte vara tomt!");
            return;
        }

        var user = _userRepository.GetUserByUsername(username);
        if (user == null)
        {
            Console.WriteLine("Användaren hittades inte, skapar ny användare...");
            user = new User { Username = username };
            _userRepository.CreateUser(user);
            Console.WriteLine($"Användare '{user.Username}' skapad med id: {user.UserId}");
        }
        else
        {
            Console.WriteLine($"Välkommen tillbaka, {user.Username}!");
        }
        CurrentUser = user;
    }
    public string AskForDifficulty()
    {
        Console.Clear();
        Console.WriteLine("Välj svårighetsgrad:");
        Console.WriteLine("[1] Lätt");
        Console.WriteLine("[2] Svårt");
        var key = Console.ReadKey(intercept: true);
        Console.WriteLine();
        if (key.KeyChar == '1')
            return "Lätt";
        else if (key.KeyChar == '2')
            return "Svårt";
        else
        {
            Console.WriteLine("Ogiltigt val, standardiserar till Lätt.");
            return "Lätt";
        }
    }
    private (Country left, Country right) SelectCountryPair(List<Country> allCountries, string difficulty)
    {
        Country left, right;
        while (true)
        {
            left = allCountries[_rand.Next(allCountries.Count)];
            right = allCountries[_rand.Next(allCountries.Count)];

            if (left.CountryId == right.CountryId)
                continue;

            double ratio = left.Population > right.Population
                ? (double)left.Population / right.Population
                : (double)right.Population / left.Population;

            if (difficulty == "Lätt" && ratio >= 1.5)
                return (left, right);
            if (difficulty == "Svårt" && ratio < 1.5)
                return (left, right);
        }
    }
    private Country GetRandomCountryByDifficulty(List<Country> allCountries, Country referenceCountry, string difficulty)
    {
        while (true)
        {
            var candidate = allCountries[_rand.Next(allCountries.Count)];
            if (candidate.CountryId == referenceCountry.CountryId)
                continue;

            double ratio = referenceCountry.Population > candidate.Population
                ? (double)referenceCountry.Population / candidate.Population
                : (double)candidate.Population / referenceCountry.Population;

            if (difficulty == "Lätt" && ratio >= 1.5)
                return candidate;
            if (difficulty == "Svårt" && ratio < 1.5)
                return candidate;
        }
    }
    public void ShowUserGameHistory()
    {
        // Hämta alla spelomgångar för den inloggade användaren
        var logs = _userGameLogRepository.GetGameLogsForUser(CurrentUser.UserId).ToList();

        Console.Clear();
        Console.WriteLine($"Spelhistorik för {CurrentUser.Username}");
        Console.WriteLine("-------------------------------------------------------");

        if (logs.Any())
        {
            foreach (var log in logs)
            {
                Console.WriteLine($"Start: {log.StartTime}");
                Console.WriteLine($"Slut: {log.EndTime}");
                Console.WriteLine($"Streak: {log.Streak}");
                Console.WriteLine($"Antal gissningar: {log.Guesses}");
                Console.WriteLine($"Svårighet: {log.Difficulty}");
                Console.WriteLine("-------------------------------------------------------");
            }
        }
        else
        {
            Console.WriteLine("Ingen spelhistorik hittades.");
            Console.WriteLine("-------------------------------------------------------");
        }

        Console.WriteLine("Tryck på en tangent för att återgå till menyn...");
        Console.ReadKey(true);
    }

}
