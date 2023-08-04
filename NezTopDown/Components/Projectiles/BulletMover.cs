using BulletMLLib;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.Projectiles
{
    public class BulletMover : Bullet
    {
        public bool Used { get; set; }

        public Sprite BulletSprite { get; set; }

        public BulletMover(BulletMLManager bulletManager) : base(bulletManager)
        {
            Used = true;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            if (Entity.Name != "topbullet")
            {
                var _sprite = Entity.AddComponent(new SpriteRenderer(BulletMLManager.BulletSprite));
                _sprite.LayerDepth = LayerDepths.GetLayerDepth(LayerDepths.Sorting.Projectiles);

            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void PostUpdate()
        {
            if (X < 0 || X > Screen.PreferredBackBufferWidth || Y < 0 || Y > Screen.PreferredBackBufferHeight)
            {
                Used = false;
                Entity.Destroy();
            }
        }
    }
}
