using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimEffect
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "ExposeData")]
    public static class ExposeData_Patch
    {
        public static void Postfix(Pawn_ApparelTracker __instance)
        {
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                var targetingHediff = __instance.pawn?.health?.hediffSet?.GetFirstHediffOfDef(RE_DefOf.RE_TargetingVI) as Hediff_TargetingVI;
                if (targetingHediff != null)
                {
                    foreach (Apparel item in __instance.WornApparel)
                    {
                        CompReloadable comp = item.GetComp<CompReloadable>();
                        if (comp != null)
                        {
                            foreach (Verb verb in comp.AllVerbs)
                            {
                                targetingHediff.TryCheckAndExtendVerbRange(verb);
                            }
                        }
                    }
                }

            }
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded")]
    public static class Notify_ApparelAdded_Patch
    {
        public static void Postfix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            var targetingHediff = __instance.pawn?.health?.hediffSet?.GetFirstHediffOfDef(RE_DefOf.RE_TargetingVI) as Hediff_TargetingVI;
            if (targetingHediff != null)
            {
                List<Verb> list = apparel.GetComp<CompReloadable>()?.AllVerbs;
                if (list != null)
                {
                    foreach (Verb verb in list)
                    {
                        targetingHediff.TryCheckAndExtendVerbRange(verb);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "ExposeData")]
    public static class Pawn_EquipmentTracker_ExposeData_Patch
    {
        public static void Postfix(Pawn_EquipmentTracker __instance)
        {
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                var targetingHediff = __instance.pawn?.health?.hediffSet?.GetFirstHediffOfDef(RE_DefOf.RE_TargetingVI) as Hediff_TargetingVI;
                if (targetingHediff != null)
                {
                    List<ThingWithComps> allEquipmentListForReading = __instance.AllEquipmentListForReading;
                    for (int i = 0; i < allEquipmentListForReading.Count; i++)
                    {
                        foreach (Verb verb in allEquipmentListForReading[i].GetComp<CompEquippable>().AllVerbs)
                        {
                            targetingHediff.TryCheckAndExtendVerbRange(verb);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    public static class Notify_EquipmentAdded_Patch
    {
        public static void Postfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            var targetingHediff = __instance.pawn?.health?.hediffSet?.GetFirstHediffOfDef(RE_DefOf.RE_TargetingVI) as Hediff_TargetingVI;
            if (targetingHediff != null)
            {
                foreach (Verb verb in eq.GetComp<CompEquippable>().AllVerbs)
                {
                    targetingHediff.TryCheckAndExtendVerbRange(verb);
                }
            }
        }
    }
}
