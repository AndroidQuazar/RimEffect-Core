namespace RimEffect
{
    using RimWorld;
    using Verse;

    public class Ability_Charge : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            IntVec3 destination = target.Cell + ((this.pawn.Position - target.Cell).ToVector3().normalized * 2).ToIntVec3();

            this.pawn.Position    = destination;

            this.pawn.Notify_Teleported(endCurrentJob: false);

            target.Thing.TakeDamage(new DamageInfo(DamageDefOf.Blunt, this.GetPowerForPawn(), 1f, instigator: this.pawn));
        }
    }
}