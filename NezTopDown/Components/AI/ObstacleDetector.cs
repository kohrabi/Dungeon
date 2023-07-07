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
    public class ObstacleDetector : Detector
    {
        private float detectionRadius = 50f;

        public int layerMask = 0;

        Collider[] colliders = new Collider[500];

        public override void OnAddedToEntity()
        {
            //Flags.SetFlag(ref layerMask, 1);
            Flags.SetFlag(ref layerMask, 2);
            Flags.SetFlag(ref layerMask, 3);
            base.OnAddedToEntity();
        }

        public override void Detect(AIData aiData)
        {
            int collide = Physics.OverlapCircleAll(Entity.Transform.Position, detectionRadius, colliders, layerMask);
            aiData.obstacles = colliders;
        }

        public override void DebugRender(Batcher batcher)
        {
            //batcher.DrawCircle(Entity.Position, detectionRadius, Color.CornflowerBlue);
            if (colliders != null)
            {
                foreach (Collider obstacleCollider in colliders)
                {
                    if (obstacleCollider == null)
                        break;
                    batcher.DrawCircle(obstacleCollider.Transform.Position, 3, Color.Red);
                }
            }
            base.DebugRender(batcher);
        }
    }
}
