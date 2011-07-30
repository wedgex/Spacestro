using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Framework;

namespace Spacestro.Entities.Components
{
    public class HealthComponent : Component
    {
        public static string NAME = "health";

        public int HitPoints { get; set; }

        public HealthComponent() : base(NAME) { }        
    }
}
