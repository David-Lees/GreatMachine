using System;

namespace GreatMachine
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new GreatMachine();
            game.Run();
        }
    }
}
