using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;

namespace NezTopDown.Components.LevelGen
{
	// Code By Six Dot: https://bit.ly/2z6f1v6
	public class LevelGenerator : Component
    {
        #region Data Struct
        public enum gridSpace { empty, floor, wall };
        struct walker
        {
            public Vector2 dir;
            public Vector2 pos;
        }
        #endregion

        #region Vars
        public Dictionary<Vector2, gridSpace> grid; // Vector 2D
		HashSet<Vector2> floors = new HashSet<Vector2>();

		public static int RoomHeight, RoomWidth;
		Vector2 roomSizeWorldUnits = new Vector2(30, 30); // Use to calculate the size of the grid
		float worldUnitsInOneGridCell = 1;

		List<walker> walkers;

		float chanceWalkerChangeDir = 0.5f, chanceWalkerSpawn = 0.05f;
		float chanceWalkerDestoy = 0.05f;
		
		int chestCount = 0, minChestCount = 2, maxChestCount = 8;
		float chanceSpawnChest = 0.3f;

		float chanceSpawnEnemy = 0.4f;
        public static int enemyCount { get; set; } = 0;
		int maxEnemyCount = 8;

		int maxWalkers = 10;
		public float Scale = 3f; // Scale the Level
		float percentToFill = 0.2f;

        #endregion

        public override void Initialize()
        {
			Levels.SetupDungeon(this);
			//var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Tiles/Garden.atlas");
			//for (int i = 1; i <= 9; i++)
			//	floorSprite[i - 1] = atlas.GetSprite("Tile" + i.ToString());
			//wallSprite[0] = atlas.GetSprite("LeftDirt");
			//wallSprite[1] = atlas.GetSprite("TopDirt");
			//wallSprite[2] = atlas.GetSprite("RightDirt");
			//wallSprite[3] = atlas.GetSprite("BottomDirt");
			base.Initialize();
        }

        public override void OnAddedToEntity()
		{
			Entity.Transform.Position = Vector2.Zero;
			Setup();
			CreateFloors();
			RemoveSingleWalls();
			//for (int i = 0; i < roomSizeWorldUnits.Y; i++)
			//{
			//	for (int j = 0; j < roomSizeWorldUnits.X; j++)
			//		Console.Write("{0}", (int)grid[new Vector2(j, i)]);
			//	Console.WriteLine();
			//}
			WallGenerator.CreateWalls(floors);
			Levels.SpawnAllFloorTiles(floors);
		}

        #region Generating Methods
        void Setup()
		{
			//find grid size
			RoomHeight = Mathf.RoundToInt(roomSizeWorldUnits.X / worldUnitsInOneGridCell);
			RoomWidth = Mathf.RoundToInt(roomSizeWorldUnits.Y / worldUnitsInOneGridCell);
			//create grid
			grid = new Dictionary<Vector2, gridSpace>();
			//set grid's default state
			for (int x = 0; x < RoomWidth - 1; x++)
			{
				for (int y = 0; y < RoomHeight - 1; y++)
				{
					//make every cell "empty"
					grid[new Vector2(x, y)] = gridSpace.empty;
				}
			}
			//set first walker
			//init list
			walkers = new List<walker>();
			//create a walker 
			walker newWalker = new walker();
			newWalker.dir = RandomDirection();
			//find center of grid
			Vector2 spawnPos = new Vector2(Mathf.RoundToInt(RoomWidth / 2.0f),
											Mathf.RoundToInt(RoomHeight / 2.0f));
			floors.Add(spawnPos);

			Entity.Scene.FindEntity("player").Transform.Position = Entity.Transform.Position + spawnPos * 16 * Scale;

			newWalker.pos = spawnPos;

			//add walker to list
			walkers.Add(newWalker);
		}

