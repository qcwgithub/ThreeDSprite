using System.Collections.Generic;

namespace Data
{

    public class MapInfoProfile
    {
        public int Chapter;
        public long LastTimeRobbed;
        public List<int> RobbedMissions;   // 被抢夺的任务(这里的任务存的都是missionIndex)
        public List<int> UnsafeMissions;   // 会被抢夺的任务
        public List<string> BattleIDs;      // 战斗ID，将这两章的战斗ID都填上，因为关卡顺序是固定的，所以说只保留两章的战斗ID即可；按照章节表分配事件时，章节首位无事件，无连续三个事件
        public List<string> UnlockIsland;
        public List<string> UnlockFog;

        public MapInfoProfile() { }

        public MapInfoProfile(MapInfoProfile info)
        {
            Chapter = info.Chapter;
            if (Chapter < 1) { Chapter = 1; }

            LastTimeRobbed = info.LastTimeRobbed;

            if (info.RobbedMissions == null) { RobbedMissions = new List<int>(); }
            else { RobbedMissions = new List<int>(info.RobbedMissions); }

            if (info.UnsafeMissions == null) { UnsafeMissions = new List<int>(); }
            else { UnsafeMissions = new List<int>(info.UnsafeMissions); }

            if (info.BattleIDs == null) { BattleIDs = new List<string>(); }
            else { BattleIDs = new List<string>(info.BattleIDs); }

            if (info.UnlockFog == null) { UnlockFog = new List<string>(); }
            else { UnlockFog = new List<string>(info.UnlockFog); }

            if (info.UnlockIsland == null) { UnlockIsland = new List<string>(); }
            else { UnlockIsland = new List<string>(info.UnlockIsland); }
        }

        public static MapInfoProfile Ensure(MapInfoProfile obj)
        {
            if (obj == null)
            {
                obj = new MapInfoProfile();
            }

            if (obj.Chapter < 1)
            {
                obj.Chapter = 1;
            }

            if (obj.RobbedMissions == null)
            {
                obj.RobbedMissions = new List<int>();
            }
            if (obj.UnsafeMissions == null)
            {
                obj.UnsafeMissions = new List<int>();
            }
            if (obj.BattleIDs == null)
            {
                obj.BattleIDs = new List<string>();
            }
            if (obj.UnlockIsland == null)
            {
                obj.UnlockIsland = new List<string>();
            }
            if (obj.UnlockFog == null)
            {
                obj.UnlockFog = new List<string>();
            }

            return obj;
        }

        public void NewChapter()
        {
            Chapter++;
        }

        public void MissionBeSafe(int missionIndex)   // 不安全任务点转为安全任务点
        {
            UnsafeMissions.Remove(missionIndex);
        }

        public void RobMission(int missionIndex, int index)
        {
            if (UnsafeMissions.Count <= index || UnsafeMissions[index] != missionIndex) { return; }

            UnsafeMissions.Remove(missionIndex);
            RobbedMissions.Add(missionIndex);
            RobbedMissions.Sort();
        }
        public void MissionCleared(int index)
        {
            if (UnsafeMissions.Contains(index))
            {
                return;
            }
            if (RobbedMissions.Contains(index)) { RobbedMissions.Remove(index); }

            UnsafeMissions.Add(index);
            UnsafeMissions.Sort();
        }
        public bool IsMissionCleared(int index)
        {
            return UnsafeMissions.Contains(index);
        }
        public bool IsMissionRobbed(int index)
        {
            return RobbedMissions.Contains(index);
        }
        public void AddUnlockMapFog(string fogID)
        {
            UnlockFog.Add(fogID);
        }
        public void AddUnlockIsland(string checkpoint)
        {
            UnlockIsland.Add(checkpoint);
        }
    }
}