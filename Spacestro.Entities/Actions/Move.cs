using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Framework;
using Spacestro.Entities.Components;

namespace Spacestro.Entities.Actions
{
    public class Move : Spacestro.Framework.Action
    {
        public static string NAME = "move";

        public Move() : base(NAME) { }

        public override void Do()
        {
            if (this.Entity == null) return;
            Spacial spacial = this.Entity.GetIComponent<Spacial>(Spacial.NAME);
            Physics physics = this.Entity.GetIComponent<Physics>(Physics.NAME);
            if (spacial == null || physics == null) return;

            spacial.Position += physics.Velocity;
        }
    }
}
