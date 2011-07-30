using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Framework;
using Spacestro.Entities.Components;
using Microsoft.Xna.Framework;

namespace Spacestro.Entities.Actions
{
    public class AccelerateAction : EntityAction
    {
        public static string NAME = "accelerate";

        public AccelerateAction() : base(NAME) { }

        public override void Do()
        {
            if (this.Entity == null) return;
            PhysicsComponent physics = this.Entity.GetComponent<PhysicsComponent>(PhysicsComponent.NAME);
            SpacialComponent spacial = this.Entity.GetComponent<SpacialComponent>(SpacialComponent.NAME);
            if (physics == null || spacial == null) return;

            Vector2 tempVelocity = new Vector2(physics.Acceleration * (float)Math.Cos(spacial.Rotation), physics.Acceleration * (float)Math.Sin(spacial.Rotation));
            tempVelocity += physics.Velocity;
            
            if (tempVelocity.Length() >= physics.MaxSpeed)
            {
                tempVelocity.Normalize();
                tempVelocity = new Vector2(tempVelocity.X * physics.MaxSpeed, tempVelocity.Y * physics.MaxSpeed);
            }

            physics.Velocity = tempVelocity;
        }
    }
}
