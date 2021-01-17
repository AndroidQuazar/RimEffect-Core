using System;

namespace RimEffect
{
    using Verse;

    public class Ability_GiveHediff : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            AbilityExtension_Hediff extension = this.def.GetModExtension<AbilityExtension_Hediff>();
            Hediff                  hediff                 = HediffMaker.MakeHediff(extension.hediff, this.pawn);
            if(Math.Abs(extension.severity - -1f) > 0.01f)
                hediff.Severity = extension.severity;
            target.Pawn.health.AddHediff(hediff);
        }
    }

    public class AbilityExtension_Hediff : DefModExtension
    {
        public HediffDef hediff;
        public float     severity = -1f;
    }
}
