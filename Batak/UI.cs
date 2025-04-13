using Batak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batak
{
    public class UI
    {
        public SuitEventArgs SuitEventArgs { get; set; }
        public TrumpSuitArgs TrumpSuitArgs { get; set; }
        public AsciiArt AsciiArt { get; set; }

        public UI()
        {
            AsciiArt = new AsciiArt();
        }
        
        public void SubscribeToEvents(MoveChecker moveChecker)
        {
            moveChecker.SuitEvent += OnSuitEvent;
            moveChecker.TrumpEvent += OnTrumpEvent;
        }

        public void OnSuitEvent(object sender, SuitEventArgs e)
        {
            string art = "";
            AsciiArt.ClearAsciiRegion(80, 2, 40, 20);
            switch (e.SuitOfCardsInTheMiddle)
            {
                case Suit.Hearts:
                    {
                        art = AsciiArt.Heart;
                        break;
                    }
                case Suit.Diamonds:
                    {
                        art = AsciiArt.Diamond;
                        break;
                    }
                case Suit.Clubs:
                    {
                        art = AsciiArt.Club;
                        break;
                    }
                case Suit.Spades:
                    {
                        art = AsciiArt.Spade;
                        break;
                    }
            }

            int artWidth = art.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                  .Max(line => line.Length);
            int left = Console.WindowWidth - artWidth - 2;
            int top = 2;

            AsciiArt.DrawSuit(art, left, top);
            
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        public void OnTrumpEvent(object sender, TrumpSuitArgs e)
        {
            string art = "";
            AsciiArt.ClearAsciiRegion(80, 2, 40, 20);
            switch (e.TrumpSuit)
            {
                case Suit.Hearts:
                    {
                        art = AsciiArt.Heart;
                        break;
                    }
                case Suit.Diamonds:
                    {
                        art = AsciiArt.Diamond;
                        break;
                    }
                case Suit.Clubs:
                    {
                        art = AsciiArt.Club;
                        break;
                    }
                case Suit.Spades:
                    {
                        art = AsciiArt.Spade;
                        break;
                    }
            }

            int artWidth = art.Split("\r\n").Max(line => line.Length);
            int left = Console.WindowWidth - artWidth - 2;
            int top = 12;

            Console.Beep();
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("TRUMP SUIT");
            Console.ResetColor();
           
            AsciiArt.DrawSuit(art, left, top);
                       
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }

    public class ScoreBoard
    {
        public GameLoop GameLoop { get; set; }
        public List<Players> Players { get; set; }

        public ScoreBoard(GameLoop gameLoop, List<Players> players)
        {
            GameLoop = gameLoop;
            Players = players;
        }

        public void DisplayScoreboard()
        {
            Console.WriteLine("\n|----------------------------------------|");
            Console.WriteLine($"| {"Name",-15} | {"Score",10} |");
            Console.WriteLine("|----------------------------------------|");

            foreach (var player in Players)
            {
                Console.WriteLine($"| {player.Name,-15} | {player.Score,10} |");
            }
            Console.WriteLine("|----------------------------------------|\n");
        }

    }

    public class AsciiArt
    {
        public static readonly string Club = " .-~~-.\r\n    {      }\r\n .-~-.    .-~-.\r\n{              }\r\n `.__.'||`.__.'\r\n       ||\r\n      '--`";
        public static readonly string Heart = ".-~~~-__-~~~-.\r\n{              }\r\n `.          .'\r\n   `.      .'\r\n     `.  .'\r\n       \\/";
        public static readonly string Spade = "    /\\\r\n     .'  `.\r\n    '      `.\r\n .'          `.\r\n{              }\r\n ~-...-||-...-~\r\n       ||\r\n      '--`";
        public static readonly string Diamond = "/\\\r\n   .'  `.\r\n  '      `.\r\n<          >\r\n `.      .'\r\n   `.  .'\r\n     \\/";

        public void DrawSuit(string art, int left, int top)
        {
            string[] lines = art.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for(int i = 0;  i < lines.Length; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.WriteLine(lines[i]);
            }

            Console.SetCursorPosition(0, top + lines.Length);
        }

        public void ClearAsciiRegion(int left, int top, int regionWidth, int regionHeight)
        {
            for (int row = 0; row <= regionWidth; row++)
            {
                Console.SetCursorPosition(left, top + row);
                Console.WriteLine(new string(' ', regionWidth));
            }
        }
    }
}







