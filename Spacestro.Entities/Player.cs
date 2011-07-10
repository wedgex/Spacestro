using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Spacestro.Cloud.Library;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Spacestro.Entities
{
    public class Player
    {
        public float currentSmoothing = 0;
        public float framesBetweenUpdates = 3;

        public Vector2 Position { get; set; }
        public Vector2 PrevPosition { get; set; }
        public Vector2 Velocity { get; set; }
        public float Acceleration { get; set; }
        public float TurnSpeed { get; set; }
        public float MaxSpeed { get; set; }
        public float Rotation { get; set; }
        public float PrevRotation { get; set; }
        public string Name { get; set; }  // holds client id at the moment
        public int FireRate { get; set; }
        public int firecounter = 0;
        public int hitCount = 0;

        public Dictionary<string, int> collidedWith = new Dictionary<string,int>();
        private List<string> removeCollidedList = new List<string>();

        public Texture2D playerTexture;

        public Player()
        {
            this.Acceleration = 0.2f;
            this.TurnSpeed = 0.1f;
            this.MaxSpeed = 8.0f;
            this.Rotation = 0.0f;
            this.Velocity = Vector2.Zero;
            this.Name = "";
            this.FireRate = 15;

            // need to get this from network instead 
            //(doesn't matter right now since network grabs uses this default as well)
            this.Position = new Vector2(400, 400);
        }

        public Player(Vector2 position)
            : this()
        {
            this.Position = position;
        }

        public int Width
        {
            get { return playerTexture.Width; }
        }

        public int Height
        {
            get { return playerTexture.Height; }
        }

        public void getHit()
        {
            hitCount++;
        }

        public void Initialize(Texture2D texture)
        {
            playerTexture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, this.Position, null, Color.White, this.Rotation, new Vector2((float)(Width / 2), (float)(Height / 2)), 1f, SpriteEffects.None, 0f);
        }

        public void handleInputState(InputState inState)
        {
            if (inState.Up)
                this.Accelerate();
            if (inState.Down)
                this.Decelerate();
            if (inState.Left)
                this.TurnLeft();
            if (inState.Right)
                this.TurnRight();
        }

        /// <summary>
        /// Increases the ship's velocity by it's acceleration value up to it's max speed.
        /// </summary>
        public void Accelerate()
        {
            Vector2 tempVelocity = new Vector2(this.Acceleration * (float)Math.Cos(this.Rotation), this.Acceleration * (float)Math.Sin(this.Rotation));
            tempVelocity += this.Velocity;
            if (tempVelocity.Length() >= this.MaxSpeed)
            {
                tempVelocity.Normalize();
                tempVelocity = new Vector2(tempVelocity.X * this.MaxSpeed, tempVelocity.Y * this.MaxSpeed);
            }

            this.Velocity = tempVelocity;
        }

        /// <summary>
        /// Reduces the ship's velocity by it's acceleration value to a minimum of 0.
        /// </summary>
        public void Decelerate()
        {

            if (this.Velocity.Length() <= 0.1f)
            {
                this.Velocity = Vector2.Zero;
                return;
            }
            Vector2 newV = this.Velocity;
            newV.Normalize();
            this.Velocity += new Vector2(-this.Acceleration * newV.X, -this.Acceleration * newV.Y);

        }

        /// <summary>
        /// Turn the ship left by it's turn speed.
        /// </summary>
        public void TurnLeft()
        {
            this.Rotation -= this.TurnSpeed;
        }

        /// <summary>
        /// Turn the ship righ by it's turn speed.
        /// </summary>
        public void TurnRight()
        {
            this.Rotation += this.TurnSpeed;
        }

        /// <summary>
        /// Increases the players position by it's velocity.
        /// </summary>
        public void Move()
        {
            this.Position += this.Velocity;
        }

        public void Move(Vector2 Pos, float Rot)
        {
            this.Position = Pos;
            this.Rotation = Rot;
        }

        public void setSvrPosRot(float svr_x, float svr_y, float svr_rot)
        {
            this.PrevPosition = this.Position;
            this.Position = new Vector2(svr_x, svr_y);
            this.PrevRotation = this.Rotation;
            this.Rotation = svr_rot;
            this.currentSmoothing = 1;
        }

        public Vector2 getNextLerpPosition()
        {
            this.currentSmoothing -= 1.0f / this.framesBetweenUpdates;
            if (this.currentSmoothing < 0)
                this.currentSmoothing = 0;
            return Vector2.Lerp(this.Position, this.PrevPosition, currentSmoothing);
        }

        public float getNextLerpRotation()
        {
            return MathHelper.Lerp(this.Rotation, this.PrevRotation, currentSmoothing);
        }

        public bool canCollide(string str)
        {
            if (collidedWith.ContainsKey(str))
            {
                return false;
            }
            else { return true; }
        }

        public bool canShoot()
        {
            if (this.firecounter == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void tickDownCollidedWithList()
        {
            if (collidedWith.Count != 0)
            {
                for (int i = 0; i < collidedWith.Count; i++)
                {
                    string key = collidedWith.ElementAt(i).Key;
                    collidedWith[key]--;

                    if (collidedWith[key] == 0)
                    {
                        removeCollidedList.Add(key);
                    }
                }

                if (removeCollidedList.Count != 0)
                {
                    foreach (string str in removeCollidedList)
                    {
                        collidedWith.Remove(str);
                    }
                }
            }
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)(this.Position.X - (25/2)), (int)(this.Position.Y - (20/2)), 25, 20);
        }
    }
}
