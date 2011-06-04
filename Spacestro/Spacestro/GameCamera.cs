using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacestro
{
    public class GameCamera
    {
        private Matrix transform;
        private Vector2 pos, _origin;
        private int viewportWidth;
        private int viewportHeight;
        private int worldWidth;
        private int worldHeight;

        public GameCamera(Vector2 playerPos, Viewport viewport, int _worldWidth,
           int _worldHeight)
        {
            pos = Vector2.Zero;
            viewportWidth = viewport.Width;
            viewportHeight = viewport.Height;
            _origin = new Vector2((float)(.5 * viewport.Width), (float)(.5 * viewport.Height));
            worldWidth = _worldWidth;
            worldHeight = _worldHeight;
        }

        public void Move(Vector2 amount)
        {
            pos += amount;
        }

        public Vector2 Pos
        {
            get { return pos; }
            set
            {
                float leftBarrier = (float)viewportWidth * .5f;
                float rightBarrier = worldWidth - (float)viewportWidth * .5f;
                float topBarrier = worldHeight - (float)viewportHeight * .5f;
                float bottomBarrier = (float)viewportHeight * .5f;
                pos = value;
                if (pos.X < leftBarrier)
                    pos.X = leftBarrier;
                if (pos.X > rightBarrier)
                    pos.X = rightBarrier;
                if (pos.Y > topBarrier)
                    pos.Y = topBarrier;
                if (pos.Y < bottomBarrier)
                    pos.Y = bottomBarrier;
            }
        }

        public Matrix getTransformation()
        {
            transform =
               Matrix.CreateTranslation(new Vector3(-pos, 0)) *
               Matrix.CreateTranslation(new Vector3(_origin, 0));

            return transform;
        }
    }
}
