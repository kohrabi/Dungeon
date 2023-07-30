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
        #region Constants

        const float knockbackLength = 0.1f;
        const float knockbackMag = 5f;
        const float delayTime = 0.16f;
        const float moveSpeed = 100f;
        const float attackDelay = 1f;

        #endregion

        #region Components

        private SpriteAnimator _animator;
        private Mover _mover;
        private WeaponHolder _weapon;

        #endregion

        #region Vars
        Transform player;

        float enemyHealth = 12f;
        Vector2 hitMotion;
        string _animation;
        public Vector2 MovementInput { get; set; }
        public EntityState enemyState { get; private set; }

        // Counter for delays
        float attackDelayRemain = attackDelay;
        float delayRemain = delayTime;
        float knockbackRemain = knockbackLength;

        int hitBy;
        #endregion

        public override void OnAddedToEntity()
        {

            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Enemy/Goblin/Goblin.atlas");
            player = Entity.Scene.FindEntity("player").Transform;

            _animator = Entity.AddComponent(new SpriteAnimator());
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.Play("Idle");
            _animator.SetLayerDepth(LayerDepths.GetLayerDepth(LayerDepths.Sorting.Enemy));
            _animator.SetMaterial(new Material());
            _animation = _animator.CurrentAnimationName;

            enemyState = EntityState.Free;

            _weapon = Entity.AddComponent(new WeaponHolder());

            // ------------Collider--------------
            var collider = Entity.AddComponent(new BoxCollider());
            Flags.SetFlag(ref collider.PhysicsLayer, (int)PhysicsLayers.Enemy);
            collider.CollidesWithLayers = 0;
            Nez.Flags.SetFlag(ref collider.CollidesWithLayers, (int)PhysicsLayers.Enemy); // enemy
            Nez.Flags.SetFlag(ref collider.CollidesWithLayers, (int)PhysicsLayers.Tile); // tile
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
            var targetCircleBehaviour = Entity.AddComponent(new TargetCircleBehaviour());

            ai.AddSteeringBehaviour(seekBehaviour);
            ai.AddSteeringBehaviour(obstacleAvoidanceBehaviour);
            ai.AddSteeringBehaviour(targetCircleBehaviour);

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
                case EntityState.Dead:
                    EnemyState_Dead();
                    break;
            }
            if (!_animator.IsAnimationActive(_animation))
                _animator.Play(_animation);

        }

        void EnemyState_Free()
        {
            if (MovementInput != Vector2.Zero)
            {
                var movement = MovementInput * moveSpeed * Time.DeltaTime;
                _animator.FlipX = !(MovementInput.X > 0);
                _animation = "Run";
                _mover.Move(movement, out var res);
            }
            else
                _animation = "Idle";

            attackDelayRemain = Math.Max(0, attackDelayRemain - Time.DeltaTime);
            Vector2 directionToPlayer = player.Position - Entity.Transform.Position;
            if (directionToPlayer.Length() <= 70f)
            {
                if (attackDelayRemain <= 0f)
                {
                    _weapon.Attack(_weapon.aimingDirection);
                    attackDelayRemain = attackDelay;
                }
            }
        }

        void EnemyState_Hit()
        { 
            if (enemyHealth <= 0)
            {
                enemyState = EntityState.Dead;
                LevelGenerator.enemyCount--;
                knockbackRemain *= 2;
                _animator.UnPause();
                _animation = "Death";
                return;
            }

            if (knockbackRemain <= 0)
            {
                if (delayRemain <= 0f)
                {
                    if (enemyHealth > 0)
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
                if (_mover.Move(Vector2.Normalize(hitMotion) * knockbackMag * knockbackRemain / knockbackLength, out var res))
                    hitMotion = -hitMotion;
                knockbackRemain = Math.Max(0, knockbackRemain - Time.DeltaTime);
            }
        }

        void EnemyState_Dead()
        {
            if (_animator.CurrentFrame == _animator.CurrentAnimation.Sprites.Length - 1)
                _animator.Pause();

            if (knockbackRemain > 0f)
            {
                float strength = Game1.WeaponsList[hitBy].hitPoint / 10 * knockbackMag * knockbackRemain / knockbackLength;
                if (_mover.Move(Vector2.Normalize(hitMotion) * strength, out var res))
                    hitMotion = -hitMotion;
            }
            else
            { 
                Entity.GetComponent<BoxCollider>().Enabled = false;
                Entity.GetComponent<EnemyAI>().Enabled = false; 
            }
            knockbackRemain = Math.Max(0, knockbackRemain - Time.DeltaTime);
            _animator.Color = Color.Gray;

            // disable entity leaving only the corpse
            Entity.GetComponent<WeaponHolder>().Enabled = false;
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
            if (other.Entity.Parent.Entity.Name != local.Entity.Name && check != other.Entity.Id && enemyHealth > 0)
            {
                hitBy = other.Entity.GetComponent<Projectile>().WeaponID;
                Hit(other.Entity.GetComponent<Projectile>().Direction, Game1.WeaponsList[hitBy].hitPoint);
                check = other.Entity.Id;
            }
        }
        
        public void OnTriggerExit(Collider other, Collider local)
        {
        }
    }
}
