using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Spacestro.Framework;

namespace Spacestro.Entities.Components
{
    public class Physics : Component
    {
        public static string NAME = "physics";

        public Vector2 Velocity { get; set; }

        public Physics() : base(NAME) { }
    }
}
