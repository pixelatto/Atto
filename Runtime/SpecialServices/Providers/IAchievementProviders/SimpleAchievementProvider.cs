using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAchievementProvider : IAchievementService
{

    List<IAchievement> achievementList;

    public void CheckAchievementUnlocks()
    {
        foreach (var achievement in achievementList)
        {
            achievement.Check();
        }
    }

    public void DefineAchievement(IAchievement achievement)
    {
        if (achievementList == null)
        {
            achievementList = new List<IAchievement>();
        }
        if (!achievementList.Contains(achievement))
        {
            achievementList.Add(achievement);
        } 
        else
        {
            Atto.Logger.Log("Achievement already defined " + achievement.name);
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

    public List<IAchievement> GetAchievementList()
    {
        return achievementList;
    }

    public void UnlockAchievement(IAchievement entry)
    {
        achievementList.Find((x => entry.name == x.name)).unlocked = true;
    }

    public void UpdateAchievementProgress(IAchievement entry, int progress)
    {
        achievementList.Find((x => entry.name == x.name)).progress = progress;
    }
    
}
