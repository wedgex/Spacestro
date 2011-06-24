using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Spacestro.Util.Input;
using Microsoft.Xna.Framework.Input;

namespace Spacestro.Screen
{
    class ConnectionScreen : GameScreen
    {
        private StringBuilder input;
        private KeyboardStringBuilder keyboardStringBuilder;

        private DisplayText displayText;
        private DisplayText enterIp;
        private double cursorPulse = 1000;
        private double cursorPulseTime;
        private bool displayCursor = true;

        public ConnectionScreen()
        {
            this.input = new StringBuilder();
            this.keyboardStringBuilder = new KeyboardStringBuilder();            
        }
        
        public override void LoadContent()
        {
            base.LoadContent();
            this.displayText = new DisplayText(new Vector2(this.ScreenManager.GraphicsDevice.Viewport.Width/2, this.ScreenManager.GraphicsDevice.Viewport.Height/2), string.Empty, Color.White, true, this.ScreenManager);
            this.enterIp = new DisplayText(new Vector2(0, 300), "Enter the server's IP address !!!11!!1evelven!", Color.White, true, this.ScreenManager);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void HandleInput(KeyboardState keyboard, MouseState mouse)
        {
            if (keyboard.IsKeyDown(Keys.Enter) && this.input.Length > 0)
            {
                LoadingScreen.Load(this.ScreenManager, true, new SpacestroScreen(this.input.ToString()));
            }
            else if (keyboard.IsKeyDown(Keys.Escape))
            {
                this.ScreenManager.AddScreen(new MainMenuScreen());
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, true, coveredByOtherScreen);

            KeyboardState keyboard = Keyboard.GetState();
            
            this.keyboardStringBuilder.Process(keyboard, gameTime, this.input);

            if (gameTime.TotalGameTime.TotalMilliseconds > this.cursorPulseTime)
            {
                this.displayCursor = true;

                if (gameTime.TotalGameTime.TotalMilliseconds > (this.cursorPulseTime + this.cursorPulse / 2))
                {
                    this.cursorPulseTime += this.cursorPulse;
                }
            }
            else
            {
                this.displayCursor = false;
            }

            this.displayText.Text = this.input.ToString();            
        }

        public override void Draw(GameTime gameTime)
        {            
            this.ScreenManager.SpriteBatch.Begin();

            this.ScreenManager.SpriteBatch.DrawString(this.ScreenManager.Font, this.enterIp.Text, this.enterIp.Location, this.enterIp.Color);
            this.ScreenManager.SpriteBatch.DrawString(this.ScreenManager.Font, (this.displayCursor) ? string.Format("{0}|", this.displayText.Text) : this.displayText.Text, this.displayText.Location, this.displayText.Color);

            this.ScreenManager.SpriteBatch.End();
        }

        
        
        private class DisplayText
        {
            public string Text { 
                get
                {
                    return this.text;
                }
                set
                {
                    if (this.xCentered)
                    {
                        this.location.X = CalculateTextPosition(value);
                    }

                    this.text = value;
                }
            }

            public Vector2 Location 
            { 
                get { return this.location; } 
                set { this.location = value; } 
            }

            public Color Color { get; set; }
            public ScreenManager ScreenManager { get; set; }

            private Vector2 location;
            private bool xCentered;
            private string text;

            public DisplayText (Vector2 location, string text, Color color, bool xCentered, ScreenManager screenManager)
	        {
                this.xCentered = xCentered;
                this.Color = color;
                this.Location =location;
                this.ScreenManager = screenManager;
                this.Text = text;
	        }            

            private float CalculateTextPosition(string text)
            {
                Vector2 stringSize = this.ScreenManager.Font.MeasureString(text);
                int viewportWidth = this.ScreenManager.GraphicsDevice.Viewport.Width;                

                return viewportWidth / 2 - stringSize.X / 2;
            }
        }
    }
}
