using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Nez.Sprites;
using Nez.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.Projectiles
{
    public abstract class Projectile : Component, Nez.IUpdateable
    {
        public int Type { get; set; }
        public int WeaponID { get; private set; }
        public Vector2 Direction { get; private set; }

        protected float _range;
        protected float _remainingRange;
        protected float _moveSpeed;
        protected ProjectileMover _mover;

        private BoxCollider _collider;
        private SpriteAnimator _animator;


        protected Projectile(int weaponID, Vector2 direction, float range = 100f, float moveSpeed = 5f)
        {
            WeaponID = weaponID;
            Type = Game1.WeaponsList[WeaponID].type;
            Direction = -Vector2.Normalize(direction);
            _range = range;
            _moveSpeed = moveSpeed;
            _remainingRange = range;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Projectiles/Projectiles.atlas");
            _animator = Entity.AddComponent(new SpriteAnimator());
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.SetLayerDepth(LayerDepths.GetLayerDepth(LayerDepths.Sorting.Projectiles));
            if (Type == 1)
                _animator.Play("SwordSwing", SpriteAnimator.LoopMode.ClampForever);
            else
                _animator.Play("Bullet");

            _collider = Entity.AddComponent(new BoxCollider());
            _collider.IsTrigger = true;
            _collider.CollidesWithLayers = 0;
            Flags.SetFlag(ref _collider.CollidesWithLayers, (int)PhysicsLayers.Tile); // tiles
            Flags.SetFlag(ref _collider.CollidesWithLayers, (int)PhysicsLayers.Enemy); // enemy
            _mover = Entity.AddComponent(new ProjectileMover());

            Entity.Transform.Rotation = Mathf.Atan2(Direction.Y, Direction.X);
            if (Type == 1)
            {
                if (Entity.Transform.Rotation <= Math.PI / 2 && Entity.Transform.Rotation >= -MathF.PI / 2)
                {
                    _animator.FlipY = false;
                    Entity.Transform.Rotation -= (float)Math.PI / 8;
                }
                else
                {
                    _animator.FlipY = true;
                    Entity.Transform.Rotation += (float)Math.PI / 8;
                }
            }
        }

        public abstract void Update();
    }
}
