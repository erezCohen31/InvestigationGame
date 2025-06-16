using InvestigationGame;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Investigation Game - Track the Iranian Agent";

        GameManager gameManager = new GameManager();
        gameManager.Menu();
    }
}
