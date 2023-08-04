using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.AI
{
    public class CombatMovementBehaviour : SteeringBehaviour
    {
        // another retarded shit
        // every fucking lines of code i wrote is fucking retarded ngl
        const float movebackDelay = 0.06f;
        float movebackRemain = 0;

        public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
        {
            var enemy = Entity.GetComponent<Enemy>();
            Vector2 directionToPlayer = Entity.Position - enemy.player.Position;
            if ((enemy.IsAttacking || directionToPlayer.Length() <= 50f) && movebackRemain <= 0)
            {
                movebackRemain = movebackDelay;
            }
            if (movebackRemain > 0)
            {
                float direction = Utils.PointDirection(Vector2.Zero, Vector2.Normalize(directionToPlayer));
                for (int i = 0; i < 8; i++)
                {
                    float angle = (360 / 8) * i;
                    float difference = Math.Abs(Utils.AngleDifference(angle, direction));
                    float result = (180 - difference) / 180;
                    result = 1 - Math.Abs(result);
                    interest[i] = result;
                    Debug.DrawLine(Entity.Position, Entity.Position + Directions.eightDirections[i] * result * 100f, Color.White);
                }
                movebackRemain = Math.Max(0, movebackRemain - Time.DeltaTime);
            }

            return (danger, interest);
        }
    }
}
