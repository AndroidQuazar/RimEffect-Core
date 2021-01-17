namespace RimEffect
{
    using System.Linq;
    using Verse;

    public class Hediff_Abilities : Hediff_ImplantWithLevel
    {
        public override void PostMake()
        {
            base.PostMake();
            this.GiveRandomAbilityAtLevel();
        }

        public override void ChangeLevel(int levelOffset)
        {
            int prevLevel = this.level;
            base.ChangeLevel(levelOffset);

            if (prevLevel != this.level)
                this.GiveRandomAbilityAtLevel();
        }

        public virtual void GiveRandomAbilityAtLevel()
        {
            this.pawn.GetComp<CompAbilities>().GiveAbility(DefDatabase<AbilityDef>.AllDefsListForReading.Where(def => def.requiredHediff != null && def.requiredHediff.hediffDef == this.def && def.requiredHediff.minimumLevel == this.level).RandomElement());
        }
    }
}
