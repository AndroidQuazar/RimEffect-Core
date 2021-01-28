namespace RimEffect
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class Ability_Flare : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);
            GenExplosion.DoExplosion(target.Cell, this.pawn.Map, this.GetRadiusForPawn(), DamageDefOf.Blunt, this.pawn, Mathf.RoundToInt(this.GetPowerForPawn()), 1f, SoundDefOf.PlanetkillerImpact);
        }
    }
}
