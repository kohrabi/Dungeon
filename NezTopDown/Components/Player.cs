using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using System;

namespace NezTopDown.Components
{
    public class Player : Component, Nez.IUpdateable
    {
        float _moveSpeed = 190f;

        SpriteAnimator _animator;
        Mover _mover;
        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;

        public static bool MouseFlip = false;

        public override void OnAddedToEntity()
        {
            var atlas = Entity.Scene.Content.LoadSpriteAtlas("Content/Sprites/Player/Player.atlas");
            
            _animator = Entity.AddComponent<SpriteAnimator>();
            _animator.AddAnimationsFromAtlas(atlas);
            _animator.Play("Idle");
            _animator.LayerDepth = 0;



            Entity.AddComponent(new WeaponHolder());
            var collider = Entity.AddComponent(new BoxCollider());
            collider.PhysicsLayer = 1 << 1;
            collider.CollidesWithLayers = 0;
            Nez.Flags.SetFlag(ref collider.CollidesWithLayers, 2); // tile
            _mover = Entity.AddComponent<Mover>();

            SetupInput();
        }

        void SetupInput()
        {
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));

        }

        float delay = 1f;
        float _remainingDelay = 1f;
        void Nez.IUpdateable.Update()
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
            if (_remainingDelay > 0)
                _remainingDelay -= Time.DeltaTime;
            if (Input.LeftMouseButtonPressed && _remainingDelay <= 0)
            {
                Entity.GetComponent<WeaponHolder>().Attack(Vector2.Normalize(aimingDirection));
                _remainingDelay = delay;
            }
        }
    }
}
