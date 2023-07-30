using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components.AI
{
    // From: Sunney Valley Studio: https://www.youtube.com/@SunnyValleyStudio
    public class AIData : Component
    {
        public List<Transform> targets = null;
        public Collider[] obstacles = null;

        public Transform currentTarget;

        public int GetTargetsCount() => targets == null ? 0 : targets.Count;

        public override void Initialize()
        {
            currentTarget = Entity.Scene.FindEntity("player").Transform;
            base.Initialize();
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
        }
    }
}
