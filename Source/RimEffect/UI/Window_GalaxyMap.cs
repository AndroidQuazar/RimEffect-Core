using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimEffect
{
    public class Window_GalaxyMap : Window
    {
		public override void DoWindowContents(Rect inRect)
		{
			Rect rect = new Rect(inRect.x, inRect.y + 15f, inRect.width, 30f);
			Text.Font = GameFont.Small;
			if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - 30f, inRect.width / 2f, 30f), "CancelButton".Translate()))
			{
				Close();
			}
			if (Widgets.ButtonText(new Rect(inRect.x + inRect.width / 2f, inRect.yMax - 30f, inRect.width / 2f, 30f), "OK".Translate()))
			{
				Close();
			}
		}
	}
}
