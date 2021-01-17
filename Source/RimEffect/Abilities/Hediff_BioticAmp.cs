namespace RimEffect
{
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
        public float energyUsed = 0f;
    }
}
