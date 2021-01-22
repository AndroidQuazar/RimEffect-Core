using HarmonyLib;
using RimWorld;
using Verse;

namespace RimEffect
{

    [HarmonyPatch(typeof(Pawn_DrawTracker), "Notify_DamageApplied")]
    public static class Notify_DamageApplied_Patch
    {
        public static bool disableFlash = false;
        public static bool Prefix()
        {
            if (disableFlash)
            {
                disableFlash = false;
                return false;
            }
            return true;
        }
    }
}
