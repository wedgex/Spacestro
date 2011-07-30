using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spacestro.Framework
{
    public abstract class Action
    {

        public string Name { get;  set; }
        public Entity Entity { get; set; }
        public abstract void Do();

        public Action(string name)
        {
            this.Name = name;
        }
    }
}
