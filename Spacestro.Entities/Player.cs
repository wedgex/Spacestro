using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Spacestro.Cloud.Library;

namespace Spacestro.Entities
{
    public class Player
    {

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Acceleration { get; set; }
        public float TurnSpeed { get; set; }
        public float MaxSpeed { get; set; }
        public float Rotation { get; set; }
        public string Name { get; set; }

        public Player()
        {
            this.Acceleration = 0.2f;
            this.TurnSpeed = 0.1f;
            this.MaxSpeed = 4.0f;
            this.Rotation = 0.0f;
            this.Velocity = Vector2.Zero;
            this.Position = Vector2.Zero;
            this.Name = "";
        }

        public Player(Vector2 position) : this()
        {
            this.Acceleration = 0.2f;
            this.TurnSpeed = 0.1f;
            this.MaxSpeed = 4.0f;
            this.Rotation = 0.0f;
            this.Velocity = Vector2.Zero;
            this.Position = position;
            this.Name = "";
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

        public void Move(Vector2 svrPos, float svrRot)
        {
            this.Position = svrPos;
            this.Rotation = svrRot;
            this.Position += this.Velocity;
        }
    }
}
