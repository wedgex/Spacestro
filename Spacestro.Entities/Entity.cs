using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacestro.Entities
{
    public class Entity
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
        public int Hitpoints { get; set; }
        public Shield EquippedShield { get; set; }

        public int FireRate { get; set; }
        public int firecounter = 0;

        public Texture2D texture;

        // calculates damage taken and takes shield into account
        public int takeDamage(int damage)
        {
            if (this.EquippedShield == null)
            {
                this.Hitpoints -= damage;
                if (this.Hitpoints < 0)
                    this.Hitpoints = 0;
                return this.Hitpoints;
            }
            else
            {
                this.Hitpoints -= this.EquippedShield.absorbDamage(damage);
                if (this.Hitpoints < 0)
                    this.Hitpoints = 0;
                return this.Hitpoints;
            }
        }

        public int Width
        {
            get { return texture.Width; }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        public void Initialize(Texture2D _texture)
        {
            texture = _texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, this.Position, null, Color.White, this.Rotation, new Vector2((float)(Width / 2), (float)(Height / 2)), 1f, SpriteEffects.None, 0f);
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

        public void setSvrPosRot(float svr_x, float svr_y, float svr_rot)
        {
            this.PrevPosition = this.Position;
            this.Position = new Vector2(svr_x, svr_y);
            this.PrevRotation = this.Rotation;
            this.Rotation = svr_rot;
            this.currentSmoothing = 1;
        }

        public void Move()
        {
            this.Position += this.Velocity;
        }


        public void Move(Vector2 Pos, float Rot)
        {
            this.Position = Pos;
            this.Rotation = Rot;
        }

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
    }
}
