using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spacestro.Entities
{
    public class Shield
    {
        public int MaxValue { get; set; }
        public int CurrentValue { get; set; }
        public int RegenTicksNeeded { get; set; }
        public int RegenRate { get; set; }
        public int DamageReduction { get; set; }

        public Shield(int maxValue, int regenRate, int dmgRedux) 
        {
            this.MaxValue = maxValue;
            this.RegenRate = regenRate;
            this.DamageReduction = dmgRedux;
            this.CurrentValue = MaxValue;
            this.RegenTicksNeeded = 0;
        }

        // takes incoming damage and returns what we'll actually take
        public int absorbDamage(int incoming)
        {
            if (this.CurrentValue != 0) // out of shield
            {
                int dmg = incoming - (this.DamageReduction * this.CurrentValue);

                if (dmg >= 0) // more damage than shield
                {
                    this.resetRegen();
                    this.CurrentValue = 0;
                    return dmg;
                }
                else if (dmg > -this.DamageReduction) // equal damage and shield
                {
                    this.resetRegen();
                    this.CurrentValue = 0;
                    return 0;
                }
                else // shield absorbed all with shield left over
                {
                    this.resetRegen();
                    CurrentValue -= (int)Math.Ceiling((double)(dmg / DamageReduction));
                    return 0;
                }
            }
            return incoming;
        }

        private void resetRegen()
        {
            this.RegenTicksNeeded = this.RegenRate;
        }

        private void tickRegen()
        {
            if (this.RegenTicksNeeded != 0)
            {
                this.RegenTicksNeeded--;
            }
        }
    }
}
