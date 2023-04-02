using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Digger
{
    public class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            int deltaX = 0;
            int deltaY = 0;
            return new CreatureCommand(deltaX, deltaY, null);
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return true;
        }

        public int GetDrawingPriority()
        {
            return 2;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }

    public class Player : ICreature
    {
        public static int playerX = 0;
        public static int playerY = 0;
        public Player()
        {
            for (int x = 0; x < Game.MapWidth; x++)
                for (int y = 0; y < Game.MapHeight; y++)
                {
                    if (Game.Map[x,y] is Player)
                    {
                        playerX = x;
                        playerY = y;
                    }
                }
        }

        public CreatureCommand Act(int x, int y)
        {
            int deltaX = 0;
            int deltaY = 0;
            if (Game.KeyPressed == Keys.Down) deltaY = 1;
            else if (Game.KeyPressed == Keys.Up) deltaY = -1;
            else if (Game.KeyPressed == Keys.Left) deltaX = -1;
            else if (Game.KeyPressed == Keys.Right) deltaX = 1;
            if (x + deltaX >= Game.MapWidth || x + deltaX < 0) deltaX = 0;
            if (y + deltaY >= Game.MapHeight || y + deltaY < 0) deltaY = 0;
            var nextObject = Game.Map[x + deltaX, y + deltaY];
            if (nextObject is Monster || nextObject is Sack)
            {
                deltaX = 0;
                deltaY = 0;
            }
            playerX += deltaX;
            playerY += deltaY;
            return new CreatureCommand(deltaX, deltaY, null);
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Digger.png";
        }
    }

    public class Sack : ICreature
    {
        bool isFalling = false;
        bool isLongFalling = false;
        public CreatureCommand Act(int x, int y)
        {
            if (y + 1 >= Game.Map.GetLength(1))
            {
                if (isLongFalling) return new CreatureCommand(0, 0, new Gold());
                return new CreatureCommand(0, 0, null);
            }
            var nextObject = Game.Map[x, y + 1];
            int deltaY = 0;
            if ((nextObject is Player || nextObject is Monster) && isFalling || nextObject == null)
            {
                if (isFalling) isLongFalling = true;
                isFalling = true;
                deltaY = 1;
            }
            else
            {
                if (isLongFalling) return new CreatureCommand(0, deltaY, new Gold());
                isFalling = false;
                isLongFalling = false;
            }
            return new CreatureCommand(0, deltaY, null);
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority()
        {
            return 3;
        }

        public string GetImageFileName()
        {
            return "Sack.png";
        }
    }

    public class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand(0, 0, null);
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Player) Game.Scores += 10;
            return true;
        }

        public int GetDrawingPriority()
        {
            return 2;
        }

        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }

    public class Monster : ICreature
    {
        int monsterX = 0;
        int monsterY = 0;
        
        public Monster()
        {
            for (int x = 0; x < Game.MapWidth; x++)
                for (int y = 0; y < Game.MapHeight; y++)
                {
                    if (Game.Map[x, y] is Player)
                    {
                        monsterX = x;
                        monsterY = y;
                    }
                }
        }
        public CreatureCommand Act(int x, int y)
        {
            if (Game.IsOver) return new CreatureCommand(0, 0, null);
            int deltaX = 0;
            int deltaY = 0;
            int dx = x - Player.playerX;
            int dy = y - Player.playerY;
            if (dx > 0) deltaX = -1;
            else if (dx < 0) deltaX = 1;
            if (dy > 0) deltaY = -1;
            else if (dy < 0) deltaY = 1;
            if (x + deltaX >= Game.Map.GetLength(0) || x + deltaX < 0) deltaX = 0;
            if (y + deltaY >= Game.Map.GetLength(1) || y + deltaY < 0) deltaY = 0;
            var nextObjectX = Game.Map[x + deltaX, y];
            if (nextObjectX is Terrain || nextObjectX is Sack || nextObjectX is Monster)
                deltaX = 0;
            var nextObjectY = Game.Map[x, y + deltaY];
            if (nextObjectY is Terrain || nextObjectY is Sack || nextObjectX is Monster)
                deltaY = 0;
            if (deltaY != 0 && deltaX != 0)
            {
                Random rnd = new Random();
                if (rnd.Next(2) == 1) deltaX = 0;
                else deltaY = 0;
            }
            return new CreatureCommand(deltaX, deltaY, null);
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Monster.png";
        }
    }
}
