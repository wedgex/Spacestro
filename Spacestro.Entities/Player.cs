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
    public class Player : Entity
    {
        private Weapon weapon;

        public Weapon Weapon 
        {
            get
            {
                return weapon;
            }
            set
            {
                // equip the weapon
                value.Owner = this;
                this.weapon = value;
            }
        }

        public string Name { get; set; }  // holds client id at the moment
        
        public int hitCount = 0;

        public Dictionary<string, int> collidedWith = new Dictionary<string,int>();
        private List<string> removeCollidedList = new List<string>();

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
        public void getHit()
        {
            hitCount++;
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

        public void TurnLeft()
        {
            this.Rotation -= this.TurnSpeed;
        }


        public void TurnRight()
        {
            this.Rotation += this.TurnSpeed;
        }

        public bool canCollide(string str)
        {
            if (collidedWith.ContainsKey(str))
            {
                return false;
            }
            else { return true; }
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

        public Projectile Fire()
        {
            return this.weapon.Fire();
        }

        public bool CanFire()
        {
             return (this.Weapon != null) ? this.Weapon.CanFire() : false;
        }
    }
}
