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
	public class Hediff_TargetingVI : HediffWithComps
	{
        public List<Verb> extendedVerbs = new List<Verb>();
        public override void PostMake()
        {
            CheckAllVerbsAndTryExtend();
            base.PostMake();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            CheckAllVerbsAndTryExtend();
        }

        private void CheckAllVerbsAndTryExtend()
        {
            if (this.pawn?.apparel?.WornApparel != null)
            {
                foreach (Apparel item in this.pawn.apparel.WornApparel)
                {
                    CompReloadable comp = item.GetComp<CompReloadable>();
                    if (comp != null)
                    {
                        foreach (Verb verb in comp.AllVerbs)
                        {
                            this.TryCheckAndExtendVerbRange(verb);
                        }
                    }
                }
            }
            if (this.pawn?.equipment?.AllEquipmentListForReading != null)
            {
                List<ThingWithComps> allEquipmentListForReading = this.pawn.equipment.AllEquipmentListForReading;
                for (int i = 0; i < allEquipmentListForReading.Count; i++)
                {
                    foreach (Verb verb in allEquipmentListForReading[i].GetComp<CompEquippable>().AllVerbs)
                    {
                        this.TryCheckAndExtendVerbRange(verb);
                    }
                }
            }
        }

        public void TryCheckAndExtendVerbRange(Verb verb)
        {
            if (!extendedVerbs.Contains(verb) && verb.verbProps.range > 1.42f && verb.caster is Pawn pawn)
            {
                ExtendVerbRangeBy(verb, 1.20f);
                extendedVerbs.Add(verb);
            }
        }
        private void ExtendVerbRangeBy(Verb verb, float multiplier)
        {
            var newProperties = new VerbProperties();
            foreach (var fieldInfo in typeof(VerbProperties).GetFields())
            {
                try
                {
                    var newField = verb.verbProps.GetType().GetField(fieldInfo.Name);
                    newField.SetValue(newProperties, fieldInfo.GetValue(verb.verbProps));
                }
                catch { }
            }
            newProperties.range *= multiplier;
            verb.verbProps = newProperties;
        }
    }
}
