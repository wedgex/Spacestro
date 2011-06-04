using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacestro.game_obj
{
    public class Player : Spacestro.Entities.Player
    {
        public Texture2D playerTexture;
        
        public int Width
        {
            get { return playerTexture.Width; }
        }

        public int Height
        {
            get { return playerTexture.Height; }
        }


        public void Initialize(Texture2D texture)
        {
            playerTexture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, this.Position, null, Color.White, this.Rotation, new Vector2((float)(Width / 2), (float)(Height / 2)), 1f, SpriteEffects.None, 0f);
        }
    }
}
