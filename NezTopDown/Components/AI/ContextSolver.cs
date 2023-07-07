using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.AI
{
    public class ContextSolver : Component
    {
        //gozmo parameters
        float[] interestGizmo = new float[0];
        Vector2 resultDirection = Vector2.Zero;
        private float rayLength = 50;

        public ContextSolver()
        {
            interestGizmo = new float[8];
        }

        public Vector2 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIData aiData)
        {
            float[] danger = new float[8];
            float[] interest = new float[8];

            //Loop through each behaviour
            foreach (SteeringBehaviour behaviour in behaviours)
            {
                (danger, interest) = behaviour.GetSteering(danger, interest, aiData);
            }

            //subtract danger values from interest array
            for (int i = 0; i < 8; i++)
            {
                interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
            }

            interestGizmo = interest;

            //get the average direction
            Vector2 outputDirection = Vector2.Zero;
            for (int i = 0; i < 8; i++)
            {
                outputDirection += Directions.eightDirections[i] * interest[i];
            }

            if (outputDirection == Vector2.Zero)
                return outputDirection;
            outputDirection.Normalize();

            resultDirection = outputDirection;

            //return the selected movement direction
            return resultDirection;
        }


        public override void DebugRender(Batcher batcher)
        {
            batcher.DrawLine(Entity.Transform.Position, Entity.Transform.Position + resultDirection * rayLength, Color.Yellow);
            base.DebugRender(batcher);
        }
    }
}
