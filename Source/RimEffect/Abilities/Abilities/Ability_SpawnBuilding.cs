namespace RimEffect
{
    using UnityEngine.Assertions.Must;
    using Verse;

    public class Ability_SpawnBuilding : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            AbilityExtension_Building extension = this.def.GetModExtension<AbilityExtension_Building>();

            if (extension.building != null)
            {
                Thing building = GenSpawn.Spawn(extension.building, target.Cell, this.pawn.Map);
                building.SetFaction(this.pawn.Faction);
            }
        }
    }

    public class AbilityExtension_Building : DefModExtension
    {
        public ThingDef building;
    }
}
