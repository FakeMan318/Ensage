﻿namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using HpMpAbuse.Helpers;

    internal sealed class UrnOfShadows : UsableItem
    {
        #region Constructors and Destructors

        public UrnOfShadows(string name)
            : base(name)
        {
            HealthRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "soul_heal_amount").Value;
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Menu.Recovery.IsEnabled(Name) && Item.CurrentCharges > 0
                   && !Hero.HasModifier(Modifiers.UrnRegeneration) && Hero.MaximumHealth - Hero.Health >= HealthRestore;
        }

        public override ItemsStats.Stats GetDropItemStats()
        {
            return ItemsStats.Stats.Health;
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Agility;
        }

        public override void Use(bool queue = true)
        {
            Item.UseAbility(Hero, queue);
            Sleeper.Sleep(1000, Name);
            Sleeper.Sleep(300, "Used");
        }

        #endregion
    }
}