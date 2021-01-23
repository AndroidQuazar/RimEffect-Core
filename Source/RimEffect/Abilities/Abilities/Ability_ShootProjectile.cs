namespace RimEffect
{
    using Verse;

    public class Ability_ShootProjectile : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            AbilityProjectile projectile = (AbilityProjectile) GenSpawn.Spawn(this.def.GetModExtension<AbilityExtension_Projectile>().projectile, this.pawn.Position, this.pawn.Map);
            projectile.power      = this.GetPowerForPawn();
            projectile.abilityDef = this.def;
            projectile.Launch(this.pawn, this.pawn.DrawPos, target.Pawn, target.Pawn, ProjectileHitFlags.IntendedTarget);
        }
    }

    public class AbilityExtension_Projectile : DefModExtension
    {
        public ThingDef projectile;
    }
}
