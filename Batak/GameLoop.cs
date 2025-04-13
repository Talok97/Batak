using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Batak
{
    public class GameLoop
    {
        public List<Card> Deck = new List<Card>();
        public List<Players> Players = new List<Players>(4);
        public Dictionary<Players, int> Bids = new Dictionary<Players, int>();
        public Suit Trump { get; set; }
        public int Rounds { get; set; }

        public Dictionary<Players, Card> CardsInTheMiddle = new Dictionary<Players, Card>();    

        public Players RoundStarter {  get; set; }

        public GameLoop(int rounds = 13)
        {
            Rounds = rounds;
        }

        public void DeckBuilder()
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                        Deck.Add(new Card(suit, rank));
                }
            }
        }

        public void CardDistributer(Players player)
        {
            Random random = new Random();
       
                for (int j = 0; j < 13; j++)
                {
                    int i = random.Next(Deck.Count);
                    if (Deck[i] != null)
                    {
                        Card dealtCard = Deck[i];
                        Deck.RemoveAt(i);
                        player.Hand.Add(dealtCard);
                    }
                }                                  
        }

        public void Bidding()
        {
            int minimumBid = 4;

            foreach (Players players in Players)
            {
                bool validInput = false;

                if (players.IsHuman)
                {
                    while (!validInput)
                    {
                        Console.WriteLine($"\n{players.Name} bids");
                        Console.WriteLine("Declare your bid (Min: 5) or pass");
                        
                        players.DisplayOrderedHand();
                        string bid = Console.ReadLine();

                        if (string.IsNullOrEmpty(bid))
                        {
                            Console.WriteLine("Bid cannot be empty.");
                            continue;
                        }

                        if (bid.Trim().ToLower() == "pass")
                        {
                            Bids.Add(players, 0);
                            validInput = true;
                        }

                        else if (int.TryParse(bid, out int bidValue))
                        {
                            if (bidValue > minimumBid)
                            {
                                minimumBid = int.Parse(bid);
                                Bids.Add(players, int.Parse(bid));
                                validInput = true;
                            }
                            else
                            {
                                Console.WriteLine("Your bid must be higher than minimum bid or at least 5.");
                            }
                        }

                        else
                        {
                            Console.WriteLine("Please enter either a number or \"pass\".");
                        }
                    }
                }
                else
                {
                    int bestBid = 0;
                    foreach (Suit s in Enum.GetValues(typeof(Suit)))
                    {
                        int suitCount = players.Hand.Count(card => card.Suit == s);
                        int highCardPoints = players.Hand.Where(card => card.Suit == s)
                                             .Sum(card => card.GetCardPoint());

                        double suitScore = suitCount * 1.5 + highCardPoints;

                        int bid;
                        if (suitScore >= 25) bid = 8;
                        else if (suitScore >= 20 && suitScore < 25) bid = 7;
                        else if (suitScore >= 15 && suitScore < 20) bid = 6;
                        else if (suitScore >= 10 && suitScore < 15) bid = 5;
                        else bid = 0;

                        if (bid > bestBid)
                        {
                            bestBid = bid;
                        }                                                         
                    }

                    Bids.Add(players, bestBid);

                    if (bestBid > minimumBid)
                    {
                        minimumBid = bestBid;
                    }
                }                                
            }
        }

        public Suit SelectTrump() // equals to Trump property in GameLoop to choose trump
        {
            var highestBid = GetHighestBid();
            RoundStarter = highestBid.Key;

            Console.WriteLine($"{highestBid.Key.Name} has the highest bid with ({highestBid.Value}) and will select the trump.");

            //continue from asking specifically the player with highest bid either human or computer

            if (highestBid.Key.IsHuman)
            {
                bool isValidAnswer = false;

                while(!isValidAnswer)
                {
                    Console.WriteLine("Please select the trump suit (hearts, diamonds, clubs, spades)");
                    string playerDecision = Console.ReadLine();
                    playerDecision = playerDecision.Trim().ToLower();

                    Console.ForegroundColor = ConsoleColor.Red;
                    switch (playerDecision)
                    {
                        case "hearts": isValidAnswer = true; Console.WriteLine("Trump suit is hearts."); return Suit.Hearts; 
                        case "diamonds": isValidAnswer = true; Console.WriteLine("Trump suit is diamonds."); return Suit.Diamonds;
                        case "clubs": isValidAnswer = true; Console.WriteLine("Trump suit is clubs.");  return Suit.Clubs;
                        case "spades": isValidAnswer = true; Console.WriteLine("Trump suit is spades."); return Suit.Spades;
                        default: Console.WriteLine("Please select a valid suit."); break;
                    }
                    Console.ResetColor();
                }                        
            }

            else
            {
                var potentialTrump = (from card in highestBid.Key.Hand
                                      group card by card.Suit into suitGroup
                                      orderby suitGroup.Count() descending
                                      select suitGroup.Key).First();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Trump suit is {potentialTrump}\n");
                Console.ResetColor();
                return potentialTrump;                                    
            }            

            return Suit.Hearts;
        }

        public KeyValuePair<Players, int> GetHighestBid()
        {
            if (!Bids.Any())
            {
                throw new InvalidOperationException("No bids were placed.");
            }
            return Bids.OrderByDescending(bid => bid.Value).First();
        }

        public void PlayGameLoop(List<Players> players, CardVerifiers cardVerifier, GameVerifiers gameVerifier, ScoreBoard scoreBoard) //loop the game for 13 rounds, start from highest bidder, winner of the previous hand will start the next hand
        {
            string continueOrExit = null;

            while (continueOrExit != "no")
            {               
                this.Players = players;
                Bids.Clear();
                gameVerifier.TrumpUsed = false;
                
                foreach(Players player in players)
                {
                    player.WonHands = 0;
                }

                DeckBuilder();

                foreach (var player in players)
                {
                    CardDistributer(player);
                }

                Bidding();
                Trump = SelectTrump();

                for (int i = 0; i < Rounds; i++)
                {
                    CardsInTheMiddle.Clear();

                    int startingIndex = players.FindIndex(p => p == RoundStarter);


                    for (int j = 0; j < players.Count(); j++)
                    {
                        int currentIndex = (startingIndex + j) % players.Count();
                        Players currentPlayer = players[currentIndex];

                        MoveChecker moveChecker = new MoveChecker(currentPlayer, this, gameVerifier, cardVerifier);
                        UI ui = new UI();
                        ui.SubscribeToEvents(moveChecker);

                        if (currentPlayer.IsHuman)
                        {
                            currentPlayer.PlayerPlayCard(this, moveChecker);
                        }

                        else
                        {
                            currentPlayer.BotPlayCard(this, moveChecker);
                        }
                    }

                    CheckRoundWinner(players, gameVerifier);
                }

                gameVerifier.UpdateScores(this);       
                scoreBoard.DisplayScoreboard();

                Console.WriteLine("Do you want to player another game? (Yes / No)");
                string input = Console.ReadLine();
                input.Trim().ToLower();
                continueOrExit = input;
            }

        }
        public void CheckRoundWinner(List<Players> players, GameVerifiers gameVerifier)
        {
            if (CardsInTheMiddle.Count() == players.Count)
            {
                Suit leadSuit = CardsInTheMiddle.First().Value.Suit;

                var trumpCards = CardsInTheMiddle.Where(kvp => kvp.Value.Suit == Trump);

                if (trumpCards.Any())
                {
                    var winningEntry = trumpCards.OrderByDescending(kvp => kvp.Value.Rank).First();

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"{winningEntry.Key.Name} wins this hand with {winningEntry.Value.Rank} of {winningEntry.Value.Suit}");
                    Console.ResetColor();

                    winningEntry.Key.WonHands++;

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{winningEntry.Key.Name} won {winningEntry.Key.WonHands} hands.\n");
                    Console.ResetColor();

                    RoundStarter = winningEntry.Key;
                }
                else
                {
                    var winningEntry = CardsInTheMiddle
                                        .Where(kvp => kvp.Value.Suit == leadSuit)
                                        .OrderByDescending(kvp => kvp.Value.Rank)
                                        .First();

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"{winningEntry.Key.Name} wins this hand with {winningEntry.Value.Rank} of {winningEntry.Value.Suit}");
                    Console.ResetColor();

                    winningEntry.Key.WonHands++;

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{winningEntry.Key.Name} won {winningEntry.Key.WonHands} hands.\n");
                    Console.ResetColor();

                    RoundStarter = winningEntry.Key;
                }
            }
        }
    }
    public class GameVerifiers
    {
        public bool TrumpUsed { get; set; } = false;      
        public bool CanUseTrump(GameLoop gameLoop, Players player)
        {            
            bool hasMatchingSuit = player.Hand.Any(card => gameLoop.CardsInTheMiddle.Any(kvp => kvp.Value.Suit == card.Suit));
         
            return TrumpUsed || !hasMatchingSuit;
        }

       
        public void UpdateScores(GameLoop gameLoop)
        {
            if(gameLoop.Rounds == 13)
            {
                var highestBid = gameLoop.GetHighestBid();

                Players highestBidder = highestBid.Key;
                int wonBid = highestBid.Value;             
                  
                if (highestBidder.WonHands == wonBid || highestBidder.WonHands == wonBid + 1)
                {
                     Console.WriteLine($"{highestBidder.Name} wins the game with {highestBidder.WonHands} hands (bid was {highestBid.Value}).");
                     highestBidder.Score += highestBidder.WonHands;
                }

                else
                {
                     Console.WriteLine($"{highestBidder.Name} loses with {highestBidder.WonHands} hands (bid was {highestBid.Value}).");
                     highestBidder.Score -= wonBid;
                }
                   
                foreach(var player in gameLoop.Bids)
                {
                    if(player.Key != highestBidder)
                    {
                        player.Key.Score += player.Key.WonHands;
                        Console.WriteLine($"{player.Key.Name} won {player.Key.WonHands} hands.");
                    }
                }                                                              
            }            
        }
    }

    public class MoveChecker
    {
        public Players Player { get; set; }
        public GameLoop GameLoop { get; set; }
        public GameVerifiers GameVerifiers { get; set; }
        public CardVerifiers CardVerifiers { get; set; }
        public event EventHandler<SuitEventArgs> SuitEvent;
        public event EventHandler<TrumpSuitArgs> TrumpEvent;

        public MoveChecker(Players player,GameLoop gameLoop, GameVerifiers gameVerifiers, CardVerifiers cardVerifiers)
        {
            Player = player;
            GameLoop = gameLoop;
            GameVerifiers = gameVerifiers;
            CardVerifiers = cardVerifiers;
        }

        public bool IsValidMove(Card selectedCard)
        {

            if (GameLoop.CardsInTheMiddle.Count == 0)
            {
                if (!GameVerifiers.TrumpUsed)
                {
                    if (selectedCard.Suit == GameLoop.Trump)
                    {
                        if(Player.IsHuman)
                        {
                            Console.WriteLine("Trump suit cannot be used unless it had to be capitalized.");
                        }
                        return false;
                    }

                    else
                    {
                        SuitEvent?.Invoke(this, new SuitEventArgs
                        {
                            SuitOfCardsInTheMiddle = selectedCard.Suit,
                            IsSpecialDisplay = true
                        });
                        return true;
                    }
                }

                else return true;
            }

            else
            {
                
                var mainSuit = GameLoop.CardsInTheMiddle.First().Value.Suit;

                if (CardVerifiers.HasSameSuit(Player, GameLoop))
                {

                    if (selectedCard.Suit != mainSuit)
                    {
                        if (Player.IsHuman)
                        {
                            Console.WriteLine("You must follow the suit.");
                        }
                        return false;
                    }

                    if (CardVerifiers.HasBiggerCard(Player, GameLoop))
                    {
                        var highestMiddleRank = GameLoop.CardsInTheMiddle
                                                .Where(kvp => kvp.Value.Suit == mainSuit)
                                                .Max(kvp => kvp.Value.Rank);

                        if (selectedCard.Rank <= highestMiddleRank)
                        {
                            if (Player.IsHuman)
                            {
                                Console.WriteLine("You must play a bigger card than the one in the middle.");
                            }
                            return false;
                        }
                    }
                    return true;
                }

                else
                {
                    if (Player.Hand.Any(c => c.Suit == GameLoop.Trump))
                    {
                        if(GameLoop.CardsInTheMiddle.Any(kvp => kvp.Value.Suit == GameLoop.Trump) && selectedCard.Suit == GameLoop.Trump)
                        {
                            var highestMiddleTrumpRank = GameLoop.CardsInTheMiddle
                                                         .Where(kvp => kvp.Value.Suit == GameLoop.Trump)
                                                         .Max(kvp => kvp.Value.Rank);

                            var highestHandTrumpRank = Player.Hand
                                                       .Where(c => c.Suit == GameLoop.Trump)
                                                       .Max(c => c.Rank);

                            if(highestHandTrumpRank > highestMiddleTrumpRank)
                            {
                                if (selectedCard.Rank <= highestMiddleTrumpRank)
                                {
                                    if (Player.IsHuman)
                                    {
                                        Console.WriteLine("You must play a card with a higher rank.");
                                    }
                                    return false;
                                }

                                TrumpEvent?.Invoke(this, new TrumpSuitArgs
                                    {
                                      TrumpSuit = GameLoop.Trump,
                                      IsSpecialDisplay = true,
                                    });
                                GameVerifiers.TrumpUsed = true;
                                return true;
                            }

                            else
                            {
                                TrumpEvent?.Invoke(this, new TrumpSuitArgs
                                {
                                    TrumpSuit = GameLoop.Trump,
                                    IsSpecialDisplay = true,
                                });
                                GameVerifiers.TrumpUsed = true;
                                return true;
                            }                             
                        }

                        else
                        {
                            if (selectedCard.Suit != GameLoop.Trump)
                            {
                                if (Player.IsHuman)
                                {
                                    Console.WriteLine("You must play a trump card if have one.");
                                }
                                return false;
                            }
                            GameVerifiers.TrumpUsed = true;
                            return true;
                        }
                    }

                    return true;
                }
            }
        }                                          
        
    }

    public class SuitEventArgs : EventArgs // add an event for displaying the suit of the cards in the middle from UI class
    {
        public Suit SuitOfCardsInTheMiddle { get; set; }
        public bool IsSpecialDisplay { get; set; }
    }

    public class TrumpSuitArgs : EventArgs // add an event for displaying the suit and making a noise once trump suit is used
    {
        public Suit TrumpSuit { get; set; }
        public bool IsSpecialDisplay { get; set; }
    }
}