		void CreateFloors()
		{
			int iterations = 0;//loop will not run forever
			do
			{
				//create floor at position of every walker
				foreach (walker myWalker in walkers)
				{
					grid[myWalker.pos] = gridSpace.floor;
					floors.Add(myWalker.pos);
                    if (Nez.Random.Chance(chanceSpawnEnemy) && enemyCount < maxEnemyCount)
                    {
                        Entity.Scene.CreateEntity("enemy", myWalker.pos).AddComponent(new Enemy());
                        enemyCount++;
                    }
                }
				//chance: destroy walker
				int numberChecks = walkers.Count; //might modify count while in this loop
				for (int i = 0; i < numberChecks; i++)
				{
					//only if its not the only one, and at a low chance
					if (Nez.Random.Chance(chanceWalkerDestoy) && walkers.Count > 1)
					{
						Vector2 pos = Entity.Transform.Position + walkers[i].pos * 16 * Scale;

						//Spawn Chest
						
						if ((Nez.Random.Chance(chanceSpawnChest) || chestCount <= minChestCount) && chestCount <= maxChestCount)
						{
							Entity.Scene.CreateEntity("chest", pos).AddComponent(new Chest());
							chestCount++;
						}

						walkers.RemoveAt(i);
						break; //only destroy one per iteration
					}
				}
				//chance: walker pick new direction
				for (int i = 0; i < walkers.Count; i++)
				{
					if (Nez.Random.Chance(chanceWalkerChangeDir))
					{
						walker thisWalker = walkers[i];
						thisWalker.dir = RandomDirection();

						walkers[i] = thisWalker;
					}
				}
				//chance: spawn new walker
				numberChecks = walkers.Count; //might modify count while in this loop
				for (int i = 0; i < numberChecks; i++)
				{
					//only if # of walkers < max, and at a low chance
					if (Nez.Random.Chance(chanceWalkerSpawn) && walkers.Count < maxWalkers)
					{
						//create a walker 
						walker newWalker = new walker();
						newWalker.dir = RandomDirection();
						newWalker.pos = walkers[i].pos;
						walkers.Add(newWalker);
					}
				}
				//move walkers
				for (int i = 0; i < walkers.Count; i++)
				{
					walker thisWalker = walkers[i];
					thisWalker.pos += thisWalker.dir;
					walkers[i] = thisWalker;
				}
				//avoid boarder of grid
				for (int i = 0; i < walkers.Count; i++)
				{
					walker thisWalker = walkers[i];
					//clamp x,y to leave a 1 space boarder: leave room for walls
					thisWalker.pos.X = Mathf.Clamp(thisWalker.pos.X, 1, RoomWidth - 2);
					thisWalker.pos.Y = Mathf.Clamp(thisWalker.pos.Y, 1, RoomHeight - 2);
					walkers[i] = thisWalker;
				}
				//check to exit loop
				if ((float)NumberOfFloors() / (float)grid.Count > percentToFill)
				{
					break;
				}
				iterations++;
			} while (iterations < 100000);
		}
		void RemoveSingleWalls()
		{
			//loop though every grid space
			for (int x = 0; x < RoomWidth - 1; x++)
			{
				for (int y = 0; y < RoomHeight - 1; y++)
				{
					//if theres a wall, check the spaces around it
					if (grid[new Vector2(x, y)] == gridSpace.wall)
					{
						//assume all space around wall are floors
						bool allFloors = true;
						//check each side to see if they are all floors
						for (int checkX = -1; checkX <= 1; checkX++)
						{
							for (int checkY = -1; checkY <= 1; checkY++)
							{
								if (x + checkX < 0 || x + checkX > RoomWidth - 1 ||
									y + checkY < 0 || y + checkY > RoomHeight - 1)
								{
									//skip checks that are out of range
									continue;
								}
								if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
								{
									//skip corners and center
									continue;
								}
								if (grid[new Vector2(x + checkX, y + checkY)] != gridSpace.floor)
								{
									allFloors = false;
								}
							}
						}
						if (allFloors)
						{
							grid[new Vector2(x, y)] = gridSpace.floor;
						}
					}
				}
			}
		}

		Vector2 RandomDirection()
		{
			//pick random int between 0 and 3
			int choice = Mathf.FloorToInt(Nez.Random.NextFloat() * 3.99f);
			//use that int to chose a direction
			switch (choice)
			{
				case 0:
					return new Vector2(0, -1);
				case 1:
					return new Vector2(-1, 0);
				case 2:
					return new Vector2(0, 1);
				default:
					return new Vector2(1, 0);
			}
		}

		int NumberOfFloors()
		{
			int count = 0;
			foreach (var space in grid)
			{
				if (space.Value == gridSpace.floor)
				{
					count++;
				}
			}
			return count;
		}

        #endregion

	}
}
