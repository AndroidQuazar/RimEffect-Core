﻿namespace RimEffect
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
                ability.Init();
                this.givenAbilities.Add(ability);
            }
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetWornGizmosExtra()) 
                yield return gizmo;

            if (this.Pawn == null)
                yield break;

            if (!this.Pawn.IsColonistPlayerControlled || !this.Pawn.Drafted)
                yield break;

            if (this.Pawn != this.pawn)
            {
                this.pawn = this.Pawn;
                foreach (Ability ability in this.givenAbilities)
                {
                    ability.pawn = this.pawn;
                    ability.Init();
                }
            }

            foreach (Ability ability in this.givenAbilities) 
                yield return ability.GetGizmo();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref this.givenAbilities, nameof(this.givenAbilities), LookMode.Deep);

            if (this.givenAbilities == null)
                this.givenAbilities = new List<Ability>();
            else if(Scribe.mode == LoadSaveMode.LoadingVars)
            {
                foreach (Ability ability in this.givenAbilities)
                {
                    ability.holder = this.parent;
                }
            }
        }
    }

    public class CompProperties_AbilitiesApparel : CompProperties
    {
        public List<AbilityDef> abilities;

        public CompProperties_AbilitiesApparel() => 
            this.compClass = typeof(CompAbilitiesApparel);
    }
}