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
    }

    public class ScoreBoard
    {
        public GameLoop GameLoop {  get; set; }
        public List <Players> Players { get; set; }

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
}







