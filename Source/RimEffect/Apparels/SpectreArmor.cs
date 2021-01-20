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
    public class SpectreArmor : Apparel
    {
        public override void DrawWornExtras()
        {
            base.DrawWornExtras();
            Comps_PostDraw();
        }
    }
}
