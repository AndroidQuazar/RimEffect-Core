namespace RimEffect
{
    using System.Collections.Generic;
    using System.Linq;
    using Verse;

    public class Hediff_Abilities : Hediff_ImplantWithLevel
    {
        public bool giveRandomAbilities = true;
        
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            this.GiveRandomAbilityAtLevel();
        }

        public override void ChangeLevel(int levelOffset)
        {
            int prevLevel = this.level;
            base.ChangeLevel(levelOffset);
            if (prevLevel != this.level && levelOffset > 0)
                for(; prevLevel < this.level; )
                    this.GiveRandomAbilityAtLevel(++prevLevel);
        }

        public virtual void GiveRandomAbilityAtLevel(int? forLevel = null)
        {
            if (!this.giveRandomAbilities) 
                return;

            forLevel = forLevel ?? this.level;

            this.pawn.GetComp<CompAbilities>().GiveAbility(DefDatabase<AbilityDef>.AllDefsListForReading.Where(def => def.requiredHediff != null && def.requiredHediff.hediffDef == this.def && def.requiredHediff.minimumLevel == forLevel).RandomElement());
        }

        public virtual IEnumerable<Gizmo> DrawGizmos()
        {
            yield break;
        }
    }
}
