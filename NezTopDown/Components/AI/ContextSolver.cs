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
        Vector2 resultDirection = Vector2.UnitX;
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

            // Based on ChessMasterRiley's Implementation
            // because SunnyValleyStudio's one doesn't take into account up / down directions
            // causing the AI to be stuck in one place
            List<int> potentials = new List<int>();
            for (int i = 0; i < Directions.eightDirections.Count; i++)
            {
                if (danger[i] < 0.4f)
                    potentials.Add(i);
            }

            float result = Utils.PointDirection(resultDirection, Vector2.Zero);
            Vector2 outputDirection = Vector2.Zero;
            float[] gizmo = new float[8];
            foreach (int potentialDirection in potentials)
            {
                float inter = interest[potentialDirection];
                float direction = (360 / 8) * potentialDirection;
                float difference = Math.Abs(Utils.AngleDifference(direction, result));
                float sqr = ((180 - difference) / 180) * ((180 - difference) / 180);
                outputDirection.X += Utils.LengthDir_X(inter, direction) * sqr;
                outputDirection.Y += Utils.LengthDir_Y(inter, direction) * sqr;
                Vector2 test = new Vector2(Utils.LengthDir_X(inter, direction), Utils.LengthDir_Y(inter, direction));
                gizmo[potentialDirection] = test.Length();
            }
            if (outputDirection == Vector2.Zero)
                return outputDirection;
            outputDirection.Normalize();
            resultDirection = outputDirection;
            interestGizmo = gizmo;

            ////subtract danger values from interest array
            //for (int i = 0; i < 8; i++)
            //{
            //    interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
            //}
            //
            //interestGizmo = interest;
            //
            ////get the average direction
            //Vector2 outputDirection = Vector2.Zero;
            //for (int i = 0; i < 8; i++)
            //{
            //    outputDirection += Directions.eightDirections[i] * interest[i];
            //}
            //
            //if (outputDirection == Vector2.Zero)
            //    return outputDirection;
            //outputDirection.Normalize();
            //
            //resultDirection = outputDirection;

            //return the selected movement direction
            return resultDirection;
        }


        public override void DebugRender(Batcher batcher)
        {
            batcher.DrawLine(Entity.Transform.Position, Entity.Transform.Position + resultDirection * rayLength, Color.Yellow);
            for (int i = 0; i < 8; i++)
                batcher.DrawLine(Entity.Transform.Position, Entity.Transform.Position + Directions.eightDirections[i] * interestGizmo[i] * rayLength, Color.Yellow);
            base.DebugRender(batcher);
        }
    }
}
