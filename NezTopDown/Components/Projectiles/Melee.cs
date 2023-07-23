using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.Projectiles
{
    public class Melee : Component, Nez.IUpdateable
    {
        ProjectileMover _mover;
        SpriteAnimator _animator;
        float _range;
        float _remainingRange;
        Vector2 _direction;
        float _moveSpeed;
        BoxCollider _collider;

        public int WeaponID { get; private set; }

        public Melee(int weaponID, Vector2 direction, float range = 100f, float moveSpeed = 5f)
        {
            WeaponID = weaponID;
            _direction = -direction; 
            _range = range;
            _moveSpeed = moveSpeed;
            _remainingRange = range;
        }

        public override void OnAddedToEntity()
        {
            _collider = Entity.AddComponent(new BoxCollider());
            _collider.IsTrigger = true;
            _collider.CollidesWithLayers = 1 << 3;
            _collider.SetSize(78f, 41f);
            _mover = Entity.AddComponent(new ProjectileMover());

            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Projectiles/Projectiles.atlas");
            _animator = Entity.AddComponent(new SpriteAnimator());
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.LayerDepth = -1;
            _animator.Play("SwordSwing", SpriteAnimator.LoopMode.ClampForever);
            Entity.Transform.Rotation = Mathf.Atan2(_direction.Y, _direction.X);
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
            base.OnAddedToEntity();
        }
        
        void Nez.IUpdateable.Update()
        {
            Vector2 motion = _direction * _moveSpeed * _remainingRange / _range;
            _mover.Move(motion);
            if (_remainingRange <= 0)// if (move and collide)
            {
                Entity.Destroy();
            }
            _remainingRange -= _moveSpeed;
        }
    }
}
