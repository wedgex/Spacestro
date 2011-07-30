using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Framework;
using Spacestro.Entities.Components;

namespace Spacestro.Entities.Actions
{
    public class MoveAction : Spacestro.Framework.EntityAction
    {
        public static string NAME = "move";

        public MoveAction() : base(NAME) { }

        public override void Do()
        {
            if (this.Entity == null) return;
            SpacialComponent spacial = this.Entity.GetComponent<SpacialComponent>(SpacialComponent.NAME);
            PhysicsComponent physics = this.Entity.GetComponent<PhysicsComponent>(PhysicsComponent.NAME);
            if (spacial == null || physics == null) return;

            spacial.Position += physics.Velocity;
        }
    }
}
