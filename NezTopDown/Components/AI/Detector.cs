using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;

namespace NezTopDown.Components.AI
{
    // From: Sunney Valley Studio: https://www.youtube.com/@SunnyValleyStudio
    public abstract class Detector : Component
    {
        public abstract void Detect(AIData aiData);
    }
}
