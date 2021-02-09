using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimEffect.GenSteps
{
	class GenStep_RE_PirateOrMech : GenStep
	{
		public override int SeedPart => 66863014;

		public override void Generate(Map map, GenStepParams parms)
		{

			ResolveParams resolveParams = new ResolveParams();
			resolveParams.rect = CellRect.CenteredOn(map.Center, 5);

			if (Rand.Bool) resolveParams.faction = Find.FactionManager.AllFactionsListForReading.FindAll(f => f.def.defName == "Mechanoid").RandomElement();
			else resolveParams.faction = Find.FactionManager.AllFactionsListForReading.FindAll(f => f.def.defName == "Pirate").RandomElement();

			Lord singlePawnLord = LordMaker.MakeNewLord(resolveParams.faction, new LordJob_DefendPoint(resolveParams.rect.CenterCell), map, null);
			resolveParams.singlePawnLord = singlePawnLord;

			resolveParams.pawnGroupKindDef = PawnGroupKindDefOf.Combat;
			if (resolveParams.pawnGroupMakerParams == null)
			{
				resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms();
				resolveParams.pawnGroupMakerParams.tile = map.Tile;
				resolveParams.pawnGroupMakerParams.faction = resolveParams.faction;
				resolveParams.pawnGroupMakerParams.points = (int)(StorytellerUtility.DefaultSiteThreatPointsNow() * 1.25);
			}
			BaseGen.symbolStack.Push("pawnGroup", resolveParams, null);
			BaseGen.globalSettings.map = map;
			BaseGen.Generate();

			Faction allianceFac = Find.QuestManager.QuestsListForReading.Find(q => q.QuestLookTargets.Any(look => look.HasWorldObject && Find.World.worldObjects.ObjectsAt(map.Tile).Contains(look.WorldObject))).InvolvedFactions.ToList().Find(f => f != map.ParentFaction && f != Faction.OfPlayer);

			Lord lord = LordMaker.MakeNewLord(allianceFac, new LordJob_DefendPointThenLeave(resolveParams.rect.CenterCell, 10f, false, false), map);
			for (int i = 0; i < Rand.RangeInclusive(2, 5); i++)
			{
				IntVec3 nearCenterWalkable;
				CellFinder.TryFindRandomCellNear(map.Center, map, 8, c => c.Walkable(map) && !map.fogGrid.IsFogged(c), out nearCenterWalkable);

				Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(RE_DefOf.RE_Alliance_Marine, allianceFac, mustBeCapableOfViolence: true));
				this.AddRandomDamageTo(pawn, Rand.RangeInclusive(15, 60));
				GenSpawn.Spawn(pawn, nearCenterWalkable, map);

				lord.AddPawn(pawn);
			}
		}

		private static IEnumerable<BodyPartRecord> HittablePartsViolence(HediffSet bodyModel)
		{
			return from x in bodyModel.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null)
				   where x.depth == BodyPartDepth.Outside || (x.depth == BodyPartDepth.Inside && x.def.IsSolid(x, bodyModel.hediffs))
				   select x;
		}

		private void AddRandomDamageTo(Pawn p, int damageAmount)
		{
			HediffSet hediffSet = p.health.hediffSet;
			p.health.forceIncap = true;
			IEnumerable<BodyPartRecord> source = from x in HittablePartsViolence(hediffSet)
												 where !p.health.hediffSet.hediffs.Any((Hediff y) => y.Part == x && y.CurStage != null && y.CurStage.partEfficiencyOffset < 0f)
												 select x;
			int num = 0;
			while (num < damageAmount && source.Any<BodyPartRecord>())
			{
				BodyPartRecord bodyPartRecord = source.RandomElementByWeight((BodyPartRecord x) => x.coverageAbs);
				int num2 = Mathf.RoundToInt(hediffSet.GetPartHealth(bodyPartRecord)) - 3;
				if (num2 >= 8)
				{
					DamageDef damageDef;
					if (bodyPartRecord.depth == BodyPartDepth.Outside)
					{
						damageDef = HealthUtility.RandomViolenceDamageType();
					}
					else
					{
						damageDef = DamageDefOf.Blunt;
					}
					int num3 = Rand.RangeInclusive(Mathf.RoundToInt((float)num2 * 0.65f), num2);
					HediffDef hediffDefFromDamage = HealthUtility.GetHediffDefFromDamage(damageDef, p, bodyPartRecord);
					if (!p.health.WouldDieAfterAddingHediff(hediffDefFromDamage, bodyPartRecord, (float)num3))
					{
						DamageInfo dinfo = new DamageInfo(damageDef, (float)num3, 999f, -1f, null, bodyPartRecord, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
						dinfo.SetAllowDamagePropagation(false);
						p.TakeDamage(dinfo);
						num += num3;
					}
				}
			}
		}
	}
}
