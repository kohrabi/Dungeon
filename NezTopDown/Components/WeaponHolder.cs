using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tweens;
using NezTopDown.Components.AI;
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
        Entity weaponSprite, weaponOrigin;
        List<int> Weapons;
        SpriteRenderer sprite;

        public Vector2 aimingDirection = Vector2.Zero;
        Transform player;

        int equipping = 0;
        public int currentWeapon { get; private set; } = 1;
        float firerateRemain = 0f;

        public override void OnAddedToEntity()
        {
            weaponOrigin = Entity.Scene.CreateEntity(Entity.Name + "Weapon");
            weaponOrigin.SetParent(Entity);
            weaponOrigin.Transform.Position = new Vector2(0, 5);

            //currentWeapon = Nez.Random.Range(1, Game1.WeaponsList.Count);
            Weapons = new List<int> { currentWeapon };

            weaponSprite = Entity.Scene.CreateEntity(Entity.Name + "WeaponSprite");
            weaponSprite.SetParent(weaponOrigin);
            sprite = weaponSprite.AddComponent(new SpriteRenderer(Game1.WeaponsList[currentWeapon].sprite));
            sprite.FlipY = true;
            sprite.LayerDepth = -1;
            weaponSprite.Transform.Position = new Vector2(-10, 15);


            player = Entity.Scene.FindEntity("player").Transform;
            base.OnAddedToEntity();
        }

        [Inspectable]
        float angle;


        void Nez.IUpdateable.Update()
        {
            //invisibleWeapon.TweenLocalRotationDegreesTo(123f, 0.3f).SetEaseType(Nez.Tweens.EaseType.ExpoOut).SetLoops(LoopType.RestartFromBeginning, -1).Start();
            /*if (Input.LeftMouseButtonPressed || isAttacking)
            {
                isAttacking = true;
                weaponOrigin.TweenRotationDegreesTo(weaponOrigin.Transform.Rotation + (float)(Math.PI / 3.0f), 0.1f)
                        .SetEaseType(Nez.Tweens.EaseType.ExpoOut)
                        .SetCompletionHandler((x) => { isAttacking = false; }) // When the tweening has completed
                        .Start();

            }*/

            float angleOffset = 0;
            if (Entity.Name == "player")
            {
                aimingDirection = Entity.Transform.Position - Entity.Scene.Camera.ScreenToWorldPoint(Input.MousePosition);
                angleOffset = (float)(Math.PI / 3);
            }
            else
            {
                if (Entity.GetComponent<TargetDetector>().Detected == true)
                    aimingDirection = Entity.Transform.Position - player.Position;
                angleOffset = 0;
            }
            angle = Mathf.Atan2(aimingDirection.Y, aimingDirection.X);

            if (angle <= Math.PI / 2 && angle >= -MathF.PI / 2)
            {
                weaponSprite.LocalPosition = new Vector2(-10, -15);
                weaponOrigin.Transform.Rotation = angle + angleOffset;
                sprite.FlipY = false;
            }
            else
            {
                weaponSprite.LocalPosition = new Vector2(-10, 15);
                weaponOrigin.Transform.Rotation = angle - angleOffset;
                sprite.FlipY = true;
            }
        }

        public void Attack(Vector2 direction)
        {
            Weapon weapon = Game1.WeaponsList[currentWeapon];
            firerateRemain = Math.Max(0, firerateRemain - weapon.firerate);
            if (firerateRemain <= 0)
            {
                firerateRemain = weapon.firerate;
                var entity = Entity.Scene.CreateEntity(Entity.Name + "Projectile");
                entity.Transform.Position = Entity.Transform.Position;
                if (weapon.type == 1)
                    entity.AddComponent(new Melee(currentWeapon, direction, weapon.range, weapon.speed));
                Entity.Scene.Camera.GetComponent<CameraShake>().Shake(8, 0.7f, Vector2.Normalize(aimingDirection));
            }
        }

        public int ChangeWeapon(int weapon)
        {
            //if (Weapons.Count == 1)
            //{
            //    Weapons.Add(weapon);
            //    return 0;
            //}
            //else
            //{
                int equip = currentWeapon;
                currentWeapon = weapon;
                Weapons[equipping] = currentWeapon;
                firerateRemain = Game1.WeaponsList[currentWeapon].firerate;
                weaponSprite.GetComponent<SpriteRenderer>().SetSprite(Game1.WeaponsList[currentWeapon].sprite);
                return equip;
            //}
        }
    }
}
