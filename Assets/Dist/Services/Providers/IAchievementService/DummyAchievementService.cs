using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAchievementService : IAchievementService<DummyAchievement>
{

    List<DummyAchievement> achievementList;

    public void CheckAchievementUnlocks()
    {
        foreach (var achievement in achievementList)
        {
            achievement.Check();
        }
    }

    public void DefineAchievement(DummyAchievement achievement)
    {
        if (achievementList == null)
        {
            achievementList = new List<DummyAchievement>();
        }
        if (!achievementList.Contains(achievement))
        {
            achievementList.Add(achievement);
        } 
        else
        {
            Core.Logger.Log("Achievement already defined " + achievement.name);
        }
    }

    public void DownloadAchievements()
    {
        throw new NotImplementedException();
    }

    public void UploadAchievements()
    {
        throw new NotImplementedException();
    }

    public List<DummyAchievement> GetAchievementList()
    {
        return achievementList;
    }

    public void UnlockAchievement(DummyAchievement entry)
    {
        achievementList.Find((x => entry.name == x.name)).unlocked = true;
    }

    public void UpdateAchievementProgress(DummyAchievement entry, int progress)
    {
        achievementList.Find((x => entry.name == x.name)).progress = progress;
    }

}

public class DummyAchievement
{
    public string name;
    public bool unlocked;
    public int progress;
    public Predicate<DummyAchievement> UnlockCondition;

    public void Check()
    {
        if (UnlockCondition != null && UnlockCondition(this))
        {
            unlocked = true;
        }
    }
}