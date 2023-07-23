using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework.Input;

namespace NezTopDown.Components
{
    public class Chest : Component, Nez.IUpdateable
    {
        Texture2D chestClosed, chestOpened;
        Entity weapon, player;
        BoxCollider weaponCollider;
        SpriteRenderer weaponSprite;

        int weaponID;

        public override void OnAddedToEntity()
        {
            chestClosed = Entity.Scene.Content.Load<Texture2D>("Sprites/Props/Chest");
            chestOpened = Entity.Scene.Content.Load<Texture2D>("Sprites/Props/ChestOpened");

            Entity.AddComponent(new BoxCollider()).IsTrigger = true;
            Entity.AddComponent(new SpriteRenderer(chestClosed)).LayerDepth = 2;
            Entity.Scale = new Vector2(1.5f);

            weapon = Entity.Scene.CreateEntity("weaponOnGround", Entity.Position);
            weaponID = Nez.Random.Range(1, Game1.WeaponsList.Count);
            weaponSprite = weapon.AddComponent(new SpriteRenderer(Game1.WeaponsList[weaponID].sprite));
            weaponSprite.LayerDepth = -5;
            weaponSprite.Enabled = false;
            weaponCollider = weapon.AddComponent(new BoxCollider());
            weaponCollider.IsTrigger = true;
            weaponCollider.Enabled = false;

            player = Entity.Scene.FindEntity("player");
        }

        void Nez.IUpdateable.Update()
        {
            // dude this is fuck up
            CollisionResult collision;
            if (Entity.GetComponent<BoxCollider>().CollidesWith(
                player.GetComponent<BoxCollider>(), out collision))
            {
                Entity.GetComponent<SpriteRenderer>().SetSprite(new Sprite(chestOpened));
                weaponSprite.Enabled = true;
                weaponCollider.Enabled = true;
            }
            if (weaponCollider.CollidesWith(player.GetComponent<BoxCollider>(), out collision) && player.GetComponent<Player>().ChangeWeapon == true)
            {
                int playerPrevWeapon = player.GetComponent<WeaponHolder>().ChangeWeapon(weaponID);
                if (playerPrevWeapon != 0)
                {
                    weaponSprite.SetSprite(Game1.WeaponsList[playerPrevWeapon].sprite);
                    weaponID = playerPrevWeapon;
                }
            }
        }
    }
}
