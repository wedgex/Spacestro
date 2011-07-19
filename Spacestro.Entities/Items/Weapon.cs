using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spacestro.Entities.Items
{
    public abstract class Weapon<T> where T: Projectile
    {
        public Player Owner { get; set; }

        public int MaxShots { get; set; }
        public int ShotCount { get; set; }

        public int ProjectileTicks { get; set; }
        

        public bool CanFire()
        {
            return ShotCount < MaxShots;
        }

        public abstract T Fire();
    }
}
