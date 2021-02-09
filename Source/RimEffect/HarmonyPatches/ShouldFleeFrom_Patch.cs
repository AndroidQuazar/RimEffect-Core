using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimEffect
{
    [HarmonyPatch(typeof(SelfDefenseUtility), "ShouldFleeFrom")]
    public static class ShouldFleeFrom_Patch
    {
        public static void Postfix(Thing t, Pawn pawn, bool checkDistance, bool checkLOS, ref bool __result)
        {
            Pawn p = t as Pawn;
            if (p != null && p.health != null && p.health.hediffSet != null && p.health.hediffSet.HasHediff(RE_DefOf.RE_TurnBackToFormerFaction)) __result = false;
        }
    }
}
