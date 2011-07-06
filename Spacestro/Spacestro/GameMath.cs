using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacestro
{
    class GameMath
    {
        internal static Matrix getBG1ParallaxTranslation(Viewport viewport, GameCamera cam)
        {
            float parallax = 0.08f;
            return Matrix.CreateTranslation(new Vector3(-cam.Pos * parallax, 0));
            
        }

        internal static Matrix getBG2ParallaxTranslation(Viewport viewport, GameCamera cam)
        {
            float parallax = 0.05f;
            return Matrix.CreateTranslation(new Vector3(-cam.Pos * parallax, 0));
        }
    }
}