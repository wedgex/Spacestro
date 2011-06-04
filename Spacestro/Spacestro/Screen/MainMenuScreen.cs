#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System;
using Spacestro.Screen;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        Texture2D bg1, bg2, title;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        Vector2 cam1_position = Vector2.Zero, cam2_position = Vector2.Zero;
        Vector2 cam_velocity = new Vector2(-1, 0);

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += ExitSelected;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            this.graphicsDevice = this.ScreenManager.GraphicsDevice;
            this.spriteBatch = this.ScreenManager.SpriteBatch;
            // TODO need to share the backgrounds instead of loading them twice
            ContentManager content = this.ScreenManager.Game.Content;
            this.bg1 = content.Load<Texture2D>("bg1");
            this.bg2 = content.Load<Texture2D>("bg2");
            this.title = content.Load<Texture2D>("title");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new SpacestroScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            
        }


        void ExitSelected(object sender, EventArgs e)
        {
            OnCancel();
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Black);
            cam1_position += cam_velocity;
            cam2_position += cam_velocity;
            if (cam1_position.X <= -142)
                cam1_position = Vector2.Zero;
            if (cam2_position.X <= -200)
                cam2_position = Vector2.Zero;

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, 
                Matrix.CreateTranslation(new Vector3(cam1_position * .7f, 0)));

            this.spriteBatch.Draw(this.bg1, Vector2.Zero, new Rectangle(0, 0, this.graphicsDevice.Viewport.Width*2, this.graphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, 
                Matrix.CreateTranslation(new Vector3(cam2_position * .5f, 0)));
            
            this.spriteBatch.Draw(this.bg2, Vector2.Zero, new Rectangle(0, 0, this.graphicsDevice.Viewport.Width*2, this.graphicsDevice.Viewport.Height), Color.White);

            this.spriteBatch.End();

            Vector2 position = new Vector2(this.graphicsDevice.Viewport.Width / 2 - this.title.Width / 2 , 50f);
            this.spriteBatch.Begin();
            this.spriteBatch.Draw(this.title, position, this.title.Bounds, Color.White);
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
