using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimEffect
{
	public class Hediff_Spectre : Hediff
	{
        public int tickShouldLeave;
        public int tickShouldLeaveMax;
        private bool leaving;
        public override void Tick()
        {
            base.Tick();
            if (!leaving)
            {
                if (this.pawn.Map != null)
                {
                    if (Find.TickManager.TicksAbs > tickShouldLeave && this.pawn.Map.dangerWatcher.DangerRating != StoryDanger.High || Find.TickManager.TicksAbs > tickShouldLeaveMax)
                    {
                        Leave();
                    }
                }
                else if (this.pawn.ParentHolder is Caravan caravan && caravan.IsPlayerControlled)
                {
                    if (Find.TickManager.TicksAbs > tickShouldLeave && Find.TickManager.TicksAbs > tickShouldLeaveMax)
                    {
                        LeaveCaravan(caravan);
                    }
                }
            }

        }

        private void Leave()
        {
            var allianceFaction = Find.FactionManager.FirstFactionOfDef(RE_DefOf.RE_SystemsAlliance);
            this.pawn.SetFaction(allianceFaction);
            var jbg = new JobGiver_ExitMapRandom();
            jbg.ResolveReferences();
            var result = jbg.TryIssueJobPackage(pawn, default(JobIssueParams));
            if (result.Job != null)
            {
                this.pawn.jobs.TryTakeOrderedJob(result.Job);
                Find.LetterStack.ReceiveLetter("RE.SpectreLeaves".Translate(), "RE.SpectreLeavesDesc".Translate(), LetterDefOf.NeutralEvent, this.pawn, allianceFaction);
                leaving = true;
            }
        }

        private void LeaveCaravan(Caravan caravan)
        {
            var allianceFaction = Find.FactionManager.FirstFactionOfDef(RE_DefOf.RE_SystemsAlliance);
            this.pawn.SetFaction(allianceFaction);
            caravan.RemovePawn(this.pawn);
            Find.LetterStack.ReceiveLetter("RE.SpectreLeaves".Translate(), "RE.SpectreLeavesDesc".Translate(), LetterDefOf.NeutralEvent, null, allianceFaction);
            leaving = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickShouldLeave, "tickShouldLeave");
            Scribe_Values.Look(ref tickShouldLeaveMax, "tickShouldLeaveMax");
            Scribe_Values.Look(ref leaving, "leaving");
        }
    }
}
