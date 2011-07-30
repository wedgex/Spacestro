using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Framework;
using Spacestro.Entities.Components;
using Microsoft.Xna.Framework;

namespace Spacestro.Entities.Actions
{
    public class DecelerateAction : EntityAction
    {
        public static string NAME = "decelerate";

        public DecelerateAction() : base(NAME) { }

        public override void Do()
        {
            if (this.Entity == null) return;
            PhysicsComponent physics = this.Entity.GetComponent<PhysicsComponent>(PhysicsComponent.NAME);
            SpacialComponent spacial = this.Entity.GetComponent<SpacialComponent>(SpacialComponent.NAME);
            if (physics == null || spacial == null) return;

            if (physics.Velocity.Length() <= 0.1f)
            {
                physics.Velocity = Vector2.Zero;
            }
            else
            {
                Vector2 newVelocity = physics.Velocity;
                newVelocity.Normalize();
                newVelocity.X *= -physics.Acceleration;
                newVelocity.Y *= -physics.Acceleration;
                physics.Velocity += newVelocity;
            }
        }
    }
}
