using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacestro.Entities
{
    public class Enemy
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
        public int ID { get; set; }
        public int FireRate { get; set; }
        public int firecounter = 0;
        public string TargetPlayer { get; set; }

        public Texture2D ufoTexture;
        public bool Active;

        public Enemy()
        {
            this.Acceleration = 0.2f;
            this.Velocity = Vector2.Zero;
            this.TurnSpeed = 0.1f;
            this.MaxSpeed = 5.0f;
            this.Rotation = 0.0f;
            this.FireRate = 15;
            this.TargetPlayer = "";
            this.Active = true;
        }

        public Enemy(Vector2 position, int id)
            : this()
        {
            this.Position = position;
            this.ID = id;
        }

        public void getHit()
        {
            this.Active = false;
        }

        public int Width
        {
            get { return ufoTexture.Width; }
        }

        public int Height
        {
            get { return ufoTexture.Height; }
        }

        public void Initialize(Texture2D texture)
        {
            ufoTexture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ufoTexture, this.Position, null, Color.White, this.Rotation, new Vector2((float)(Width / 2), (float)(Height / 2)), 1f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Increases the bullet's velocity by it's acceleration value up to it's max speed.
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
        /// Reduces the bullet's velocity by it's acceleration value to a minimum of 0.
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
        /// Increases the bullet's position by it's velocity.
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

        public Rectangle getRectangle()
        {
            return new Rectangle((int)this.Position.X, (int)this.Position.Y, 35, 35);
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
    }
}
