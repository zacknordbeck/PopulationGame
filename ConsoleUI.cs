using System;
using System.Threading;

namespace PopulationGame
{
    public static class ConsoleUI
    {
        public static int BoxWidth = 70;
        public static int BoxHeight = 12; // Ökar höjden lite så att vi får plats med extra information
        public static int AnimationDelay = 50; // Fördröjning i ms per bokstav

        /// <summary>
        /// Ritar hela UI:et med användardata, ländernamn och populationer.
        /// </summary>
        /// <param name="username">Användarens namn.</param>
        /// <param name="currentStreak">Aktuell streak.</param>
        /// <param name="highscore">Global highscore.</param>
        /// <param name="leftCountryName">Vänster lands namn.</param>
        /// <param name="rightCountryName">Höger lands namn.</param>
        /// <param name="leftPopulation">Vänster lands population (formaterat sträng, t.ex. "10 327 589").</param>
        /// <param name="rightPopulation">Höger lands population.</param>
        public static void RenderUI(
            string username,
            int currentStreak,
            int highscore,
            string leftCountryName,
            string rightCountryName,
            string leftPopulation = null,
            string rightPopulation = null)
        {
            Console.Clear();
            DrawBox(BoxWidth, BoxHeight);

            // Placera användardata på första raden
            Console.SetCursorPosition(2, 1);
            Console.Write($"Användare: {username}");
            Console.SetCursorPosition(BoxWidth / 2 - 6, 1);
            Console.Write($"Streak: {currentStreak}");
            Console.SetCursorPosition(BoxWidth - 18, 1);
            Console.Write($"Highscore: {highscore}");

            // Rita en vertikal linje i mitten
            for (int row = 2; row < BoxHeight - 1; row++)
            {
                Console.SetCursorPosition(BoxWidth / 2, row);
                Console.Write("│");
            }

            // Bestäm positionerna för vänster och höger lands namn
            int middleRow = BoxHeight / 2 - 1; // Väljer en rad lite över mitten för landsnamnet
            int leftX = BoxWidth / 4 - leftCountryName.Length / 2;
            int rightX = (3 * BoxWidth / 4) - rightCountryName.Length / 2;

            // Skriv ut ländernamnen med animation
            WriteAnimated(leftCountryName, leftX, middleRow, AnimationDelay);
            WriteAnimated(rightCountryName, rightX, middleRow, AnimationDelay);

            // Om populationen finns, skriv ut den precis under respektive lands namn
            if (!string.IsNullOrEmpty(leftPopulation))
            {
                int leftPopX = BoxWidth / 4 - leftPopulation.Length / 2;
                Console.SetCursorPosition(leftPopX, middleRow + 1);
                Console.Write(leftPopulation);
            }
            if (!string.IsNullOrEmpty(rightPopulation))
            {
                int rightPopX = (3 * BoxWidth / 4) - rightPopulation.Length / 2;
                Console.SetCursorPosition(rightPopX, middleRow + 1);
                Console.Write(rightPopulation);
            }
        }

        // Skriver ut text med animation (tecken för tecken)
        public static void WriteAnimated(string text, int x, int y, int delay)
        {
            Console.SetCursorPosition(x, y);
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
        }

        // Rita en enkel box med Unicode-ramar
        private static void DrawBox(int width, int height)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("┌" + new string('─', width - 2) + "┐");
            for (int row = 1; row < height - 1; row++)
            {
                Console.SetCursorPosition(0, row);
                Console.Write("│" + new string(' ', width - 2) + "│");
            }
            Console.SetCursorPosition(0, height - 1);
            Console.Write("└" + new string('─', width - 2) + "┘");
        }
    }
}
