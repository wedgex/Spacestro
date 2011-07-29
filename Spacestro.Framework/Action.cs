using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spacestro.Framework
{
    public class Action
    {
        public string Name { get; set; }
        public Entity Entity { get; set; }
        public virtual void Do() { }
    }
}
