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
	public class JobDriver_UseVIITerminal : JobDriver
	{
		private float workLeft = -1000f;


		private Building_VIInterface Building_VIInterface => this.TargetA.Thing as Building_VIInterface;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
			Toil doWork = new Toil();
			doWork.initAction = delegate
			{
				workLeft = new IntRange(400, 800).RandomInRange;
				Building_VIInterface.Connect(this.pawn);
			};
			doWork.tickAction = delegate
			{
				workLeft -= 1;
				if (workLeft <= 0f)
				{
					ReadyForNextToil();
				}
				else
				{
					Building_VIInterface.ChatWithTerminal(pawn);
					JoyUtility.JoyTickCheckEnd(pawn);
				}
			};
			doWork.defaultCompleteMode = ToilCompleteMode.Never;
			doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			doWork.activeSkill = (() => SkillDefOf.Social);
			doWork.socialMode = RandomSocialMode.Off;
			doWork.AddFinishAction(() => Building_VIInterface.Disconnect());
			yield return doWork;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref workLeft, "workLeft", 0f);
		}
	}
}
