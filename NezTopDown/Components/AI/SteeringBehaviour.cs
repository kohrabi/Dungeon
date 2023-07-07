using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.AI
{
    public abstract class SteeringBehaviour : Component
    {
        public abstract (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData);
    }
}
