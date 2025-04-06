using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Batak
{
    public class Players
    {
        public string Name { get; set; }
        public List <Card> Hand = new List <Card> ();
        public bool IsHuman { get; set; }
        public int WonHands { get; set; }
        public int Score { get; set; }

        public Players(bool isHuman)
        {
            IsHuman = isHuman;
        }

        public string PlayerNaming()
        {
            if (IsHuman)
            {               
                bool isValidName = false;

                while (!isValidName)
                {
                    Console.WriteLine("Please enter your name.");
                    string playerName = Console.ReadLine();

                    if (!string.IsNullOrEmpty(playerName))
                    {
                        isValidName = true;
                        return Name = playerName;
                    }

                    else
                    {
                        Console.WriteLine("Player name cannot be empty.");
                    }
                }          
            }

            else
            {
                return "Bot" + Guid.NewGuid().ToString("N").Substring(0,4);
            }

            return "Bot12345";
        }

        public void PlayerPlayCard(GameLoop gameLoop, MoveChecker moveChecker)
        {
            //show a list of the cards in player`s hand
            //use all card and game verifiers

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Select the card you wish to play (choose the number).");
            Console.ResetColor();

            DisplayOrderedHand();

            int playerChoice = -1;
            bool validInput = false;
          
            while (true)
            {
                while (!validInput)
                {
                    string playerInput = Console.ReadLine();
                    if (int.TryParse(playerInput, out playerChoice))
                    {
                        if (playerChoice >= 0 && playerChoice < Hand.Count)
                        {
                            validInput = true;
                        }

                        else
                        {
                            Console.WriteLine("The number is out of range. Please choose a valid card number.");
                        }
                    }

                    else
                    {
                        Console.WriteLine("Please enter a valid number.");
                    }
                }

                bool validMove = moveChecker.IsValidMove(Hand[playerChoice]);

                if (validMove)
                {
                    Card chosenCard = Hand[playerChoice];
                    Hand.RemoveAt(playerChoice);
                    gameLoop.CardsInTheMiddle.Add(this, chosenCard);
                    Console.WriteLine($"Card played: {chosenCard.Suit} / {chosenCard.Rank}");
                    break;
                }
            }
        }

        public void DisplayOrderedHand()
        {
            var orderedHand = from card in this.Hand
                              group card by card.Suit into suitGroup
                              orderby suitGroup.Key descending
                              select suitGroup;

            foreach ( var group in orderedHand )
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{group.Key}]\t\t ");
                Console.ResetColor();

                foreach ( var card in group )
                {
                    Console.Write($"{card.Rank} "); // order the rank here
                }
                Console.WriteLine();
            }
        }

        private static readonly Random random = new Random();
        public void BotPlayCard(GameLoop gameLoop, MoveChecker moveChecker)
        {
            bool validMove = false;

            while(!validMove)
            {
                int cardIndex = random.Next(0, Hand.Count);
                Card randomCard = Hand[cardIndex];

                if (moveChecker.IsValidMove(randomCard))
                {
                    Card chosenCard = randomCard;
                    Hand.Remove(randomCard);
                    gameLoop.CardsInTheMiddle.Add(this, chosenCard);
                    Console.WriteLine($"Card played: {chosenCard.Suit} / {chosenCard.Rank}");
                    validMove = true;
                }
            }           
        }
    }
}


