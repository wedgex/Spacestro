using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacestro
{
    class GameMath
    {
        internal static Matrix getBG1ParallaxTranslation(Viewport viewport, GameCamera cam)
        {
            float parallax = 0.07f;
            Matrix transform =
               Matrix.CreateTranslation(new Vector3(-cam.Pos * parallax, 0));
               //* Matrix.CreateTranslation(new Vector3(new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f), 0));
            return transform;
        }

        internal static Matrix getBG2ParallaxTranslation(Viewport viewport, GameCamera cam)
        {
            float parallax = 0.05f;
            Matrix transform =
               Matrix.CreateTranslation(new Vector3(-cam.Pos * parallax, 0));
               //* Matrix.CreateTranslation(new Vector3(new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f), 0));
            return transform;
        }
    }
}