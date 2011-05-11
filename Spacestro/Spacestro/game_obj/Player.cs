using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacestro.game_obj
{
    class Player
    {
        public Texture2D playerTexture;
        public Vector2 positionVector;
        public Vector2 velocity;
        public float speed = 0.2f;
        public float turnspeed = 0.1f;
        public float maxspeed = 4.0f;
        public float rotation = 0.0f;
        
        public int Width
        {
            get { return playerTexture.Width; }
        }

        public int Height
        {
            get { return playerTexture.Height; }
        }


        public void Initialize(Texture2D texture, Vector2 position)
        {
            playerTexture = texture;
            positionVector = position;
            velocity = Vector2.Zero;
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, positionVector, null, Color.White, rotation, new Vector2((float)(Width / 2), (float)(Height / 2)), 1f, SpriteEffects.None, 0f);
        }
    }
}
