using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using NezTopDown.Components.AI;
using NezTopDown.Components.Projectiles;
using System;

namespace NezTopDown.Components
{
    public class Enemy : Component, Nez.IUpdateable, ITriggerListener
    {
        float _moveSpeed = 100f;
        public Vector2 MovementInput { get; set; }
        Vector2 hitMotion;

        const float knockbackLength = 5f;
        const float knockbackMag = 5f;
        const float delayTime = 0.16f;

        SpriteAnimator _animator;
        Mover _mover;
        public EntityState enemyState { get; private set; }
        string _animation;

        float enemyHealth;

        public override void OnAddedToEntity()
        {
            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Enemy/Goblin/Goblin.atlas");

            enemyHealth = 4;

            _animator = Entity.AddComponent(new SpriteAnimator());
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.Play("Idle");
            _animator.LayerDepth = 0;
            _animator.SetMaterial(new Material());
            _animation = _animator.CurrentAnimationName;

            enemyState = EntityState.Free;

            Entity.AddComponent(new WeaponHolder());
            // ------------Collider--------------
            var collider = Entity.AddComponent(new BoxCollider());
            Flags.SetFlag(ref collider.PhysicsLayer, 3);
            collider.CollidesWithLayers = 0;
            Nez.Flags.SetFlag(ref collider.CollidesWithLayers, 3); // enemy
            Nez.Flags.SetFlag(ref collider.CollidesWithLayers, 2); // tile
            //collider.IsTrigger = true;

            _mover = Entity.AddComponent(new Mover());
            // --------------AI------------------
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

        void Nez.IUpdateable.Update()
        {
            switch (enemyState)
            {
                case EntityState.Free:
                    EnemyState_Free();
                    break;
                case EntityState.Hit:
                    EnemyState_Hit();
                    break;
            }
            if (!_animator.IsAnimationActive(_animation))
                _animator.Play(_animation);

        }

        void EnemyState_Free()
        {
            if (MovementInput != Vector2.Zero)
            {
                var movement = MovementInput * _moveSpeed * Time.DeltaTime;
                _animator.FlipX = !(MovementInput.X > 0);
                _animation = "Run";
                _mover.Move(movement, out var res);
            }
            else
                _animation = "Idle";
        }

        float delayRemain = delayTime;
        float knockbackRemain = knockbackLength;
        void EnemyState_Hit()
        {
            if (knockbackRemain <= 0)
            {
                if (delayRemain <= 0f)
                {
                    enemyState = EntityState.Free;
                    knockbackRemain = knockbackLength;
                    delayRemain = delayTime;
                    _animator.UnPause();
                }
                _animator.Material.Effect = null;
                delayRemain -= Time.DeltaTime;
            }
            else
            {
                _animation = "Death";
                _animator.Pause();
                _animator.Material.Effect = Game1.HitFlashEffect;
                _mover.Move(Vector2.Normalize(hitMotion) * knockbackMag, out var res);
                knockbackRemain = Math.Max(0, knockbackRemain - knockbackMag / knockbackLength);
            }
        }

        public void Hit(Vector2 motion, float hitPoint)
        {
            Entity.Scene.Camera.GetComponent<CameraShake>().Shake(8, 0.7f);
            hitMotion = motion;
            enemyHealth -= hitPoint;
            enemyState = EntityState.Hit;
        }

        uint check;
        public void OnTriggerEnter(Collider other, Collider local)
        {
            if (check != other.Entity.Id)
            {
                int hitBy = other.Entity.GetComponent<Melee>().WeaponID;
                Hit(local.Transform.Position - other.Transform.Position, Game1.WeaponsList[hitBy].hitPoint);
                check = other.Entity.Id;
            }
        }
        
        public void OnTriggerExit(Collider other, Collider local)
        {
        }
    }
}
