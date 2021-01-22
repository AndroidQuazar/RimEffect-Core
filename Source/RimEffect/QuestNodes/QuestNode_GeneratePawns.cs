using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimEffect
{
	public class QuestNode_GeneratePawns : QuestNode
	{
		[NoTranslate]
		public SlateRef<string> storeAs;

		public SlateRef<int> count;

		public SlateRef<List<PawnKindDef>> pawnKindDefs;
		protected override bool TestRunInt(Slate slate)
		{
			return true;
		}
		protected override void RunInt()
		{
			List<Pawn> pawns = new List<Pawn>();
			var slate = QuestGen.slate;
			for (var i = 0; i < count.GetValue(slate); i++)
            {
				var pawn = PawnGenerator.GeneratePawn(pawnKindDefs.GetValue(slate).RandomElement());
				pawns.Add(pawn);
            }
			slate.Set<List<Pawn>>(storeAs.GetValue(slate), pawns);
			var personnelData = "";
			foreach (var pawn in pawns)
            {
				personnelData += "\t" + pawn.LabelShort + ", " + pawn.kindDef.label + "\n";
            }
			slate.Set("personnelData", personnelData);
		}
	}
}
