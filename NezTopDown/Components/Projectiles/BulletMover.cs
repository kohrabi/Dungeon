using BulletMLLib;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.Projectiles
{
    public class BulletMover : Bullet
    {
        public Vector2 pos;

        public override float X
        {
            get { return pos.X; }
            set { pos.X = value; }
        }

        public override float Y
        {
            get { return pos.Y; }
            set { pos.Y = value; }
        }

        public bool Used { get; set; }

        public BulletMover(IBulletManager bulletManager) : base(bulletManager)
        {

        }

        public void Init()
        {
            Used = true;
        }

        public override void PostUpdate()
        {
            if (X < 0 || X > Screen.PreferredBackBufferWidth || Y < 0 || Y > Screen.PreferredBackBufferHeight)
            {
                Used = false;
            }
        }
    }
}
