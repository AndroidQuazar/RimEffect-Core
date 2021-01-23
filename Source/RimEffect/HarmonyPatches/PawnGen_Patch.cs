namespace RimEffect
{
    using HarmonyLib;
    using RimWorld;
    using Verse;

    [HarmonyPatch(typeof(PawnGenerator), "GenerateNewPawnInternal")]
    public class PawnGen_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __result)
        {
            PawnKindAbilityExtension abilityExtension = __result.kindDef.GetModExtension<PawnKindAbilityExtension>();
            if (abilityExtension == null)
                return;

            if (abilityExtension.implantDef != null)
            {
                if (HediffMaker.MakeHediff(abilityExtension.implantDef, __result, __result.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Brain).FirstOrFallback()) is Hediff_Abilities implant)
                {
                    implant.giveRandomAbilities = abilityExtension.giveRandomAbilities;
                    __result.health.AddHediff(implant);
                    implant.SetLevelTo(abilityExtension.initialLevel);
                }
            }

            CompAbilities comp = __result.GetComp<CompAbilities>();

            foreach (AbilityDef abilityDef in abilityExtension.giveAbilities) 
                comp.GiveAbility(abilityDef);
        }
    }
}