using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.AI
{
    public class TargetCircleBehaviour : SteeringBehaviour
    {
        private float radius = 120f;

        // Some Dummy dumb dumb stuff based on Game Endeavor's Devlog
        public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
        {
            var player = Entity.Scene.FindEntity("player").Transform.Position;
            float distance = Vector2.Distance(Entity.Transform.Position, player);
            if (distance <= radius)
            {
                for (int i = 0; i < 8; i++)
                {
                    interest[i] = 1 - Math.Abs(0.6f - interest[i]);
                }
            }
            return (danger, interest);
        }
    }
}
