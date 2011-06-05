using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Spacestro.Screen
{
    class BackgroundScreen : GameScreen
    {
        ContentManager content;
        Texture2D background1, background2;
        Vector2 cam1_position = Vector2.Zero, cam2_position = Vector2.Zero;
        Vector2 cam_velocity = new Vector2(-1, 0);

        #region Initialization

        public BackgroundScreen()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (this.content == null)
            {
                this.content = new ContentManager(this.ScreenManager.Game.Services, "Content");
            }

            this.background1 = this.content.Load<Texture2D>("bg1");
            this.background2 = this.content.Load<Texture2D>("bg2");
        }

        public override void UnloadContent()
        {
            this.content.Unload();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = this.ScreenManager.SpriteBatch;
            this.ScreenManager.GraphicsDevice.Clear(Color.Black);

            cam1_position += cam_velocity;
            cam2_position += cam_velocity;
            if (cam1_position.X <= -142)
                cam1_position = Vector2.Zero;
            if (cam2_position.X <= -200)
                cam2_position = Vector2.Zero;

            Rectangle fullScreen = new Rectangle(0, 0, this.ScreenManager.GraphicsDevice.Viewport.Width + 200, this.ScreenManager.GraphicsDevice.Viewport.Height);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null,
                Matrix.CreateTranslation(new Vector3(cam1_position * .7f, 0)));

            spriteBatch.Draw(this.background1, Vector2.Zero, fullScreen, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null,
                Matrix.CreateTranslation(new Vector3(cam2_position * .5f, 0)));

            spriteBatch.Draw(this.background2, Vector2.Zero, fullScreen, Color.White);

            spriteBatch.End();
        }
        #endregion
    }
}
