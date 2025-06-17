using InvestigationGame;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Investigation Game - Track the Iranian Agent";

        try
        {
            GameManager gameManager = new GameManager();
            gameManager.Menu();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Une erreur inattendue est survenue : {ex.Message}");
        }
    }
}
