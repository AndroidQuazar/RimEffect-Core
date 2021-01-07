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
	[StaticConstructorOnStartup]
	public class CompPowerPlantSolarCollectors : CompPowerPlant
	{
		private const float FullSunPower = 2400f;

		private const float NightPower = 0f;

		private static readonly Vector2 BarSize = new Vector2(2.5f, 0.11f);

		private static readonly Material PowerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));

		private static readonly Material PowerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));

		protected override float DesiredPowerOutput => Mathf.Lerp(0f, FullSunPower, parent.Map.skyManager.CurSkyGlow) * RoofedPowerOutputFactor;

		private float RoofedPowerOutputFactor
		{
			get
			{
				int num = 0;
				int num2 = 0;
				foreach (IntVec3 item in parent.OccupiedRect())
				{
					num++;
					if (parent.Map.roofGrid.Roofed(item))
					{
						num2++;
					}
				}
				return (float)(num - num2) / (float)num;
			}
		}

		public override void PostDraw()
		{
			base.PostDraw();
			GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
			var pos = parent.DrawPos;
			//pos.x += 0.005f;
			pos.z -= 0.535f;
			r.center = pos;
			r.size = BarSize;
			r.fillPercent = base.PowerOutput / FullSunPower;
			r.filledMat = PowerPlantSolarBarFilledMat;
			r.unfilledMat = PowerPlantSolarBarUnfilledMat;
			r.margin = 0.15f;
			Rot4 rotation = parent.Rotation;
			rotation.Rotate(RotationDirection.Clockwise);
			r.rotation = rotation;
			GenDraw.DrawFillableBar(r);
		}
	}
}
