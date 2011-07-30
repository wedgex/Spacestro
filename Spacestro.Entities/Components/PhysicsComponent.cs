using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Spacestro.Framework;

namespace Spacestro.Entities.Components
{
    public class PhysicsComponent : Component
    {
        public static string NAME = "physics";

        public Vector2 Velocity { get; set; }
        public float TurnSpeed { get; set; }
        public float MaxSpeed { get; set; }
        public float Acceleration { get; set; }

        public PhysicsComponent() : base(NAME) { }
    }
}
