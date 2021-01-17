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
    [DefOf]
    public static class RE_DefOf
    {
        public static ThingDef RE_AmmoCryoBelt;
        public static ThingDef RE_AmmoDisruptorBelt;
        public static ThingDef RE_AmmoExplosiveBelt;
        public static ThingDef RE_AmmoIncendiaryBelt;
        public static ThingDef RE_AmmoPiercingBelt;
        public static ThingDef RE_AmmoToxicBelt;

        public static HediffDef Hypothermia;
        public static DamageDef RE_BombNoShake;

        public static SoundDef RE_Ammo_Enable;
        public static JobDef RE_PyjakNuzzle;

        public static ThoughtDef RE_Thought_PyjakNuzzle;
        public static InteractionDef RE_Interaction_PyjakNuzzle;
        public static FactionDef RE_SystemsAlliance;
        public static HediffDef RE_Spectre;
        public static PawnKindDef RE_Pyjak;

        public static JobDef RE_UseAbility;
        public static JobDef RE_UseCommsConsole;
		
        public static HediffDef RE_BioticAmpHediff;
        public static StatDef   RE_BioticAbilityCostMultiplier;
    }
}
