using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using SharpDX.MediaFoundation;
using System;

namespace NezTopDown.Components.Projectiles
{
    public class WeaponBullet : Projectile
    {

        public WeaponBullet(int weaponID, Vector2 direction, float range = 100f, float moveSpeed = 5f)
            : base(weaponID, direction, range, moveSpeed)
        {
        }

        public override void Update()
        {
            if (_range == 0)
            {       
                Vector2 motion = Direction * _moveSpeed;
                if (_mover.Move(motion))
                    Entity.Destroy();
              
            }
            else
            {
                Vector2 motion = Direction * _moveSpeed * _remainingRange / _range;
                if (_mover.Move(motion))
                {
                    Entity.Destroy();
                }
                _remainingRange -= _moveSpeed;
            }
        }
    }
}
