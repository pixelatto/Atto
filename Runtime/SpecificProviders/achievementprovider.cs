using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BindService]
public class AchievementsProvider
{

    ILogService logger;

    List<Achievement> achievementList;

    public AchievementsProvider()
    {
        logger = Atto.Get<ILogService>();
    }

    public void CheckAchievementUnlocks()
    {
        foreach (var achievement in achievementList)
        {
            achievement.Check();
        }
    }

    public void DefineAchievement(Achievement achievement)
    {
        if (achievementList == null)
        {
            achievementList = new List<Achievement>();
        }
        if (!achievementList.Contains(achievement))
        {
            achievementList.Add(achievement);
        } 
        else
        {
            logger.Log("Achievement already defined " + achievement.name);
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

    public List<Achievement> GetAchievementList()
    {
        return achievementList;
    }

    public void UnlockAchievement(Achievement entry)
    {
        achievementList.Find((x => entry.name == x.name)).unlocked = true;
    }

    public void UpdateAchievementProgress(Achievement entry, int progress)
    {
        achievementList.Find((x => entry.name == x.name)).progress = progress;
    }
    
}

public class Achievement
{
    public string name { get; set; }
    public bool unlocked { get; set; }
    public int progress { get; set; }
    public UnlockCondition Condition { get; set; }


    public void Check()
    {
        if (Condition != null && Condition())
        {
            unlocked = true;
        }
    }
}

public delegate bool UnlockCondition();