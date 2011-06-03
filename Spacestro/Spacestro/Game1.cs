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

namespace Spacestro
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Spacestro.game_obj.Player player;
        Viewport viewport;
        GameCamera cam;
        int worldWidth = 2000;
        int worldHeight = 2000;
        int windowHeight = 768;
        int windowWidth = 1024;

        Texture2D bg1, bg2, asteroid, playerTexture;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        InputState state;

        private CloudMessenger cloudMessenger;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
            Content.RootDirectory = "Content";

            this.cloudMessenger = new CloudMessenger("spacestro");
        }

        protected override void Initialize()
        {
            player = new Spacestro.game_obj.Player();
            viewport = graphics.GraphicsDevice.Viewport;
            cam = new GameCamera(player.Position, viewport, worldWidth, worldHeight);
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
            playerTexture = Content.Load<Texture2D>("player");

            font = Content.Load<SpriteFont>("SpriteFont");
        }

        protected override void Update(GameTime gameTime)
        {
            // this.Exit();
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            HandleKeyboardInput();
            HandleNetworkOut();
            HandleNetworkIn();
            HandlePlayerMoving();
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // first batch containing bg1
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                        SamplerState.LinearWrap, null, null, null, GameMath.getBG1ParallaxTranslation(viewport, cam));

            spriteBatch.Draw(bg1, Vector2.Zero, new Rectangle(0, 0, worldWidth, worldHeight), Color.White);
            spriteBatch.End();

            // second batch cotaining bg2
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                        SamplerState.LinearWrap, null, null, null, GameMath.getBG2ParallaxTranslation(viewport, cam));

            spriteBatch.Draw(bg2, Vector2.Zero, new Rectangle(0, 0, worldWidth, worldHeight), Color.White);
            spriteBatch.End();

            // third batch containing player and entities
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                        null, null, null, null, cam.getTransformation());

            foreach (Spacestro.Entities.Player p in this.cloudMessenger.playerList)
            {
                if (p.Name.Equals(this.cloudMessenger.client_id))  // it's us
                { 
                    player.Draw(spriteBatch);
                    spriteBatch.DrawString(font,( "x:" + p.Position.X + " y:" + p.Position.Y), player.Position + new Vector2(-40,-40), Color.PeachPuff);
                }
                else // it's someone else
                {
                    spriteBatch.Draw(playerTexture, p.Position, null, Color.White, p.Rotation, new Vector2((float)(playerTexture.Width / 2), (float)(playerTexture.Height / 2)), 1f, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.DrawString(font, "<poemdexter> I'm a big ol' chat thing.", cam.Pos - new Vector2(0.5f * viewport.Width, -0.38f * viewport.Height), Color.Yellow);

            spriteBatch.Draw(asteroid, new Vector2(600, 600), new Rectangle(0, 0, asteroid.Width, asteroid.Height), Color.White);
            spriteBatch.End();


            base.Draw(gameTime);
        }

        protected void HandleKeyboardInput()
        {
            state.resetStates();

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                //this.player.TurnLeft();
                state.Left = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                //this.player.TurnRight();
                state.Right = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                //this.player.Accelerate();
                state.Up = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                //this.player.Decelerate();
                state.Down = true;
            }
        }

        protected void HandleNetworkOut()
        {
            if (state.HasKeyDown())
            {
                this.cloudMessenger.SendMessage(state);
            }
        }

        protected void HandleNetworkIn()
        {
            this.cloudMessenger.CheckForNewMessages();
        }

        protected void HandlePlayerMoving()
        {
            // we're still updating our local player entity since we need this position to update camera
            if (this.cloudMessenger.getPlayer(this.cloudMessenger.client_id) != null)
            {
                this.player.Move(this.cloudMessenger.getPlayer(this.cloudMessenger.client_id).getNextLerpPosition(), this.cloudMessenger.getPlayer(this.cloudMessenger.client_id).getNextLerpRotation());
                this.cam.Pos = this.player.Position;
            }
        }

        protected override void UnloadContent() { }
    }
}
