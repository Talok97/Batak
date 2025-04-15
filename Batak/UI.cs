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
        public AsciiArt AsciiArt { get; set; }

        private const int SuitRegionLeft = 100;
        private const int SuitRegionTop = 2;
        private const int SuitRegionWidth = 40;
        private const int SuitRegionHeight = 20;       
        public UI()
        {
            AsciiArt = new AsciiArt();
        }
        
        public void SubscribeToEvents(MoveChecker moveChecker)
        {
            moveChecker.SuitEvent += OnSuitEvent;
        }

        public void OnSuitEvent(object sender, SuitEventArgs e)
        {
            string art = "";
            AsciiArt.ClearAsciiRegion(SuitRegionLeft, SuitRegionTop, SuitRegionWidth, SuitRegionHeight);
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

            int top = SuitRegionTop;

            AsciiArt.DrawSuit(art, SuitRegionLeft, top);

            Console.SetCursorPosition(0, 0);

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
        public static readonly string Club = "     .-~~-.\r\n    {      }\r\n .-~-.    .-~-.\r\n{              }\r\n `.__.'||`.__.'\r\n       ||\r\n      '--`";
        public static readonly string Heart = " .-~~~-__-~~~-.\r\n{              }\r\n `.          .'\r\n   `.      .'\r\n     `.  .'\r\n       \\/";
        public static readonly string Spade = "       /\\\r\n     .'  `.\r\n    '      `.\r\n .'          `.\r\n{              }\r\n ~-...-||-...-~\r\n       ||\r\n      '--`";
        public static readonly string Diamond = "     /\\\r\n   .'  `.\r\n  '      `.\r\n<          >\r\n `.      .'\r\n   `.  .'\r\n     \\/";

        public void DrawSuit(string art, int left, int top)
        {
            string[] lines = art.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for(int i = 0;  i < lines.Length; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.WriteLine(lines[i]);
            }

            //Console.SetCursorPosition(0, top + lines.Length);
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







