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
	public class IncidentWorker_SpectreJoin : IncidentWorker_WandererJoin
	{
        public override Pawn GeneratePawn()
        {
			Gender? fixedGender = null;
			if (def.pawnFixedGender != 0)
			{
				fixedGender = def.pawnFixedGender;
			}
			Pawn spectre = PawnGenerator.GeneratePawn(new PawnGenerationRequest(def.pawnKind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, 
				forceGenerateNewPawn: true, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, def.pawnMustBeCapableOfViolence, 20f, 
				forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, 
				forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 1f, null, 1f, null,
				null, null, null, null, null, null, fixedGender));
			spectre.apparel.LockAll();
			var spectreHediff = HediffMaker.MakeHediff(RE_DefOf.RE_Spectre, spectre) as Hediff_Spectre;
			spectreHediff.tickShouldLeave = (int)(Find.TickManager.TicksAbs + (GenDate.TicksPerDay * Rand.Range(3f, 6f)));
			spectreHediff.tickShouldLeaveMax = (int)(Find.TickManager.TicksAbs + (GenDate.TicksPerDay * Rand.Range(9f, 12f)));
			spectre.health.AddHediff(spectreHediff);
			return spectre;
		}
    }
}
