using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimEffect
{
	public class QuestNode_GetRoomsToBuild : QuestNode
	{
		[NoTranslate]
		public SlateRef<string> storeAs;

		public SlateRef<List<RoomRoleDef>> roomRoleDefs;
		public SlateRef<int> count;
		public SlateRef<int> minRoomSize;
		protected override bool TestRunInt(Slate slate)
		{
			return true;
		}

		protected override void RunInt()
		{
			Dictionary<RoomRoleDef, int> roomsDict = new Dictionary<RoomRoleDef, int>();
			List<RoomRoleDef> rooms = new List<RoomRoleDef>();
			var slate = QuestGen.slate;
			for (var i = 0; i < count.GetValue(slate); i++)
			{
				if (roomRoleDefs.GetValue(slate).TryRandomElement(out RoomRoleDef result))
				{
					if (roomsDict.ContainsKey(result))
					{
						roomsDict[result]++;
					}
					else
					{
						roomsDict[result] = 1;
					}
					rooms.Add(result);
				}
			}

			if (rooms.Any())
			{
				slate.Set(storeAs.GetValue(slate), rooms);
				string roomsToBeBuild = "";
				foreach (var room in roomsDict)
				{
					roomsToBeBuild += "\t" + room.Value + "x " + room.Key.label + "\n";
				}
				slate.Set("roomsToBeBuild", roomsToBeBuild);
				slate.Set(storeAs.GetValue(slate), rooms);
				var map = slate.Get<Map>("map");
				QuestPart_RoomRequirement questPart_RoomRequirement = new QuestPart_RoomRequirement();
				questPart_RoomRequirement.inSignal = QuestGenUtility.HardcodedSignalWithQuestID("RoomConstructionCheck");
				questPart_RoomRequirement.map = map;
				var oldRooms = new List<RoomRequirement>();
				foreach (var room in map.regionGrid.allRooms)
                {
					var roomReq = new RoomRequirement();
					roomReq.currentCellCount = room.CellCount;
					roomReq.roomRoleDef = room.Role;
					oldRooms.Add(roomReq);
				}
				questPart_RoomRequirement.oldRooms = oldRooms;
				var newRooms = new List<RoomRequirement>();
				var minSize = minRoomSize.GetValue(slate);
				foreach (var room in rooms)
                {
					var roomReq = new RoomRequirement();
					roomReq.minimumCellRequirement = minSize * minSize;
					roomReq.roomRoleDef = room;
					newRooms.Add(roomReq);
				}
				questPart_RoomRequirement.roomsToBeBuilt = newRooms;
				slate.Set<string>("minRoomSizeInfo", minSize + "x" + minSize);
				QuestGen.quest.AddPart(questPart_RoomRequirement);
			}
		}
	}
}
