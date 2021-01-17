namespace RimEffect
{
    using System;
    using System.Collections.Generic;
    using RimWorld;
    using Verse;

    public class CompAbilitiesApparel : ThingComp
    {
        public CompProperties_AbilitiesApparel Props => (CompProperties_AbilitiesApparel) this.props;

        private Pawn pawn;
        private Pawn Pawn => (this.parent as Apparel)?.Wearer;

        private List<Ability> givenAbilities = new List<Ability>();

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            foreach (AbilityDef abilityDef in this.Props.abilities)
            {
                Ability ability = (Ability)Activator.CreateInstance(abilityDef.abilityClass);
                ability.def    = abilityDef;
                ability.holder = this.parent;

                this.givenAbilities.Add(ability);
            }
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetWornGizmosExtra()) 
                yield return gizmo;

            if (this.Pawn == null)
                yield break;

            if (this.Pawn != this.pawn)
            {
                this.pawn = this.Pawn;
                foreach (Ability ability in this.givenAbilities) 
                    ability.pawn = this.pawn;
            }

            foreach (Ability ability in this.givenAbilities)
                yield return ability.GetGizmo();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref this.givenAbilities, nameof(this.givenAbilities), LookMode.Deep);
        }
    }

    public class CompProperties_AbilitiesApparel : CompProperties
    {
        public List<AbilityDef> abilities;

        public CompProperties_AbilitiesApparel() => 
            this.compClass = typeof(CompAbilitiesApparel);
    }
}