using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using NezTopDown.Components.AI;
using System;

namespace NezTopDown.Components
{
    public class Player : Component, Nez.IUpdateable
    {
        #region Constants

        private const float knockbackDelay = 0.2f;
        private const float _moveSpeed = 190f;
        
        #endregion

        #region Components

        private SpriteAnimator _animator;
        private BoxCollider _collider;
        private Mover _mover;
        private WeaponHolder _weapon;
        private VirtualIntegerAxis _xAxisInput;
        private VirtualIntegerAxis _yAxisInput;
        private EntityState playerState;

        #endregion

        #region Vars
        
        public bool ChangeWeapon { get; private set; } = false;
        Vector2 weaponKnockback = Vector2.Zero;
        float knockbackDelayRemain = knockbackDelay;

        #endregion

        public override void OnAddedToEntity()
        {
            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Player/Player.atlas");
            
            _animator = Entity.AddComponent<SpriteAnimator>();
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.Play("Idle");
            _animator.SetLayerDepth(LayerDepths.GetLayerDepth(LayerDepths.Sorting.Player));
            _animator.SetMaterial(new Material());

            _weapon = Entity.AddComponent(new WeaponHolder());
            _collider = Entity.AddComponent(new BoxCollider());
            Flags.SetFlag(ref _collider.PhysicsLayer, (int)PhysicsLayers.Player);
            _collider.CollidesWithLayers = 0;
            Flags.SetFlag(ref _collider.CollidesWithLayers, (int)PhysicsLayers.Tile); // tile
            _mover = Entity.AddComponent<Mover>();

            playerState = EntityState.Free;

            SetupInput();
        }

        public override void DebugRender(Batcher batcher)
        {
            base.DebugRender(batcher);
        
            float direction = Utils.PointDirection(Vector2.Zero, _weapon.aimingDirection);
            batcher.DrawCircle(Entity.Transform.Position, 100f, Color.Red);
            for (int i = 0; i < 8; i++)
            {
                float angle = (360 / 8) * i;
                float difference = Math.Abs(Utils.AngleDifference(angle, direction));
                float result = (180 - difference) / 180;
                result = 1 - Math.Abs(result);
                if (result > 0)
                //Vector2 di = new Vector2(Utils.LengthDir_X(result, angle), Utils.LengthDir_Y(result, angle));
                batcher.DrawLine(Entity.Position, Entity.Position + Directions.eightDirections[i] * result * 100f, Color.White);
                //accept only directions at the less than 90 degrees to the target direction
        
            }
        }

        void SetupInput()
        {
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D));

            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));
            _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.W, Keys.S));

        }

        void Nez.IUpdateable.Update()
        {
            switch (playerState)
            {
                case EntityState.Free:
                    PlayerState_Free();
                    break;
                case EntityState.Hit:
                    PlayerState_Hit();
                    break;
            }
        }

        void PlayerState_Free()
        {
            var moveDir = new Vector2(_xAxisInput.Value, _yAxisInput.Value);
            var movement = moveDir * _moveSpeed * Time.DeltaTime + weaponKnockback * knockbackDelayRemain / knockbackDelay;
            var animation = _animator.CurrentAnimationName;

            if (movement != Vector2.Zero)
                animation = "Run";
            else
                animation = "Idle"; 

            _animator.FlipX = !(moveDir.X > 0);
            _mover.Move(movement, out var res);

            var aimingDirection = _weapon.aimingDirection;
            if ((aimingDirection).X < 0)
                _animator.FlipX = false;
            if ((aimingDirection).X > 0)
                _animator.FlipX = true;

            if (!_animator.IsAnimationActive(animation))
                _animator.Play(animation);

            if (knockbackDelayRemain > 0 && weaponKnockback != Vector2.Zero)
            {
                knockbackDelayRemain = Math.Max(0, knockbackDelayRemain - Time.DeltaTime);
                if (knockbackDelayRemain <= 0)
                {
                    weaponKnockback = Vector2.Zero;
                    knockbackDelayRemain = knockbackDelay;
                }
            }

            //Attack
            if (Game1.WeaponsList[_weapon.currentWeapon].type == 1)
            {
                if (Input.LeftMouseButtonPressed)
                    weaponKnockback = _weapon.Attack(Vector2.Normalize(aimingDirection));
            }
            else
            {
                if (Input.LeftMouseButtonDown)
                    weaponKnockback = _weapon.Attack(Vector2.Normalize(aimingDirection));
            }

            if (Input.IsKeyPressed(Keys.F))
                ChangeWeapon = true;
            else
                ChangeWeapon = false;
        }

        void PlayerState_Hit()
        {
            
        }
    }
}
