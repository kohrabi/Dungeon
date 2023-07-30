using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.AI
{
    // From: Sunney Valley Studio: https://www.youtube.com/@SunnyValleyStudio

    public class TargetDetector : Detector
    {
        private float targetDetectionRange = 175f;

        private int obstaclesLayerMask, playerLayerMask;

        //gizmo parameters
        private List<Transform> colliders;

        public bool Detected { get; private set; }  

        public TargetDetector()
        {
            colliders = new List<Transform>();
        }

        public override void OnAddedToEntity()
        {
            Nez.Flags.SetFlag(ref obstaclesLayerMask, (int)PhysicsLayers.Tile); // tile
            Nez.Flags.SetFlag(ref obstaclesLayerMask, (int)PhysicsLayers.Player); // player

            Nez.Flags.SetFlag(ref playerLayerMask, (int)PhysicsLayers.Player);

            base.OnAddedToEntity();
        }

        public override void Detect(AIData aiData)
        {
            //Find out if player is near
            Collider playerCollider =
                Physics.OverlapCircle(Entity.Transform.Position, targetDetectionRange, playerLayerMask);

            if (playerCollider != null)
            {
                //Check if you see the player
                //Normalize(EndPoint - StartPoint) : direction * targetDetectionRange to get the end point
                if (playerCollider.Transform.Position - Entity.Transform.Position == Vector2.Zero)
                {
                    colliders = null;
                    return;
                }
                Vector2 endPoint = Entity.Transform.Position + Vector2.Normalize(playerCollider.Transform.Position - Entity.Transform.Position) * targetDetectionRange;

               
                RaycastHit hit =
                    Physics.Linecast(Entity.Transform.Position, endPoint, obstaclesLayerMask);


                //Make sure that the collider we see is on the "Player" layer
                if (hit.Collider != null && Flags.IsFlagSet(hit.Collider.PhysicsLayer, playerLayerMask))
                {
                    //Debug.DrawLine(Entity.Transform.Position, playerCollider.Transform.Position, Color.Magenta);
                    colliders = new List<Transform>() { playerCollider.Transform };
                    Detected = true;
                }
                else
                {
                    Detected = false;
                    colliders = null;
                }
            }
            else
            {
                Detected = false;
                //Enemy doesn't see the player
                colliders = null;
            }
            aiData.targets = colliders;
        }

        public override void DebugRender(Batcher batcher)
        {
            //batcher.DrawCircle(Entity.Transform.Position, targetDetectionRange, Color.Violet);

            if (colliders == null)
                return;

            foreach(var item in colliders)
            {
                batcher.DrawCircle(item.Position, 3f, Color.Magenta);
            }
            

            base.DebugRender(batcher);
        }
    }
}
