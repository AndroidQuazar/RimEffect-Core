using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimEffect
{
	public class QuestNode_GetRandomPawnKinds : QuestNode
	{
		[NoTranslate]
		public SlateRef<string> storeAs;
		public SlateRef<List<PawnKindDef>> pawnKinds;
		public SlateRef<int> count;
		protected override bool TestRunInt(Slate slate)
		{
			return SetVars(slate);
		}

		protected override void RunInt()
		{
			SetVars(QuestGen.slate);
		}

		private bool SetVars(Slate slate)
		{
			List<PawnKindDef> pawns = new List<PawnKindDef>();
			for (var i = 0; i < count.GetValue(slate); i++)
            {
				if (pawnKinds.GetValue(slate).TryRandomElement(out PawnKindDef result))
				{
					if (pawns.Contains(result))
                    {
						i--;
                    }
					else
                    {
						pawns.Add(result);
                    }
				}
			}
			if (pawns.Any())
            {
				slate.Set(storeAs.GetValue(slate), pawns);
				return true;
			}
			return false;
		}
	}
}
