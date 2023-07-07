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
        private float radius = 70f;

        public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
        {
            var player = Entity.Scene.FindEntity("player");
            float distance = Vector2.Distance(Entity.Transform.Position, player.Transform.Position);
            if (distance <= radius)
            {
                float weight = 1 - distance / radius;
                Vector2 direction = player.Transform.Position - Entity.Transform.Position;
                for (int i = 0; i < Directions.eightDirections.Count; i++)
                {
                    float result = Vector2.Dot(direction, Directions.eightDirections[i]); 

                    float valueToPut = result * weight;
                    if (valueToPut > danger[i])
                    {
                        danger[i] = valueToPut;
                    }
                }
            }
            return (danger, interest);
        }
    }
}
