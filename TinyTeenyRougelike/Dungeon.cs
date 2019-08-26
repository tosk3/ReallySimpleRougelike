using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TinyTeenyRougelike
{
    class Dungeon
    {
        public static Random r = new Random();
        public Player player;
        List<Monster> monsters;
        List<Sword> swords;
        List<Wall> walls;
        public Tile[,] Tiles;
        private int xMax;
        private int yMax;
        List<Point> availablePoints;


        public enum Direction
        {
            North,
            South,
            East,
            West
        }

        public bool IsGameActive
        {

            get
            {
                return (player.Hits > 0 && monsters.Any(m => m.Hits > 0));
            }
        }

        public Dungeon(int xMax, int yMax)
        {
            availablePoints = new List<Point>();
            monsters = new List<Monster>();
            walls = new List<Wall>();
            swords = new List<Sword>();
            this.xMax = xMax;
            this.yMax = yMax;
            Tiles = new Tile[xMax, yMax];

            for (int i = 1; i < xMax-2; i++)
            {
                for (int j = 1; j < yMax-2; j++)
                {
                    Point point = new Point(i, j);
                    availablePoints.Add(point);
                }
            }
            BuildRandomDungeon();
            SetDungeonTiles();
        }

        public string ExecuteCommand(ConsoleKeyInfo command)
        {
            string commandResult = ProcessCommand(command);
            ProcessMonsters();
            SetDungeonTiles();

            return commandResult;
        }

        private void ProcessMonsters()
        {
            if (monsters != null && monsters.Count > 0)
            {
                monsters.Where(m => m.Hits >= 0).ToList().ForEach(m =>
                {
                    MoveMonsterToPlayer(m);
                });
            }
        }

        private void MoveMonsterToPlayer(Monster m)
        {
            if (m.X == player.X && m.Y == player.Y)
            {
                ResolveCombat(m);
            }
            else
            {
                if (!m.monsterTurn)
                {
                    m.monsterTurn = true;
                }
                else
                {
                    m.monsterTurn = false;
                    if (Math.Abs(m.X - player.X) > Math.Abs(m.Y - player.Y))
                    {
                        if (m.X - player.X > 0)
                        {
                            m.X--;
                        }
                        else
                        {
                            m.X++;
                        }
                    }
                    else
                    {
                        if (m.Y - player.Y > 0)
                        {
                            m.Y--;
                        }
                        else
                        {
                            m.Y++;
                        }
                    }
                }

            }
        }

        private void BuildRandomDungeon()
        {

            for (int i = 0; i < xMax; i++)
            {
                Wall top = new Wall(i, 0);
                walls.Add(top);
                Wall bottom = new Wall(i, yMax - 1);
                walls.Add(bottom);
            }

            for (int i = 0; i < yMax; i++)
            {
                Wall left = new Wall(0, i);
                walls.Add(left);
                Wall right = new Wall(xMax - 1, i);
                walls.Add(right);
            }
            //spawns swords randomly

            for (int i = 0; i < Constants.NumberOfSwords; i++)
            {
                if (r.Next(1 + 1) == 1) // 50 : 50 chance of swords
                {
                    Sword s = new Sword(GetValidRandomPoint());
                    swords.Add(s);
                }
                if (swords.Count <= 0) // incase none spawn
                {
                    Sword s = new Sword(GetValidRandomPoint());
                    swords.Add(s);
                }

            }

            // creates a new player in Random point

            player = new Player(GetValidRandomPoint());

            //creates monsters based on the const value;

            for (int i = 0; i < Constants.NumberOfMonsters; i++)
            {
                Monster m = new Monster(GetValidRandomPoint());
                monsters.Add(m);
            }

            //sets the dungeon

            SetAllDungeonSquaresToTiles();
        }

        private Point GetValidRandomPoint() // takes a point from the list and returns it while also removing it from the list not allowing it to be used again.
        { 
            int rndIndex = r.Next(0, availablePoints.Count);
            Point point = availablePoints[rndIndex];
            availablePoints.RemoveAt(rndIndex);
            return point;
        }

        private void ResolveCombat(Monster monster)
        {
            if (player.Inventory.Any())
                monster.Die();
            else
                player.Die();
        }

        public string ProcessCommand(ConsoleKeyInfo command)
        {

            string output = string.Empty;
            switch (command.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.RightArrow:
                case ConsoleKey.LeftArrow:
                    output = GetNewLocation(command, new Point(player.X, player.Y));
                    break;
                case ConsoleKey.F1:
                    output = Constants.NoHelpText;
                    break;
            }

            return output;
        }

        private string GetNewLocation(ConsoleKeyInfo command, Point move)
        {
            switch (command.Key)
            {
                case ConsoleKey.UpArrow:
                    move.Y -= 1;
                    break;
                case ConsoleKey.DownArrow:
                    move.Y += 1;
                    break;
                case ConsoleKey.RightArrow:
                    move.X += 1;
                    break;
                case ConsoleKey.LeftArrow:
                    move.X -= 1;
                    break;
            }

            if (!IsInvalidValidMove(move.X, move.Y))
            {
                player.X = move.X;
                player.Y = move.Y;
                if (Tiles[move.X, move.Y] is Sword && player.Inventory.Count == 0)
                {
                    Sword sword = (Sword)Tiles[move.X, move.Y];
                    player.Inventory.Add(sword);
                    swords.Remove(sword);
                }
                return Constants.OKCommandText;
            }
            else
                return Constants.InvalidMoveText;
        }

        public bool IsInvalidValidMove(int x, int y)
        {
            return (x == 0 || x == Constants.DungeonWidth - 1 || y == Constants.DungeonHeight - 1 || y == 0);
        }

        public void SetDungeonTiles()
        {
            //Draw the empty dungeon
            SetAllDungeonSquaresToTiles();

            SetAllDungeonObjectsToTiles();
        }

        private void SetAllDungeonObjectsToTiles()
        {
            //Now draw each of the parts of the dungeon
            walls.ForEach(w => Tiles[w.X, w.Y] = w);
            swords.ForEach(s => Tiles[s.X, s.Y] = s);
            monsters.ForEach(m => Tiles[m.X, m.Y] = m);
            Tiles[player.X, player.Y] = player;
        }

        private void SetAllDungeonSquaresToTiles()
        {

            for (int i = 0; i < yMax; i++)
            {
                for (int j = 0; j < xMax; j++)
                {
                    Tiles[j, i] = new Tile(i, j);
                }
            }
        }

        public void DrawToConsole()
        {
            Console.Clear();
            for (int i = 0; i < yMax; i++)
            {
                for (int j = 0; j < xMax; j++)
                {
                    Console.ForegroundColor = Tiles[j, i].Color;
                    Console.Write(Tiles[j, i].ImageCharacter);
                }
                Console.WriteLine();
            }
        }
    }
    public class Tile
    {
        public string name { get; set; }
        public string ImageCharacter { get; set; }
        public ConsoleColor Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Tile() { }

        public Tile(int x, int y)
            : base()
        {
            this.X = x;
            this.Y = y;
            ImageCharacter = Constants.TileImage;
            Color = Constants.TileColor;
        }
    }

    public class Wall : Tile
    {
        public Wall(int x, int y)
            : base(x, y)
        {
            ImageCharacter = Constants.WallImage;
            this.Color = Constants.WallColor;
        }
    }

    public class Sword : Tile
    {
        public Sword(Point p)
        {
            ImageCharacter = Constants.SwordImage;
            this.Color = Constants.SwordColor;
            X = p.X;
            Y = p.Y;
        }
    }
}

