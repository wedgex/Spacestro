using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spacestro.Cloud.Library;
using Spacestro.Entities;
using System.Threading;

namespace Spacestro.Screen
{
    class SpacestroScreen : GameScreen
    {

        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;

        Player player;
        Texture2D bg1, bg2, asteroid, playerTexture, bulletTexture, ufoTexture;
        SpriteFont font, font_S;
        Viewport viewport;
        GameCamera cam;
        Thread spServerThread;


        int worldWidth = 2000;
        int worldHeight = 2000;
        
        InputState inputState;

        private Cloud.Cloud spServer;
        private CloudMessenger cloudMessenger;

        public SpacestroScreen(CloudMessenger messenger, Thread spServerThread = null, Spacestro.Cloud.Cloud spServer = null)
        {
            this.cloudMessenger = messenger;
            this.spServerThread = spServerThread;
            this.spServer = spServer;
        }

        #region Load and Unload Content
        public override void LoadContent()
        {            
            ContentManager content = this.ScreenManager.Game.Content;

            this.player = new Player();
            
            this.graphicsDevice = this.ScreenManager.GraphicsDevice;
            this.spriteBatch = this.ScreenManager.SpriteBatch;

            viewport = graphicsDevice.Viewport;
            
            bg1 = content.Load<Texture2D>("bg1");
            bg2 = content.Load<Texture2D>("bg2");
            asteroid = content.Load<Texture2D>("asteroid");
            playerTexture = content.Load<Texture2D>("player");
            bulletTexture = content.Load<Texture2D>("bullet");
            ufoTexture = content.Load<Texture2D>("UFO");
            
            font = content.Load<SpriteFont>("Orbitron");
            font_S = content.Load<SpriteFont>("Orbitron_S");

            player.Initialize(playerTexture);
            
            cam = new GameCamera(player.Position, viewport, worldWidth, worldHeight);
            cam.Pos = this.player.Position;
        }

        public override void UnloadContent()
        {
            this.cloudMessenger.Stop();
            if (this.spServer != null)
            {
                this.spServer.Stop();
            }            
        }
        #endregion

        #region Update and Draw
        public override void HandleInput(KeyboardState keyboard, MouseState mouse)
        {
            inputState.resetStates();

            if (keyboard.IsKeyDown(Keys.Left))
            {
                //this.player.TurnLeft();
                inputState.Left = true;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                //this.player.TurnRight();
                inputState.Right = true;
            }
            if (keyboard.IsKeyDown(Keys.Up))
            {
                //this.player.Accelerate();
                inputState.Up = true;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                //this.player.Decelerate();
                inputState.Down = true;
            }
            if (keyboard.IsKeyDown(Keys.Space))
            {
                //this.player.Decelerate();
                inputState.Space = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Black);

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
                        null, null, null, null, this.cam.getTransformation());

            foreach (Player p in this.cloudMessenger.GameController.playerList)
            {
                if (p.Name.Equals(this.cloudMessenger.ClientID))  // it's us
                {
                    // draw player
                    this.player.Draw(spriteBatch);

                    // draw something above player
                    spriteBatch.DrawString(font_S, "Hit: " + p.hitCount.ToString(), this.player.Position + new Vector2(-40, -40), Color.PeachPuff);
                }
                else // it's someone else
                {
                    spriteBatch.Draw(playerTexture, p.getNextLerpPosition(), null, Color.White, p.Rotation, new Vector2((float)(playerTexture.Width / 2), (float)(playerTexture.Height / 2)), 1f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font_S, "Hit: " + p.hitCount.ToString(), p.Position + new Vector2(-40, -40), Color.PeachPuff);
                }
            }

            foreach (Projectile proj in this.cloudMessenger.GameController.projectiles)
            {
                if (proj.Active)
                {
                    spriteBatch.Draw(bulletTexture, proj.getNextLerpPosition(), null, Color.White, proj.getNextLerpRotation(), new Vector2((float)(bulletTexture.Width / 2), (float)(bulletTexture.Height / 2)), 1f, SpriteEffects.None, 0f);
                    proj.TicksAlive++;
                    if (proj.TicksAlive >= 120)
                    {
                        proj.Active = false;
                    }
                }
            }

            foreach (Enemy en in this.cloudMessenger.GameController.enemies)
            {
                spriteBatch.Draw(ufoTexture, en.getNextLerpPosition(), null, Color.White, 0, new Vector2((float)(ufoTexture.Width / 2), (float)(ufoTexture.Height / 2)), 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.DrawString(font_S, "Spacestro alpha (milestone 2)", this.cam.Pos - new Vector2(0.5f * viewport.Width, -0.38f * viewport.Height), Color.Yellow);

            spriteBatch.Draw(asteroid, new Vector2(600, 600), new Rectangle(0, 0, asteroid.Width, asteroid.Height), Color.White);

            spriteBatch.End();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            HandleNetworkOut();
            HandleNetworkIn();
            HandlePlayerMoving();

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion
        
        protected void HandleNetworkOut()
        {
            if (inputState.HasKeyDown())
            {
                this.cloudMessenger.SendMessage(inputState);
            }
        }

        protected void HandleNetworkIn()
        {
            this.cloudMessenger.CheckForNewMessages();
        }

        protected void HandlePlayerMoving()
        {
            // we're still updating our local player entity since we need this position to update camera
            if (this.cloudMessenger.GameController.getPlayer(this.cloudMessenger.ClientID) != null)
            {
                this.player.Move(this.cloudMessenger.GameController.getPlayer(this.cloudMessenger.ClientID).getNextLerpPosition(), this.cloudMessenger.GameController.getPlayer(this.cloudMessenger.ClientID).getNextLerpRotation());
                this.cam.Pos = this.player.Position;
            }
        }
    }
}
