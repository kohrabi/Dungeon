using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using NezTopDown.Components.AI;
using System;

namespace NezTopDown.Components
{
    public class Enemy : Component, Nez.IUpdateable
    {
        float _moveSpeed = 100f;
        public Vector2 MovementInput { get; set; }

        SpriteAnimator _animator;
        Mover _mover;

        public override void OnAddedToEntity()
        {
            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Enemy/Goblin/Goblin.atlas");

            _animator = Entity.AddComponent(new SpriteAnimator());
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.Play("Idle");
            _animator.LayerDepth = 0;

            Entity.AddComponent(new WeaponHolder());
            var collider = Entity.AddComponent(new BoxCollider());
            collider.PhysicsLayer = 1 << 3;
            collider.CollidesWithLayers = 0;
            Nez.Flags.SetFlag(ref collider.CollidesWithLayers, 3); // enemy
            Nez.Flags.SetFlag(ref collider.CollidesWithLayers, 2); // tile
            //collider.IsTrigger = true;

            _mover = Entity.AddComponent(new Mover());
            // AI
            Entity.AddComponent(new AIData());
            var ai = Entity.AddComponent(new EnemyAI());

            var obstacleDetector = Entity.AddComponent(new ObstacleDetector());
            var targetDetector = Entity.AddComponent(new TargetDetector());

            ai.AddDetector(obstacleDetector);
            ai.AddDetector(targetDetector);

            var seekBehaviour = Entity.AddComponent(new SeekBehaviour());
            var obstacleAvoidanceBehaviour = Entity.AddComponent(new ObstacleAvoidanceBehaviour());
            //var targetCircleBehaviour = Entity.AddComponent(new TargetCircleBehaviour());

            ai.AddSteeringBehaviour(seekBehaviour);
            ai.AddSteeringBehaviour(obstacleAvoidanceBehaviour);
            //ai.AddSteeringBehaviour(targetCircleBehaviour);

            Entity.AddComponent(new ContextSolver());

            base.OnAddedToEntity();
        }

        float _range = 150f;
        void Nez.IUpdateable.Update()
        {
            if (MovementInput != Vector2.Zero)
            {
                var movement = MovementInput * _moveSpeed * Time.DeltaTime;
                //_animator.FlipX = !(moveDir.X > 0);
                //animation = "Run";
                _mover.Move(movement, out var res);
            }

        }

    }
}
