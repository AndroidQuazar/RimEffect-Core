namespace RimEffect
{
    using RimWorld;
    using Verse;

    public class Ability_Charge : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            LongEventHandler.QueueLongEvent(() =>
                                            {
                                                IntVec3          destination = target.Cell + ((this.pawn.Position - target.Cell).ToVector3().normalized * 2).ToIntVec3();
                                                Map              map     = this.pawn.Map;

                                                AbilityPawnFlyer flyer = (AbilityPawnFlyer) PawnFlyer.MakeFlyer(RE_DefOf.RE_AbilityFlyer_Charge, this.pawn, destination);
                                                flyer.ability = this;
                                                flyer.target  = destination.ToVector3();
                                                GenSpawn.Spawn(flyer, target.Cell, map);
                                                target.Thing.TakeDamage(new DamageInfo(DamageDefOf.Blunt, this.GetPowerForPawn(), 1f, instigator: this.pawn));

                                            }, "chargeAbility", false, null);
        }
    }
}