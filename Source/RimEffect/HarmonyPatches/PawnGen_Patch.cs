namespace RimEffect
{
    using HarmonyLib;
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

            CompAbilities comp = __result.GetComp<CompAbilities>();

            foreach (AbilityDef abilityDef in abilityExtension.giveAbilities) 
                comp.GiveAbility(abilityDef);
        }
    }
}