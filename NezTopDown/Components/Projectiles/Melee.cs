using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;

namespace NezTopDown.Components.Projectiles
{
    public class Melee : Projectile
    {
        public Melee(int weaponID, Vector2 direction, float range = 100f, float moveSpeed = 5f)
            : base(weaponID, direction, range, moveSpeed)
        {

        }

        public override void Update()
        {
            Vector2 motion = Direction * _moveSpeed * _remainingRange / _range;
            _mover.Move(motion);
            _remainingRange = Math.Max(0, _remainingRange - _moveSpeed);
            if (_remainingRange <= 0)// if (move and collide)
            {
                Entity.Destroy();
            }
            if (_animator.CurrentFrame == 2)
                _collider.Enabled = false;
        }
    }
}
