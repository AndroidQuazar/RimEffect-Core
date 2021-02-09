using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RimEffect
{
    internal class LordJob_DefendPointThenLeave : LordJob
    {
		public override bool IsCaravanSendable
		{
			get
			{
				return this.isCaravanSendable;
			}
		}

		public override bool AddFleeToil
		{
			get
			{
				return this.addFleeToil;
			}
		}

		public LordJob_DefendPointThenLeave()
		{
		}

		public LordJob_DefendPointThenLeave(IntVec3 point, float? wanderRadius = null, bool isCaravanSendable = false, bool addFleeToil = true)
		{
			this.point = point;
			this.wanderRadius = wanderRadius;
			this.isCaravanSendable = isCaravanSendable;
			this.addFleeToil = addFleeToil;
		}

		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			stateGraph.AddToil(new LordToil_DefendPointThenLeave(this.point, 28f, this.wanderRadius));
			return stateGraph;
		}

		public override void ExposeData()
		{
			Scribe_Values.Look<IntVec3>(ref this.point, "point", default(IntVec3), false);
			Scribe_Values.Look<float?>(ref this.wanderRadius, "wanderRadius", null, false);
			Scribe_Values.Look<bool>(ref this.isCaravanSendable, "isCaravanSendable", false, false);
			Scribe_Values.Look<bool>(ref this.addFleeToil, "addFleeToil", false, false);
		}

		private IntVec3 point;

		private float? wanderRadius;

		private bool isCaravanSendable;

		private bool addFleeToil;
	}
}