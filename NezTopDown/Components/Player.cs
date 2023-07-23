using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using System;

namespace NezTopDown.Components
{
    public enum EntityState
    {
        Free,
        Hit
    }

    public class Player : Component, Nez.IUpdateable
    {
        float _moveSpeed = 190f;

        SpriteAnimator _animator;
        BoxCollider _collider;
        Mover _mover;
        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;

        public bool ChangeWeapon { get; private set; } = false; 

        EntityState playerState;

        public override void OnAddedToEntity()
        {
            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Player/Player.atlas");
            
            _animator = Entity.AddComponent<SpriteAnimator>();
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.Play("Idle");
            _animator.LayerDepth = 0;
            //_animator.SetMaterial(new Material(_effect));
            //_animator.Material.Effect.Parameters["overlayColor"].SetValue(Color.White.ToVector4());

            Entity.AddComponent(new WeaponHolder());
            _collider = Entity.AddComponent(new BoxCollider());
            _collider.PhysicsLayer = 1 << 1;
            _collider.CollidesWithLayers = 0;
            Nez.Flags.SetFlag(ref _collider.CollidesWithLayers, 2); // tile
            _mover = Entity.AddComponent<Mover>();

            playerState = EntityState.Free;

            SetupInput();
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

        const float delay = 0.4f;
        float _remainingDelay = delay;
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
            var animation = _animator.CurrentAnimationName;

            if (moveDir != Vector2.Zero)
            {
                var movement = moveDir * _moveSpeed * Time.DeltaTime;
                _animator.FlipX = !(moveDir.X > 0);
                animation = "Run";
                _mover.CalculateMovement(ref movement, out var res);
                _mover.ApplyMovement(movement);
            }
            else
            {
                animation = "Idle";
            }

            var aimingDirection = Entity.GetComponent<WeaponHolder>().aimingDirection;
            if ((aimingDirection).X < 0)
                _animator.FlipX = false;
            if ((aimingDirection).X > 0)
                _animator.FlipX = true;

            if (!_animator.IsAnimationActive(animation))
                _animator.Play(animation);
            //Attack
            if (Input.LeftMouseButtonPressed)
                Entity.GetComponent<WeaponHolder>().Attack(Vector2.Normalize(aimingDirection));


            if (Input.IsKeyPressed(Keys.F))
                ChangeWeapon = true;
            else
                ChangeWeapon = false;
        }

        void PlayerState_Hit()
        {
            throw new NotImplementedException();
        }
    }
}
