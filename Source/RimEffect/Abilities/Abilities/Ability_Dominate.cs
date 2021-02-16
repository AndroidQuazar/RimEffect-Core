namespace RimEffect
{
    using RimWorld;
    using Verse;

    public class Ability_Dominate : Ability
    {
        public override bool ValidateTarget(LocalTargetInfo target) => 
            base.ValidateTarget(target) && target.Pawn.RaceProps.IsFlesh;

        public override void Cast(LocalTargetInfo target)
        {
            if(target.Pawn.mindState.mentalStateHandler.TryStartMentalState(RE_DefOf.RE_DominationBerserk, "RE.AbilityDominateBerserkReason".Translate(this.pawn.NameShortColored), forceWake: true))
                base.Cast(target);
        }
    }
}