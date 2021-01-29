using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimEffect
{
	public class JoyGiver_UseVITerminal : JoyGiver_InteractBuilding
	{
		protected override bool CanDoDuringGathering => true;
		protected override Job TryGivePlayJob(Pawn pawn, Thing t)
		{
			var vIInterface = t as Building_VIInterface;
			if (vIInterface is null || !vIInterface.CanUse || vIInterface.InUse || !pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, Danger.Deadly))
			{
				Log.Message(pawn + " can't get " + def.jobDef);
				Log.Message($"{t is Building_VIInterface} || {vIInterface.CanUse} || {vIInterface.InUse} || {pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, Danger.Deadly)}");
				return null;
			}
			Log.Message(pawn + " got " + def.jobDef);
			return JobMaker.MakeJob(def.jobDef, t);
		}
	}
}
