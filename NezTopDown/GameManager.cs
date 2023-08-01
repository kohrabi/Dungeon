// This is where most of the global stuff stay

using System;
using Nez;

namespace NezTopDown
{
    public class LayerDepths
    {
        public enum Sorting
        {
            Projectiles,
            Weapons,
            WallBarrier,
            Player,
            Enemy,
            Chest,
            Tile,
        }

        public static float GetLayerDepth(Sorting sorting)
        {
            return (int)sorting / 100f;
        }
    }

    public enum PhysicsLayers
    {
        Player = 1,
        Tile,
        Enemy
    }

    public enum EntityState
    {
        Free,
        Hit,
        Dead
    }

    public class GameManager
    {

    }
}
