using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TinyTeenyRougelike
{
    class Entities
    {
    }
    public class Creature : Tile
    {
        public int Hits { get; set; }
        public void Die()
        {
            Hits = 0;
        }
    }

    public class Player : Creature
    {
        public Player(Point p)
        {
            ImageCharacter = Constants.PlayerImage;
            Color = Constants.PlayerColor;
            Inventory = new List<Sword>();
            X = p.X;
            Y = p.Y;
            Hits = Constants.StartingHitPoints;
        }

        public List<Sword> Inventory { get; set; }
    }

    public class Monster : Creature
    {
        public bool monsterTurn;

        public Monster(Point p)
        {
            if (Dungeon.r.Next(1 + 1)==1){
                monsterTurn = false;
            }
            else
            {
                monsterTurn = true;
            }

            ImageCharacter = Constants.MonsterImage;
            Color = Constants.MonsterColor;
            X = p.X;
            Y = p.Y;
            Hits = Constants.StartingHitPoints;
        }
    }
}
