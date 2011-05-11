using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Spacestro.game_obj;

namespace Spacestro
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            player = new Player();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width / 2, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(Content.Load<Texture2D>("player"), playerPosition);
        }

        protected override void Update(GameTime gameTime)
        {
            // this.Exit();
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            handleKeyboardInput();
            handlePlayerMoving();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            player.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void handleKeyboardInput()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Left)) // counter-clockwise
            {
                GameMath.turnLeft(player);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right)) // clockwise
            {
                GameMath.turnRight(player);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up)) // speed up
            {
                GameMath.increasePlayerSpeed(player);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down)) // slow down
            {
                GameMath.decreasePlayerSpeed(player);
            }
        }

        protected void handlePlayerMoving() 
        {
            player.positionVector += player.velocity;
        }

        protected override void UnloadContent() { }
    }
}
