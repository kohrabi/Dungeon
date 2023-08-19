using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework.Input;

namespace NezTopDown.Components
{
    public class Chest : Component, Nez.IUpdatable
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
            Entity.AddComponent(new SpriteRenderer(chestClosed)).SetLayerDepth(LayerDepths.GetLayerDepth(LayerDepths.Sorting.Chest));
            Entity.Scale = new Vector2(1.5f);

            weapon = Entity.Scene.CreateEntity("weaponOnGround", Entity.Position);
            weaponID = 5;
            weaponSprite = weapon.AddComponent(new SpriteRenderer(GameManager.WeaponsList[weaponID].sprite));
            weaponSprite.SetLayerDepth(LayerDepths.GetLayerDepth(LayerDepths.Sorting.Weapons));
            weaponSprite.Enabled = false;
            weaponCollider = weapon.AddComponent(new BoxCollider());
            weaponCollider.IsTrigger = true;
            weaponCollider.Enabled = false;

            player = Entity.Scene.FindEntity("player");
        }

        public void Update()
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
                    weaponSprite.SetSprite(GameManager.WeaponsList[playerPrevWeapon].sprite);
                    weaponID = playerPrevWeapon;
                }
            }
        }
    }
}
