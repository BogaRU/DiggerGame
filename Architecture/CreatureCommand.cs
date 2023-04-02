namespace Digger
{
    public class CreatureCommand
    {
        public int DeltaX;
        public int DeltaY;
        public ICreature TransformTo;
        public CreatureCommand (int x, int y, ICreature TransformTo)
        {
            DeltaX = x;
            DeltaY = y;
            this.TransformTo = TransformTo;
        }
    }
}