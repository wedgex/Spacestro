using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacestro.Framework;
using Spacestro.Entities.Components;

namespace Spacestro.Entities.Actions
{
    public class TakeDamageAction : EntityAction
    {
        public static string NAME = "take_damage";

        public TakeDamageAction() : base(NAME) { }

        public override void Do()
        {
            if (this.Entity == null) return;
            DamageTakenComponent damage = this.Entity.GetComponent<DamageTakenComponent>(DamageTakenComponent.NAME);
            HealthComponent health = this.Entity.GetComponent<HealthComponent>(HealthComponent.NAME);
            if (damage == null || health == null) return;

            health.HitPoints -= damage.Damage;
            this.Entity.RemoveComponent(damage.Name);
        }
    }
}
