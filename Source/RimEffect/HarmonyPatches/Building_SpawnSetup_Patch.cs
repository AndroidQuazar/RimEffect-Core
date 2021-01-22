using HarmonyLib;
using RimWorld;
using Verse;

namespace RimEffect
{

    [HarmonyPatch(typeof(TickManager), "DoSingleTick")]
    public static class DoSingleTick_Patch
    {
        public static bool colonyGrowthQuest_Dirty = false;
        public static void Postfix()
        {
            if (colonyGrowthQuest_Dirty)
            {
                foreach (var quest in Find.QuestManager.QuestsListForReading)
                {
                    var signal = new Signal("Quest" + quest.id + ".RoomConstructionCheck");
                    Find.SignalManager.SendSignal(signal);
                }
                colonyGrowthQuest_Dirty = false;
            }
        }
    }


    [HarmonyPatch(typeof(Building), "SpawnSetup")]
    public static class SpawnSetup_Patch
    {
        public static void Postfix()
        {
            DoSingleTick_Patch.colonyGrowthQuest_Dirty = true;
        }
    }

    [HarmonyPatch(typeof(Building), "DeSpawn")]
    public static class DeSpawn_Patch
    {
        public static void Postfix()
        {
            DoSingleTick_Patch.colonyGrowthQuest_Dirty = true;
        }
    }
}
