using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Framework;

namespace Spacestro.Entities.Components
{
    public class DamageTakenComponent : Component
    {
        public static string NAME = "damage_taken";

        public int Damage { get; set; }

        public DamageTakenComponent() : base(NAME) { }
    }
}
