﻿namespace RimEffect
{
    using Verse;

    public class Ability_ShootProjectile : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            Projectile projectile = GenSpawn.Spawn(this.def.GetModExtension<AbilityExtension_Projectile>().projectile, this.pawn.Position, this.pawn.Map) as Projectile;

            if (projectile is AbilityProjectile abilityProjectile)
            {
                abilityProjectile.power      = this.GetPowerForPawn();
                abilityProjectile.abilityDef = this.def;
            }
            projectile.Launch(this.pawn, this.pawn.DrawPos, target, target, ProjectileHitFlags.IntendedTarget);
        }
    }

    public class AbilityExtension_Projectile : DefModExtension
    {
        public ThingDef projectile;
    }
}
