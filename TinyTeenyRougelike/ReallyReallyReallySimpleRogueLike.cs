using System;

namespace TinyTeenyRougelike
{
    class ReallyReallyReallySimpleRogueLike
    {
        static void Main(string[] args)
        {
            Dungeon dungeon = new Dungeon(Constants.DungeonWidth, Constants.DungeonHeight);
            string displayText = Constants.IntroductionText;
            while (dungeon.IsGameActive) // or true to test. there aint no monsters to resolve this so it ends.
            {
                dungeon.DrawToConsole();
                Console.WriteLine(displayText);
                Console.Write(Constants.CursorImage);
                displayText = dungeon.ExecuteCommand(Console.ReadKey());
            }

            Console.WriteLine(ConcludeGame(dungeon));
            Console.ReadLine();
        }

        private static string ConcludeGame(Dungeon dungeon)
        {
            return (dungeon.player.Hits > 0) ? Constants.PlayerWinsText : Constants.MonsterWinsText;
        }
    }
}

