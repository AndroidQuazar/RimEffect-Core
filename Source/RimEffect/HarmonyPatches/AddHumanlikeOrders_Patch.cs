using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimEffect
{

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class AddHumanlikeOrders_Patch
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            TargetingParameters targetingParameters = new TargetingParameters();
            targetingParameters.canTargetPawns = true;
            targetingParameters.canTargetBuildings = false;
            targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
            targetingParameters.validator = delegate (TargetInfo x)
            {
                Pawn p = x.Thing as Pawn;
                return p != null && !p.Dead && p.health.hediffSet.HasHediff(RE_DefOf.RE_TurnBackToFormerFaction);
            };

            foreach (LocalTargetInfo dest2 in GenUI.TargetsAt_NewTemp(clickPos, targetingParameters, true, null))
            {
                Pawn toHelpPawn = (Pawn)dest2.Thing;
                FloatMenuOption item2;
                if (!pawn.CanReach(dest2, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
                {
                    item2 = new FloatMenuOption("CannotGoNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
                else
                {
                    item2 = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RE.FreeHostages".Translate(), delegate ()
                    {
                        pawn.jobs.TryTakeOrderedJob(new Job(RE_DefOf.RE_SaveHostages, toHelpPawn), JobTag.Misc);
                    }, MenuOptionPriority.RescueOrCapture, null, toHelpPawn, 0f, null, null), pawn, toHelpPawn, "ReservedBy");
                }
                opts.Add(item2);
            }
        }
    }
}

