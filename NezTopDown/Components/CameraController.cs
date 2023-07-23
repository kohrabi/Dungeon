using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NezTopDown.Components
{
    // By Six Dot: https://www.youtube.com/@IxxHATExxBUFFERING
    public class CameraController : SceneComponent
    {
        Transform player;
        Vector2 mousePos, refVel, shakeOffset;
        float cameraDist = 3.5f;
        float smoothTime = 0.2f;
        private static float shakeMag, shakeLength, shakeRemain;
        private static Vector2 shakeVector;
        private static bool shaking;

        public override void OnEnabled()
        {
            player = Scene.FindEntity("player").Transform;
            base.OnEnabled();
        }

        public override void Update()
        {
            // Capture Mouse Position
            mousePos = ScreenToViewportPoint(Input.MousePosition);
            mousePos *= 2;
            mousePos -= Vector2.One;
            float max = 1.15f; // the maximum offset the camera can have
                              // putting this at 1.1f instead of 1f
                              // because Input.MousePosition returns the mouse's position on the whole screen
            if (Math.Abs(mousePos.X) > max || Math.Abs(mousePos.Y) > max)
                mousePos.Normalize();
            
            // Update Target Position
            Vector2 mouseOffset = mousePos * cameraDist;
            shakeOffset = UpdateShake();

            Scene.Camera.Position = player.Position + mouseOffset * 20f + shakeOffset;
            base.Update();
        }

        Vector2 UpdateShake()
        {
            if (!shaking || shakeRemain <= 0)
            {
                shaking = false;
                return Vector2.Zero;
            }

            Vector2 offset = shakeVector * Nez.Random.Range(-shakeRemain, shakeRemain);
            if (shakeVector == Vector2.Zero)
                offset = new Vector2(Nez.Random.Range(-shakeRemain, shakeRemain), Nez.Random.Range(-shakeRemain, shakeRemain));
            shakeRemain = Math.Max(0, shakeRemain - (1 / shakeLength) * shakeMag);
            return offset;
        }

        public static void Shake(Vector2 direction, float magnitude, float length)
        {
            shaking = true;
            shakeVector = direction;
            shakeMag = magnitude; // how far to shake 
            shakeRemain = magnitude; // how far to shake 
            shakeLength = length; // how long
        }

        Vector2 ScreenToViewportPoint(Vector2 position)
        {
            float width = Core.GraphicsDevice.Viewport.Width;
            float height = Core.GraphicsDevice.Viewport.Height;
            return new Vector2(position.X / width, position.Y / height);
        }
    }
}
