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
        float moveSpeed = 5f;

        public Melee(Vector2 direction, float range = 120f)
        {
            _direction = -direction; 
            _range = range;
            _remainingRange = range;
        }

        public override void OnAddedToEntity()
        {
            Entity.AddComponent(new BoxCollider()).PhysicsLayer = 0;
            _mover = Entity.AddComponent(new ProjectileMover());
            
            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Projectiles/Projectiles.atlas");
            _animator = Entity.AddComponent(new SpriteAnimator());
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.LayerDepth = 0;
            _animator.Play("SwordSwing", SpriteAnimator.LoopMode.ClampForever);
            Entity.Transform.Rotation = Mathf.Atan2(_direction.Y, _direction.X);
            if (Entity.Transform.Rotation <= Math.PI / 2 && Entity.Transform.Rotation >= -MathF.PI / 2)
            {
                _animator.FlipY = false;
                Entity.Transform.Rotation -= (float)Math.PI / 6;
            }
            else
            {
                _animator.FlipY = true;
                Entity.Transform.Rotation += (float)Math.PI / 6;
            }
            base.OnAddedToEntity();
        }
        
        void Nez.IUpdateable.Update()
        {
            Vector2 motion = _direction * moveSpeed * _remainingRange / _range;
            _mover.Move(motion);
            if (_remainingRange <= 0)// if (move and collide)
            {
                Entity.Destroy();
            }
            _remainingRange -= moveSpeed;
        }
    }
}
