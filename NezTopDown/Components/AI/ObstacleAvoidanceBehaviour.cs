using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;

namespace NezTopDown.Components.AI
{
    public class ObstacleAvoidanceBehaviour : SteeringBehaviour
    {
        private float radius = 30f, agentColliderSize = 17f;

        //gizmo parameters
        float[] dangersResultTemp = null;

        public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
        {
            if (aiData.obstacles == null)
                return (danger, interest);
            foreach (Collider obstacleCollider in aiData.obstacles)
            {
                if (obstacleCollider == null)
                    break;
                if (obstacleCollider.Entity == Entity)
                    continue;
                Vector2 directionToObstacle = 
                    obstacleCollider.Bounds.GetClosestPointOnRectangleBorderToPoint(Entity.Transform.Position, out var e) - Entity.Transform.Position;
                float distanceToObstacle = directionToObstacle.Length();

                //calculate weight based on the distance Enemy<--->Obstacle
                float weight = distanceToObstacle <= agentColliderSize ? 1 : (radius - distanceToObstacle) / radius;
                if (weight < 0)
                    continue;

                Vector2 directionToObstacleNormalized = Vector2.Normalize(directionToObstacle);

                //Add obstacle parameters to the danger array
                for (int i = 0; i < Directions.eightDirections.Count; i++)
                {
                    //Debug.DrawLine(Entity.Transform.Position, Entity.Transform.Position + directionToObstacleNormalized * 100, Color.Aqua);
                    //Debug.DrawLine(Entity.Transform.Position, Entity.Transform.Position + Directions.eightDirections[i] * 100, Color.Aqua);
                    float result = Vector2.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);

                    float valueToPutIn = result * weight;

                    //override value only if it is higher than the current one stored in the danger array
                    if (valueToPutIn > danger[i])
                    {
                        danger[i] = valueToPutIn;
                    }
                }
            }
            dangersResultTemp = danger;
            return (danger, interest);
        }


        [Inspectable]
        bool showObstacle = true;
        public override void DebugRender(Batcher batcher)
        {
            if (!showObstacle) return;
            //batcher.DrawCircle(Entity.Transform.Position, radius, Color.Aqua);
            //batcher.DrawCircle(Entity.Transform.Position, agentColliderSize, Color.Black);
            if (dangersResultTemp != null)
            {
                for (int i = 0; i < dangersResultTemp.Length; i++)
                {
                    batcher.DrawLine(Entity.Transform.Position, Entity.Transform.Position + Directions.eightDirections[i] * dangersResultTemp[i] * 100, Color.Red);
                }
            }
            else
            {
                batcher.DrawCircle(Entity.Transform.Position, radius, Color.Red);
            }
            base.DebugRender(batcher);
        }
    }
    public static class Directions
    {
        public static List<Vector2> eightDirections = new List<Vector2>{
            Vector2.Normalize(new Vector2(1,0)),
            Vector2.Normalize(new Vector2(1,1)),
            Vector2.Normalize(new Vector2(0,1)),
            Vector2.Normalize(new Vector2(-1,1)),
            Vector2.Normalize(new Vector2(-1,0)),
            Vector2.Normalize(new Vector2(-1,-1)),
            Vector2.Normalize(new Vector2(0,-1)),
            Vector2.Normalize(new Vector2(1,-1))
        };
    }
}
