namespace RimEffect
{
    using RimWorld;
    using Verse;

    public class Hediff_BioticAmp : Hediff_Abilities
    {
        public void UseEnergy(float energyUsed)
        {

        }

        public bool SufficientEnergyPresent(float energyWanted)
        {
            return true;
        }
    }

    public class AbilityExtension_Biotic : DefModExtension
    {
        public float GetEnergyUsedByPawn(Pawn pawn) => this.energyUsed * pawn.GetStatValue(RE_DefOf.RE_BioticAbilityCostMultiplier);

        public float energyUsed = 0f;
    }
}
