using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components
{
    public class Chest : Component, Nez.IUpdateable
    {
        Texture2D chestClosed, chestOpened;

        public override void OnAddedToEntity()
        {
            chestClosed = Entity.Scene.Content.Load<Texture2D>("Sprites/Props/Chest");
            chestOpened = Entity.Scene.Content.Load<Texture2D>("Sprites/Props/ChestOpened");

            Entity.AddComponent(new BoxCollider()).IsTrigger = true;
            Entity.AddComponent(new SpriteRenderer(chestClosed));
            Entity.Scale = new Vector2(1.5f);
        }

        void Nez.IUpdateable.Update()
        {
            CollisionResult collision;
            
            if(Entity.GetComponent<BoxCollider>().CollidesWith(
                Entity.Scene.FindEntity("player").GetComponent<BoxCollider>(), out collision))
            {
                Entity.GetComponent<SpriteRenderer>().SetSprite(new Sprite(chestOpened));
            }
        }
    }
}
