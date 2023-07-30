using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using System;
using System.Linq;

namespace NezTopDown.Components.AI
{
    public class SeekBehaviour : SteeringBehaviour
    {
        private float targetReachedThreshold = 30f;

        bool reachedLastTarget = true;

        //debug parameters
        private Vector2 targetPositionCached;
        private float[] interestsTemp;

        public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
        {
            //if we don't have a target stop seeking
            //else set a new target
            if (reachedLastTarget)
            {
                if (aiData.targets == null || aiData.targets.Count <= 0)
                {
                    aiData.currentTarget = null;
                    return (danger, interest);
                }
                else
                {
                    reachedLastTarget = false;
                    aiData.currentTarget = aiData.targets.OrderBy(target => Vector2.Distance(target.Position, Entity.Transform.Position)).FirstOrDefault();
                }

            }

            //cache the last position only if we still see the target (if the targets collection is not empty)
            if (aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
                targetPositionCached = aiData.currentTarget.Position;

            //First check if we have reached the target
            if (Vector2.Distance(Entity.Transform.Position, targetPositionCached) < targetReachedThreshold)
            {
                reachedLastTarget = true;
                aiData.currentTarget = null;
                return (danger, interest);
            }

            //If we havent yet reached the target do the main logic of finding the interest directions
            float targetAngle = Utils.PointDirection(targetPositionCached, Entity.Transform.Position);
            for (int i = 0; i < interest.Length; i++)
            {
                float angle = (360 / 8) * i;
                float difference = Math.Abs(Utils.AngleDifference(angle, targetAngle));
                float result = (180 - difference) / 180;
                interest[i] = result;
                //accept only directions at the less than 90 degrees to the target direction

            }
            interestsTemp = interest;
            return (danger, interest);
        }

        //public override void DebugRender(Batcher batcher)
        //{
        //    batcher.DrawCircle(targetPositionCached, 2f, Color.White);
        //    if (interestsTemp != null)
        //    {
        //        for (int i = 0; i < interestsTemp.Length; i++)
        //        {
        //            batcher.DrawLine(Entity.Transform.Position, Entity.Transform.Position + Directions.eightDirections[i] * interestsTemp[i] * 50, Color.White);
        //        }
        //        if (reachedLastTarget == false)
        //        {
        //            batcher.DrawCircle(targetPositionCached, 1f, Color.Red);
        //        }
        //    }
        //
        //    base.DebugRender(batcher);
        //}
    }
}
