namespace RimEffect
{
    using Verse;

    public class Ability_Barrier : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            if(this.pawn.GetComp<CompAbilities>().ReinitShield(this.GetPowerForPawn()))
                base.Cast(target);
        }
    }
}