using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spacestro.Framework
{
    public abstract class EntityAction
    {

        public string Name { get;  set; }
        public Entity Entity { get; set; }
        public abstract void Do();

        public EntityAction(string name)
        {
            this.Name = name;
        }
    }
}
