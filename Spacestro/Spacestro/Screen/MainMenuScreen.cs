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
using System.Threading;
using Spacestro;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        Texture2D title;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;


        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
            // Create our menu entries.
            MenuEntry singleplayerGameMenuEntry = new MenuEntry("Singleplayer");
            MenuEntry multiplayerGameMenuEntry = new MenuEntry("Multiplayer");            
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            singleplayerGameMenuEntry.Selected += SingleplayerGameMenuEntrySelected;
            multiplayerGameMenuEntry.Selected += MultiplayerGameMenuEntrySelected;            
            exitMenuEntry.Selected += ExitSelected;

            // Add entries to the menu.
            MenuEntries.Add(singleplayerGameMenuEntry);
            MenuEntries.Add(multiplayerGameMenuEntry);            
            MenuEntries.Add(exitMenuEntry);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            this.graphicsDevice = this.ScreenManager.GraphicsDevice;
            this.spriteBatch = this.ScreenManager.SpriteBatch;
            // TODO need to share the backgrounds instead of loading them twice
            ContentManager content = this.ScreenManager.Game.Content;            
            this.title = content.Load<Texture2D>("title");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        #endregion

        #region Handle Input

        void SingleplayerGameMenuEntrySelected(object sender, EventArgs e)
        {
            // start a thread with the server
            Spacestro.Cloud.Cloud cloud = new Spacestro.Cloud.Cloud("spacestro", 8383);
            ThreadStart serverStart = new ThreadStart(cloud.Start);
            Thread serverThread = new Thread(serverStart);
            serverThread.Name = "Single Player Server";
            serverThread.Start();
            // create cloudMessenger
            CloudMessenger messenger = new CloudMessenger("spacestro", "localhost");

            // HACK: ghettofabulous.
            while (!messenger.Connected) ;

            // launch the game with the cloud messenger
            LoadingScreen.Load(this.ScreenManager, true, new SpacestroScreen(messenger, spServer:cloud));            
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void MultiplayerGameMenuEntrySelected(object sender, EventArgs e)
        {
            this.ScreenManager.AddScreen(new ConnectionScreen());            
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
            Vector2 position = new Vector2(this.graphicsDevice.Viewport.Width / 2 - this.title.Width / 2 , 50f);
            this.spriteBatch.Begin();
            this.spriteBatch.Draw(this.title, position, this.title.Bounds, Color.White);
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
