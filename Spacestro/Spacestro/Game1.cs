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
using System.Net.Sockets;
using System.Text;

namespace Spacestro
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Viewport viewport;
        GameCamera cam;
        int worldWidth = 2000;
        int worldHeight = 2000;
        int windowHeight = 768;
        int windowWidth = 1024;

        Texture2D bg1, bg2, asteroid;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            player = new Player();
            viewport = graphics.GraphicsDevice.Viewport;
            cam = new GameCamera(viewport, worldWidth, worldHeight);
            cam.Pos = this.player.Position;            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width / 2, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(Content.Load<Texture2D>("player"), playerPosition);
            bg1 = Content.Load<Texture2D>("bg1");
            bg2 = Content.Load<Texture2D>("bg2");
            asteroid = Content.Load<Texture2D>("asteroid");
        }

        protected override void Update(GameTime gameTime)
        {
            // this.Exit();
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            HandleKeyboardInput();
            HandlePlayerMoving();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // first batch containing bg1
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                        SamplerState.LinearWrap, null, null, null, GameMath.getBG1ParallaxTranslation(viewport, cam));

            spriteBatch.Draw(bg1, Vector2.Zero, new Rectangle(0,0, worldWidth, worldHeight), Color.White);
            spriteBatch.End();
            
            // second batch cotaining bg2
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                        SamplerState.LinearWrap, null, null, null, GameMath.getBG2ParallaxTranslation(viewport, cam));

            spriteBatch.Draw(bg2, Vector2.Zero, new Rectangle(0,0, worldWidth, worldHeight), Color.White);
            spriteBatch.End();

            // third batch containing player
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, 
                        null, null, null,null, cam.getTransformation());

            
            player.Draw(spriteBatch);
            spriteBatch.Draw(asteroid, new Vector2(600, 600), new Rectangle(0, 0, asteroid.Width, asteroid.Height), Color.White);
            spriteBatch.End();

            
            base.Draw(gameTime);
        }

        protected void HandleKeyboardInput()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A))
            {
                this.player.TurnLeft();                
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D)) 
            {
                this.player.TurnRight();                
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W)) 
            {
                this.player.Accelerate();                              
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S)) 
            {
                this.player.Decelerate();                
            }
        }

        protected void HandlePlayerMoving() 
        {
            this.player.Move();
            this.cam.Pos = this.player.Position;
        }

        void Server_MessageRecieved(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void UnloadContent() { }
    }
}
