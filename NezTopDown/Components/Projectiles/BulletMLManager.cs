using BulletMLLib;
using Equationator;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.Projectiles
{
    // From BulletMLSample
    public class BulletMLManager : BulletMLLib.IBulletManager
    {
        public Transform PlayerTransform;

        public List<BulletMover> movers = new List<BulletMover>();

        public List<BulletMover> topLevelMovers = new List<BulletMover>();

        private float _timeSpeed = 1.0f;
        private float _scale = 1.0f;

        /// <summary>
		/// How fast time moves in this game.
		/// Can be used to do slowdown, speedup, etc.
		/// </summary>
		/// <value>The time speed.</value>
        public float TimeSpeed
        {
            get
            {
                return _timeSpeed;
            }
            set
            {
                //set my time speed
                _timeSpeed = value;

                //set all the bullet time speeds
                foreach (BulletMover myDude in movers)
                {
                    myDude.TimeSpeed = _timeSpeed;
                }
            }
        }

        /// <summary>
        /// Change the size of this bulletml script
        /// If you want to reuse a script for a game but the size is wrong, this can be used to resize it
        /// </summary>
        /// <value>The scale.</value>
        public float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                //set my scale
                _scale = value;

                //set all the bullet scales
                foreach (BulletMover myDude in movers)
                {
                    myDude.Scale = _scale;
                }
            }
        }

        public System.Random Rand { get; private set; } = new System.Random();

        public Dictionary<string, FunctionDelegate> CallbackFunctions { get; set; } = new Dictionary<string, FunctionDelegate>();

        public double Difficulty { get; set; }

        public FunctionDelegate GameDifficulty => () => Difficulty;

        public BulletMLManager(Transform playerTransform)
        {
            PlayerTransform = playerTransform;
        }

        public IBullet CreateBullet()
        {
            //create the new bullet
            BulletMover bullet = new BulletMover(this);

            //set the speed and scale of the bullet
            bullet.TimeSpeed = TimeSpeed;
            bullet.Scale = Scale;

            //initialize, store in our list, and return the bullet
            bullet.Init();
            movers.Add(bullet);
            return bullet;
        }

        public IBullet CreateTopBullet()
        {
            //create the new bullet
            BulletMover mover = new BulletMover(this);

            //set the speed and scale of the bullet
            mover.TimeSpeed = TimeSpeed;
            mover.Scale = Scale;

            //initialize, store in our list, and return the bullet
            mover.Init();
            topLevelMovers.Add(mover);
            return mover;
        }

        public Vector2 PlayerPosition(IBullet targettedBullet)
        {
            Debug.ErrorIf(PlayerTransform == null, "BulletML: PlayerTransform is null");
            return PlayerTransform.Position;
        }

        public void RemoveBullet(IBullet deadBullet)
        {
            BulletMover myMover = deadBullet as BulletMover;
            if (myMover != null)
            {
                myMover.Used = false;
            }
        }

        public void Update()
        {
            for (int i = 0; i < movers.Count; i++)
            {
                movers[i].Update();
            }

            for (int i = 0; i < topLevelMovers.Count; i++)
            {
                topLevelMovers[i].Update();
            }

            FreeMovers();
        }

        public void FreeMovers()
        {
            for (int i = 0; i < movers.Count; i++)
            {
                if (!movers[i].Used)
                {
                    movers.Remove(movers[i]);
                    i--;
                }
            }

            //clear out top level bullets
            for (int i = 0; i < topLevelMovers.Count; i++)
            {
                if (topLevelMovers[i].TasksFinished())
                {
                    topLevelMovers.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Clear()
        {
            movers.Clear();
            topLevelMovers.Clear();
        }

        public double Tier()
        {
            return 0.0;
        }
    }
}
