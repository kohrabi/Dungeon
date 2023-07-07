using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tweens;
using NezTopDown.Components.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components
{
    public class WeaponHolder : Component, Nez.IUpdateable
    {
        Entity weaponSprite, invisibleWeapon;
        List<int> Weapons;
        int currentWeapon = 3;
        bool equippedChange = false;
        SpriteRenderer sprite;

        public Vector2 aimingDirection = Vector2.Zero;

        bool isAttacking = false;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            invisibleWeapon = Entity.Scene.CreateEntity(Entity.Name + "Weapon");
            invisibleWeapon.SetParent(Entity);
            invisibleWeapon.Transform.Position = new Vector2(0, 5);

            weaponSprite = Entity.Scene.CreateEntity(Entity.Name + "WeaponSprite");
            weaponSprite.SetParent(invisibleWeapon);
            sprite = weaponSprite.AddComponent(new SpriteRenderer(Game1.WeaponAtlas.Sprites[currentWeapon]));
            sprite.FlipY = true;
            sprite.LayerDepth = -1;
            weaponSprite.Transform.Position = new Vector2(-10, 15);

        }

        [Inspectable]
        float angle;


        void Nez.IUpdateable.Update()
        {
           //invisibleWeapon.TweenLocalRotationDegreesTo(123f, 0.3f).SetEaseType(Nez.Tweens.EaseType.ExpoOut).SetLoops(LoopType.RestartFromBeginning, -1).Start();
            /*if (Input.LeftMouseButtonPressed || attack)
            {
                attack = true;
                invisibleWeapon.TweenLocalRotationDegreesTo(invisibleWeapon.Transform.Rotation - (float)(Math.PI / 3.0f), 0.2f)
                        .SetEaseType(Nez.Tweens.EaseType.ExpoOut)
                        .SetCompletionHandler((x) => { attack = false; }) // When the tweening has completed
                        .Start();

            }*/
            aimingDirection = Entity.Transform.Position - Entity.Scene.Camera.ScreenToWorldPoint(Input.MousePosition);
            angle = Mathf.Atan2(aimingDirection.Y, aimingDirection.X);

            if (angle <= Math.PI / 2 && angle >= -MathF.PI / 2)
            {
                weaponSprite.LocalPosition = new Vector2(-10, -15);
                sprite.FlipY = false;
            }
            else
            {
                weaponSprite.LocalPosition = new Vector2(-10, 15);
                sprite.FlipY = true;
            }
            invisibleWeapon.Transform.Rotation = angle;
        }


        public void Attack(Vector2 direction)
        {
            var entity = Entity.Scene.CreateEntity(Entity.Name + "Projectile");
            entity.Transform.Position = Entity.Transform.Position;
            entity.AddComponent(new Melee(direction));
        }
    }
}
