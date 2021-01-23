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
	public class IncidentWorker_PyjakCrate : IncidentWorker
	{
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            var cell = DropCellFinder.RandomDropSpot(map);
            var pyjakCount = Rand.RangeInclusive(2, 4);
            var pyjaks = new List<Pawn>();
            for (var i = 0; i < pyjakCount; i++)
            {
                var pyjak = PawnGenerator.GeneratePawn(RE_DefOf.RE_Pyjak, Faction.OfPlayer);
                pyjaks.Add(pyjak);
            }
            DropPodUtility.DropThingsNear(cell, map, pyjaks, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false, forbid: false);
            SendStandardLetter("RE.LetterPyjaksCrate".Translate(), "RE.LetterPyjaksCrateDesc".Translate(), baseLetterDef: LetterDefOf.PositiveEvent, 
                parms: parms, lookTargets: pyjaks, textArgs: Array.Empty<NamedArgument>());
            return true;
        }
    }
}
