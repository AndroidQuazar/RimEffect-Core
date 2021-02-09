using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimEffect
{
    public class LordToil_DefendPointThenLeave : LordToil
    {
        protected LordToilData_DefendPoint Data
        {
            get
            {
                return (LordToilData_DefendPoint)this.data;
            }
        }

        public override IntVec3 FlagLoc
        {
            get
            {
                return this.Data.defendPoint;
            }
        }

        public override bool AllowSatisfyLongNeeds
        {
            get
            {
                return this.allowSatisfyLongNeeds;
            }
        }

        public LordToil_DefendPointThenLeave(bool canSatisfyLongNeeds = true)
        {
            this.allowSatisfyLongNeeds = canSatisfyLongNeeds;
            this.data = new LordToilData_DefendPoint();
        }

        public LordToil_DefendPointThenLeave(IntVec3 defendPoint, float defendRadius = 28f, float? wanderRadius = null) : this(true)
        {
            this.Data.defendPoint = defendPoint;
            this.Data.defendRadius = defendRadius;
            this.Data.wanderRadius = wanderRadius;
        }

        public override void LordToilTick()
        {
            if (Find.TickManager.TicksGame % 10 == 0)
            {
                this.UpdateAllDuties();
            }
            base.LordToilTick();
        }

        public override void UpdateAllDuties()
        {
            if (this.lord.ownedPawns.Count > 0)
            {
                LordToilData_DefendPoint data = this.Data;
                if (Map.mapPawns.AllPawnsSpawned.Any(p => p.Faction.HostileTo(this.lord.ownedPawns[0].Faction)))
                {
                    for (int i = 0; i < this.lord.ownedPawns.Count; i++)
                    {
                        Pawn pawn = this.lord.ownedPawns[i];
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.Defend, data.defendPoint, -1f);
                        pawn.mindState.duty.focusSecond = data.defendPoint;
                        pawn.mindState.duty.radius = ((pawn.kindDef.defendPointRadius >= 0f) ? pawn.kindDef.defendPointRadius : data.defendRadius);
                        pawn.mindState.duty.wanderRadius = data.wanderRadius;
                    }
                }
                else
                {
                    for (int i = 0; i < this.lord.ownedPawns.Count; i++)
                    {
                        Pawn pawn = this.lord.ownedPawns[i];
                        if (Rand.RangeInclusive(1, 4) == 3)
                        {
                            pawn.SetFaction(Faction.OfPlayer);
                            Find.LetterStack.ReceiveLetter("RE.NewRecruit".Translate(), "RE.JoinFac".Translate(pawn.Label), LetterDefOf.PositiveEvent);

                        }
                        else pawn.mindState.exitMapAfterTick = 5;
                    }
                    foreach (var quest in Find.QuestManager.QuestsListForReading)
                    {
                        var signal = new Signal("Quest" + quest.id + ".ShuttleDefenseOver");
                        Find.SignalManager.SendSignal(signal);
                    }
                    this.lord.ownedPawns.Clear();
                }
            }
        }

        public void SetDefendPoint(IntVec3 defendPoint)
        {
            this.Data.defendPoint = defendPoint;
        }

        private bool allowSatisfyLongNeeds = true;
    }
}