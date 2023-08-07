using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.LevelGen
{
    public class WallGenerator
    {
        public static void CreateWalls(HashSet<Vector2> floorPositions)
        {
            var wallPositions = FindWallsInDirections(floorPositions, Direction2D.eightDirectionsList);
            CreateCornerWalls(wallPositions, floorPositions);
        }

        //  Searching for Diagonal Walls
        private static void CreateCornerWalls(HashSet<Vector2> cornerWallPositions, HashSet<Vector2> floorPositions)
        {
            foreach (var position in cornerWallPositions)
            {
                FastList<int> neighboursBinaryType = new FastList<int>();
                foreach (var direction in Direction2D.eightDirectionsList)
                {
                    var neighbourPosition = position + direction;
                    if (floorPositions.Contains(neighbourPosition))
                    {
                        neighboursBinaryType.Add(0);
                    }
                    else
                    {
                        neighboursBinaryType.Add(1);
                    }
                }

                Levels.PaintWall(position, neighboursBinaryType);
            }
        }

        private static HashSet<Vector2> FindWallsInDirections(HashSet<Vector2> floorPositions, List<Vector2> directionList)
        {
            HashSet<Vector2> wallPositions = new HashSet<Vector2>();
            foreach (var position in floorPositions)
            {
                foreach (var direction in directionList)
                {
                    var neighbourPosition = position + direction;
                    if (floorPositions.Contains(neighbourPosition) == false)
                        wallPositions.Add(neighbourPosition);
                }
            }
            return wallPositions;
        }
    }

    public static class Direction2D
    {
        public static List<Vector2> eightDirectionsList = new List<Vector2>
        {
            new Vector2(-1, -1), //LEFT-UP
            new Vector2( 0, -1), //UP
            new Vector2( 1, -1), //UP-RIGHT
            new Vector2(-1,  0), //LEFT
            new Vector2( 0,  0), // Mid
            new Vector2( 1,  0), //RIGHT
            new Vector2(-1,  1), // DOWN-LEFT
            new Vector2( 0,  1), // DOWN
            new Vector2( 1,  1), //RIGHT-DOWN

        };
    }

    public static class WallRules
    {
        public static int[] Top = new int[]
        {
            0, -1, 0,
            0,  1, 0,
            0,  0, 0
        };

        public static int[] Bottom = new int[]
        {
            0,  0, 0,
            0,  1, 0,
            0, -1, 0
        };

        public static int[] Left = new int[]
        {
            0,  0, 0,
            -1,  1, 0,
            0, 0, 0
        };

        public static int[] Right = new int[]
        {
            0,  0, 0,
            0,  1, -1,
            0,  0, 0
        };

        public static int[] TopLeft = new int[]
        {
             0, -1, 0,
            -1,  1, 0,
             0,  0, 0
        };

        public static int[] TopRight = new int[]
        {
             0, -1, 0,
             0,  1,-1,
             0,  0, 0
        };

        public static int[] BottomRight = new int[]
        {
             0, 0, 0,
             0,  1, -1,
             0,  -1, 0
        };

        public static int[] BottomLeft = new int[]
        {
             0, 0, 0,
             -1,  1, 0,
             0,  -1, 0
        };

        public static int[] BottomLeftCorner = new int[]
        {
             0, 1, -1,
             0,  1, 1,
             0,  0, 0
        };

        public static int[] BottomRightCorner = new int[]
        {
             -1 , 1, 0,
             1,  1, 0,
             0,  0, 0
        };

        public static int[] TopLeftCorner = new int[]
        {
             0, 0, 0,
             0,  1, 1,
             0,  1, -1
        };

        public static int[] TopRightCorner = new int[]
        {
             0, 0, 0,
             1,  1, 0,
             -1,  1, 0
        };
    }
}
