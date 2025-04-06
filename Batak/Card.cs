using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Batak
{
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public Suit GetSuit()
        {
            return Suit;
        }

        public int GetCardPoint()
        {
            switch(Rank)
            {
                case Rank.Ace: return 4;
                case Rank.King: return 3;
                case Rank.Queen: return 2;
                case Rank.Jack: return 1;
                default: return 0;
            }
        }
    }

    public class CardVerifiers
    {
        public bool HasSameSuit(Players players, GameLoop gameLoop)
        {
            var firstCard = gameLoop.CardsInTheMiddle.FirstOrDefault();
            if (firstCard.Equals(default(KeyValuePair<Players, Card>))) 
                return false;

            Suit mainSuitToFollow = firstCard.Value.Suit;

            return players.Hand.Any(card => card.Suit == mainSuitToFollow);
        }
        public bool HasBiggerCard(Players player, GameLoop gameLoop)
        {
            if (!HasSameSuit(player, gameLoop))
                return false;

            var firstPair = gameLoop.CardsInTheMiddle.FirstOrDefault();
            if (firstPair.Equals(default(KeyValuePair<Players, Card>)))
                return false;

            Suit mainSuitToFollow = firstPair.Value.Suit;

            List<Card> allCardsWithSuitToFollow = new List<Card>();

            foreach(Card card in gameLoop.CardsInTheMiddle.Select(kvp => kvp.Value))
            {
                if(card.Suit == mainSuitToFollow)
                    allCardsWithSuitToFollow.Add(card);
            }

            var suitsInHandThatMatches = from card in player.Hand
                                         where card.Suit == mainSuitToFollow
                                         select card
                                         into biggerCards
                                         where biggerCards.Rank > allCardsWithSuitToFollow.Max(x => x.Rank)
                                         select biggerCards;

            return suitsInHandThatMatches.Any();               
        }
    }
}
