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
	internal class CompPowerFusionCell : CompPowerPlant
	{
        public override void PostExposeData()
        {
			Thing refee = null;
			if (Scribe.mode == LoadSaveMode.Saving && connectParent != null)
			{
				refee = connectParent.parent;
			}
			Scribe_References.Look(ref refee, "parentThingFusionCell");
			if (refee != null)
			{
				connectParent = ((ThingWithComps)refee).GetComp<CompPower>();
			}
			if (Scribe.mode == LoadSaveMode.PostLoadInit && connectParent != null)
			{
				ConnectToTransmitter(connectParent, reconnectingAfterLoading: true);
			}
		}
    }
}
