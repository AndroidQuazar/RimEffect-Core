using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using System.Text;

namespace RimEffect
{
    public class CompPowerPlantZero : CompPowerTrader
    {

        protected CompRefuelable refuelableComp;

        protected CompBreakdownable breakdownableComp;

        protected CompVariableHeatPusher variableHeatPusherComp;

        public float radiationRadius = 0;

        public int tickRadiation = 0;

        public const float radiationRadiusBase = 5f;

        public const float tickRadiationBase = 1250;

        public int temperatureRightNow = 0;
        public const int criticalTempWarning = 80;
        public const int criticalTemp = 100;

        public bool signalMeltdown = false;

        public bool noReactorRoom = false;

        public bool onlyBreakOnceDuringSolarFlare = false;



        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.radiationRadius, "radiationRadius", 0, false);
            Scribe_Values.Look<int>(ref this.tickRadiation, "tickRadiation", 0, false);
            Scribe_Values.Look<bool>(ref this.signalMeltdown, "signalMeltdown", false, false);
            Scribe_Values.Look<bool>(ref this.onlyBreakOnceDuringSolarFlare, "onlyBreakOnceDuringSolarFlare", false, false);

        }

        protected virtual float DesiredPowerOutputAndRadius
        {
            get
            {

                if (this.breakdownableComp.BrokenDown && signalMeltdown)
                {

                    radiationRadius = radiationRadiusBase * 7;
                    tickRadiation = (int)Math.Round(tickRadiationBase) / 3;
                    return 0f;
                }
                if (this.breakdownableComp.BrokenDown && !signalMeltdown)
                {

                    radiationRadius = 0;
                    tickRadiation = 0;
                    return 0f;
                }
                if (noReactorRoom)
                {
                    radiationRadius = 0;
                    tickRadiation = 0;
                    return 0f;
                }
                if (!base.PowerOn)
                {
                    return 0f;
                }

                if (!this.refuelableComp.HasFuel)
                {
                    return 0f;
                }
                if (this.refuelableComp.FuelPercentOfMax <= 0.5)
                {
                    signalMeltdown = false;
                    radiationRadius = 0;
                    tickRadiation = 0;
                    variableHeatPusherComp.HeatPerSecondVariable = variableHeatPusherComp.Props.heatPerSecond;
                    return -base.Props.basePowerConsumption;
                }
                else
                {
                    signalMeltdown = false;
                    float powerAdditional;
                    powerAdditional = (this.refuelableComp.FuelPercentOfMax) * base.Props.basePowerConsumption;
                    radiationRadius = radiationRadiusBase + ((this.refuelableComp.FuelPercentOfMax - 0.5f) * radiationRadiusBase * 5);
                    variableHeatPusherComp.HeatPerSecondVariable = variableHeatPusherComp.Props.heatPerSecond + (variableHeatPusherComp.Props.heatPerSecond * this.refuelableComp.FuelPercentOfMax);
                    tickRadiation = (int)Math.Round((tickRadiationBase * (1.0f - this.refuelableComp.FuelPercentOfMax)) + tickRadiationBase);
                    return -base.Props.basePowerConsumption - powerAdditional;
                }

            }
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            if (this.radiationRadius > 0)
            {
                GenDraw.DrawCircleOutline(this.parent.Position.ToVector3Shifted(), this.radiationRadius, SimpleColor.Blue);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.refuelableComp = this.parent.GetComp<CompRefuelable>();
            this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
            this.variableHeatPusherComp = this.parent.GetComp<CompVariableHeatPusher>();

            if (base.Props.basePowerConsumption < 0f && !this.parent.IsBrokenDown() && FlickUtility.WantsToBeOn(this.parent))
            {
                base.PowerOn = true;
            }
        }

        public override void CompTick()
        {
            base.CompTick();



            this.UpdateDesiredPowerOutput();

            if (!onlyBreakOnceDuringSolarFlare)
            {
                if (this.parent.Map.gameConditionManager.ElectricityDisabled)
                {
                    this.breakdownableComp.DoBreakdown();
                    onlyBreakOnceDuringSolarFlare = true;
                }
            }
            if (!this.parent.Map.gameConditionManager.ElectricityDisabled)
            {
                onlyBreakOnceDuringSolarFlare = false;
            }


            Room room = this.parent.PositionHeld.GetRoom(this.parent.Map, RegionType.Set_All);
            if (room != null)
            {
                if ((room.OutdoorsForWork || (!this.parent.Map.roofGrid.Roofed(this.parent.PositionHeld))) || room.OpenRoofCount > 0)
                {
                    noReactorRoom = true;
                }
                else noReactorRoom = false;
            }

            if (!noReactorRoom)
            {
                if (this.radiationRadius > 0)
                {
                    if (Find.TickManager.TicksGame % tickRadiation == 0)
                    {
                        int num = GenRadial.NumCellsInRadius(this.radiationRadius);
                        for (int i = 0; i < num; i++)
                        {
                            AffectCell(this.parent.Position + GenRadial.RadialPattern[i]);
                        }
                    }
                }


                float result;
                GenTemperature.TryGetTemperatureForCell(this.parent.Position, this.parent.Map, out result);
                temperatureRightNow = (int)Math.Round(result);
                if ((temperatureRightNow > criticalTemp) && !signalMeltdown)
                {
                    signalMeltdown = true;                  
                    this.breakdownableComp.DoBreakdown();
                    this.refuelableComp.ConsumeFuel(refuelableComp.Fuel);
                    List<ThingDef> links = new List<ThingDef>();
                    links.Add(ThingDef.Named("RE_ElementZeroReactor"));
                    Find.LetterStack.ReceiveLetter("RE_MeltdownLetterLabelEezo".Translate(), "RE_MeltdownLetterEezo".Translate(), LetterDefOf.NegativeEvent, this.parent, null, null, links, null);
                    /*GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.PsychicDrone, -1);
                    gameCondition.Duration = 120000;                  
                    gameCondition.conditionCauser = this.parent;
                    parent.Map.gameConditionManager.RegisterCondition(gameCondition);*/
                    IncidentDef localDef = IncidentDef.Named("PsychicDrone");
                   
                    IncidentParms parms = StorytellerUtility.DefaultParmsNow(localDef.category, parent.Map);
                    localDef.Worker.TryExecute(parms);


                }

            }



        }

        public void AffectCell(IntVec3 c)
        {

            if (c.InBounds(this.parent.Map))
            {
                HashSet<Thing> hashSet = new HashSet<Thing>(c.GetThingList(this.parent.Map));
                if (hashSet != null)
                {
                    foreach (Thing thing in hashSet)
                    {
                        Pawn affectedPawn = thing as Pawn;
                        if (affectedPawn != null && affectedPawn.RaceProps.IsFlesh && !affectedPawn.AnimalOrWildMan())
                        {
                            float num = 1;
                            num *= affectedPawn.GetStatValue(StatDefOf.PsychicSensitivity, true);
                            if (num != 0f)
                            {
                                affectedPawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("RE_EezoDrone"), null);
                            }

                        }
                    }
                }







            }
        }

        public void UpdateDesiredPowerOutput()
        {

            base.PowerOutput = this.DesiredPowerOutputAndRadius;
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.CompInspectStringExtra());
            stringBuilder.AppendLine();


            if (noReactorRoom)
            {
                stringBuilder.Append("RE_NoReactorRoomEezo".Translate());
                if (this.parent.GetComp<CompBreakdownable>().BrokenDown && signalMeltdown)
                {
                    stringBuilder.AppendLine();
                    CompPlantHarmRadiusIfBroken comp = this.parent.GetComp<CompPlantHarmRadiusIfBroken>();
                    stringBuilder.Append("RE_MeltdownEezo".Translate());
                }
                return stringBuilder.ToString();

            }
            else
            {
                if (temperatureRightNow < 80)
                {
                    stringBuilder.Append("RE_TempInReactorRoomEezo".Translate(temperatureRightNow));
                }
                else stringBuilder.Append("RE_TempInReactorRoomCriticalEezo".Translate(temperatureRightNow));

                if (this.radiationRadius > 0)
                {
                    stringBuilder.AppendLine();

                    stringBuilder.Append("RE_RadiationProducedEezo".Translate((int)Math.Round(this.radiationRadius)));

                }
                if (this.parent.GetComp<CompBreakdownable>().BrokenDown && signalMeltdown)
                {
                    stringBuilder.AppendLine();
                  
                    stringBuilder.Append("RE_MeltdownEezo".Translate());
                }

                return stringBuilder.ToString();

            }


        }


    }
}
