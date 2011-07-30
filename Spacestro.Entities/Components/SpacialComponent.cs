using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Spacestro.Framework;

namespace Spacestro.Entities.Components
{
    public class SpacialComponent : Component
    {
        public static string NAME = "spacial";

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }

        public SpacialComponent() : base(NAME) { }
    }
}
