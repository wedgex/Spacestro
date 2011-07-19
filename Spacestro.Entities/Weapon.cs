using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Diagnostics;

namespace Spacestro.Entities
{
    public class Weapon
    {
        private int fireCounter = 0;

        public int Id { get; set; }
        public ProjectileType ProjectileType { get; set; }
        public Player Owner { get; set; }
        public Vector2 ProjectileVelocity { get; set; }
        public float ProjectileAcceleration { get; set; }
        public float ProjectileTurnSpeed { get; set; }
        public float ProjectileMaxSpeed { get; set; }
        public float ProjectileRotation { get; set; }
        public int ProjectileTicksAlive { get; set; }
        public int ProjectileMaxTicks { get; set; }
        public int ProjectileTextureId { get; set; }
        public int FireRate { get; set; }

        public Weapon()
        {
            this.ProjectileAcceleration = 1.0f;
            this.ProjectileTurnSpeed = 0.1f;
            this.ProjectileMaxSpeed = 15.0f;
            this.ProjectileRotation = 0.0f;
            this.ProjectileTicksAlive = 0;
            this.ProjectileMaxTicks = 60;
            this.FireRate = 15;
        }

        public Projectile Fire()
        {            
            Projectile projectile;

            switch (this.ProjectileType)
            {
                default:
                    projectile = new Projectile(this.Owner.Position, this.Owner.Rotation, Projectile.GetNextKey(), this.Owner.Name);
                    break;
            }

            this.fireCounter = this.FireRate;
            Debug.WriteLine("Bullet created");
            return projectile;
        }

        public void UpdateFireCount()
        {
            if (this.fireCounter != 0)
                this.fireCounter--;
        }

        public bool CanFire()
        {
            Debug.WriteLine("CanFire: {0}", this.fireCounter == 0);
            return this.fireCounter == 0;
        }
    }

    public enum ProjectileType
    {
        Default
    }   
}
