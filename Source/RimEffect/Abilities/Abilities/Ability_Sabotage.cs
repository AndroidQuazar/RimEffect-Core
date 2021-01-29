namespace RimEffect
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class Ability_Sabotage : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            if (target.Pawn != null)
            {
                if (target.Pawn.RaceProps.IsFlesh)
                    target.Pawn.stances.StaggerFor(Mathf.RoundToInt(this.GetPowerForPawn() * GenTicks.TicksPerRealSecond));
                else if (target.Pawn.RaceProps.IsMechanoid)
                    target.Pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, forceWake: true);
            }
        }
    }
}
