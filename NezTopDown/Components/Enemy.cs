using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using NezTopDown.Components.AI;
using NezTopDown.Components.LevelGen;
using NezTopDown.Components.Projectiles;
using System;

namespace NezTopDown.Components
{
    public class Enemy : Component, Nez.IUpdateable, ITriggerListener
    {
        #region Constants

        const float knockbackLength = 0.1f;
        const float knockbackMag = 5f;
        const float delayTime = 0.06f;
        const float moveSpeed = 100f;
        const float attackDelay = 1.3f;
        const float wanderDelay = 1.4f;
        const float wanderingTime = 1f;

        #endregion

        #region Components

        private SpriteAnimator _animator;
        private Mover _mover;
        private WeaponHolder _weapon;
        private BoxCollider _collider;

        #endregion

        #region Vars

        float enemyHealth = 12f;
        string _animation;
        Vector2 hitMotion;
        Vector2 wanderDirection;
        CollisionResult collisionResult;

        public Transform player { get; private set; }
        public bool IsAttacking { get; private set; }
        public Vector2 MovementInput { get; set; }
        public EntityState enemyState { get; private set; }

        // Counter for delays
        float attackDelayRemain = attackDelay;
        float delayRemain = delayTime;
        float knockbackRemain = knockbackLength;
        float wanderRemain = wanderDelay;
        float wanderingTimeRemain = 0;

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
            _collider = Entity.AddComponent(new BoxCollider());
            Flags.SetFlag(ref _collider.PhysicsLayer, (int)PhysicsLayers.Enemy);
            _collider.CollidesWithLayers = 0;
            Nez.Flags.SetFlag(ref _collider.CollidesWithLayers, (int)PhysicsLayers.Enemy); // enemy
            Nez.Flags.SetFlag(ref _collider.CollidesWithLayers, (int)PhysicsLayers.Tile); // tile
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
            var combatMovementBehaviour = Entity.AddComponent(new CombatMovementBehaviour());

            ai.AddSteeringBehaviour(seekBehaviour);
            ai.AddSteeringBehaviour(obstacleAvoidanceBehaviour);
            ai.AddSteeringBehaviour(targetCircleBehaviour);
            ai.AddSteeringBehaviour(combatMovementBehaviour);

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
            // Handling Movement
            // Most

            Vector2 movement = Vector2.Zero;

            // this is retarded i can just use the context steering instead of this 
            Vector2 directionToPlayer = player.Position - Entity.Transform.Position;
            if (MovementInput == Vector2.Zero)
            {
                if (wanderingTimeRemain <= 0 && wanderRemain <= 0)
                {
                    float angle = 360 / 16 * Nez.Random.Range(0, 15);
                    wanderDirection = new Vector2(Utils.LengthDir_X(1, angle), Utils.LengthDir_Y(1, angle));
                    wanderingTimeRemain = wanderingTime + Nez.Random.Range(0, 1f);
                }
                if (collisionResult.Normal != Vector2.Zero)
                    wanderDirection = Vector2.Reflect(wanderDirection, collisionResult.Normal);
                movement = wanderDirection;

                if (wanderRemain <= 0)
                {
                    wanderingTimeRemain = Math.Max(0, wanderingTimeRemain - Time.DeltaTime);
                    if (wanderingTimeRemain <= 0)
                    {
                        wanderRemain = wanderDelay + Nez.Random.Range(0, 1f);
                        movement = Vector2.Zero;
                        wanderDirection = Vector2.Zero;
                    }
                }
                wanderRemain = Math.Max(0, wanderRemain - Time.DeltaTime);
            }
            else
            {
                movement = MovementInput * moveSpeed * Time.DeltaTime;
                wanderDirection = Vector2.Zero;
            }

            if (movement != Vector2.Zero)
            {
                _animator.FlipX = !(MovementInput.X > 0);
                _animation = "Run";
                _mover.Move(movement, out collisionResult);
            }
            else
                _animation = "Idle";
            // Handling Random Wandering


            // Handling Attack
            attackDelayRemain = Math.Max(0, attackDelayRemain - Time.DeltaTime);
            if (directionToPlayer.Length() <= 75f)
            {
                if (attackDelayRemain <= 0f)
                {
                    _weapon.Attack(_weapon.aimingDirection);
                    attackDelayRemain = attackDelay;
                    IsAttacking = true;
                }
                else
                {
                    IsAttacking = false;
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
                _animator.Material.Effect = GameManager.HitFlashEffect;
                CollisionResult res;
                if (_mover.Move(Vector2.Normalize(hitMotion) * enemyHealth / GameManager.WeaponsList[hitBy].hitPoint * knockbackMag * knockbackRemain / knockbackLength, out res))
                    hitMotion = Vector2.Reflect(hitMotion, res.Normal);
                knockbackRemain = Math.Max(0, knockbackRemain - Time.DeltaTime);
            }
        }

        void EnemyState_Dead()
        {
            if (_animator.CurrentFrame == _animator.CurrentAnimation.Sprites.Length - 1)
                _animator.Pause();
            Flags.UnsetFlag(ref _collider.PhysicsLayer, (int)PhysicsLayers.Enemy);
            if (knockbackRemain > 0f)
            {
                float strength = GameManager.WeaponsList[hitBy].hitPoint / 10 * knockbackMag * knockbackRemain / knockbackLength;
                CollisionResult res;
                Debug.DrawLine(Entity.Position, Entity.Position + hitMotion * 100f, Color.White, 5f);
                if (_mover.Move(Vector2.Normalize(hitMotion) * strength, out res))
                    hitMotion = Vector2.Reflect(hitMotion, res.Normal);
            }
            else
            { 
                Entity.GetComponent<BoxCollider>().Enabled = false;
            }
            knockbackRemain = Math.Max(0, knockbackRemain - Time.DeltaTime);
            _animator.Color = Color.Gray;

            // disable entity leaving only the corpse
            Entity.GetComponent<EnemyAI>().Enabled = false; 
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
            if (other.Entity.Name != (string)(local.Entity.Name + "Projectile") && check != other.Entity.Id && enemyHealth > 0)
            {
                hitBy = other.Entity.GetComponent<Projectile>().WeaponID;
                Hit(other.Entity.GetComponent<Projectile>().Direction, GameManager.WeaponsList[hitBy].hitPoint);
                check = other.Entity.Id;
            }
        }
        
        public void OnTriggerExit(Collider other, Collider local)
        {
        }
    }
}
