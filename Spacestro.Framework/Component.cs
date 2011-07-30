using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spacestro.Framework
{
    public abstract class Component
    {
        public string Name { get; set; }

        public Component(string name)
        {
            this.Name = name;
        }
    }
}
