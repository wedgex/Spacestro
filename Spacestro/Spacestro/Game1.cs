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
using System.Net.Sockets;
using System.Text;
using Lidgren.Network;
using Spacestro.Cloud.Library;
using Spacestro.game_obj;
using Spacestro.Entities;
using GameStateManagement;

namespace Spacestro
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ScreenManager screenManager;

        int worldWidth = 2000;
        int worldHeight = 2000;
        int windowHeight = 768;
        int windowWidth = 1024;                                       
                
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
            Content.RootDirectory = "Content";            

            // Create the screen manager and add the title screen
            this.screenManager = new ScreenManager(this);

            this.Components.Add(this.screenManager);

            this.screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void Initialize()
        {            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);            
        }

        protected override void Update(GameTime gameTime)
        {
            // this.Exit();            
            
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {            
            base.Draw(gameTime);
        }

        protected override void UnloadContent() { }
    }
}
