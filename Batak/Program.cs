namespace Batak
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameLoop gameLoop = new GameLoop();

            List<Players> players = new List<Players>();

            Players humanPlayer = new Players(true);
            Players botPlayer1 = new Players(false);
            Players botPlayer2 = new Players(false);
            Players botPlayer3 = new Players(false);

            GameVerifiers gameVerifiers = new GameVerifiers();
            CardVerifiers cardVerifiers = new CardVerifiers();
            players.Add(humanPlayer);
            players.Add(botPlayer1);
            players.Add(botPlayer2);
            players.Add(botPlayer3);

            humanPlayer.PlayerNaming();
            botPlayer1.PlayerNaming();
            botPlayer2.PlayerNaming();
            botPlayer3.PlayerNaming();
            
            gameLoop.PlayGameLoop(players, cardVerifiers, gameVerifiers);
        }
    }
}
