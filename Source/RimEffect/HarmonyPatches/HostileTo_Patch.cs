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
    [HarmonyPatch(typeof(GenHostility), "HostileTo", new Type[] { typeof(Thing), typeof(Thing) })]
    public static class HostileTo_Patch
    {
        public static void Postfix(this Thing a, Thing b, ref bool __result)
        {
            Pawn p = b as Pawn;
            if (p != null && p.health != null && p.health.hediffSet != null && p.health.hediffSet.HasHediff(RE_DefOf.RE_TurnBackToFormerFaction)) __result = false;
        }
    }
}
