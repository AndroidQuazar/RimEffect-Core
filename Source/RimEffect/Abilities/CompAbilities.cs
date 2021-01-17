namespace RimEffect
{
    using System;
    using System.Collections.Generic;
    using Verse;

    public class CompAbilities : ThingComp
    {
        private Pawn Pawn => (Pawn) this.parent;

        private List<Ability> learnedAbilities = new List<Ability>();

        public Ability currentlyCasting;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if(this.learnedAbilities == null)
                this.learnedAbilities = new List<Ability>();
        }

        public void GiveAbility(AbilityDef abilityDef)
        {
            Ability ability = (Ability) Activator.CreateInstance(abilityDef.abilityClass);
            ability.def    = abilityDef;
            ability.pawn   = this.Pawn;
            ability.holder = this.Pawn;

            this.learnedAbilities.Add(ability);
        }
        
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra()) 
                yield return gizmo;

            foreach (Ability ability in this.learnedAbilities) 
                yield return ability.GetGizmo();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref this.learnedAbilities, nameof(this.learnedAbilities), LookMode.Deep);
            Scribe_References.Look(ref this.currentlyCasting, nameof(this.currentlyCasting));

            if (this.learnedAbilities == null)
                this.learnedAbilities = new List<Ability>();
        }
    }
}
