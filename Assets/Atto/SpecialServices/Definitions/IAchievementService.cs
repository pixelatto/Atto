
using System;
using System.Collections.Generic;

public interface IAchievementService
{
    void DownloadAchievements();
    void UploadAchievements();

    List<IAchievement> GetAchievementList();

    void DefineAchievement(IAchievement achievement);
    void UnlockAchievement(IAchievement achievement);
    void UpdateAchievementProgress(IAchievement achievement, int progress);
    void CheckAchievementUnlocks();
}

public interface IAchievement
{
    string name { get; set; }
    bool unlocked { get; set; }
    int progress { get; set; }
    UnlockCondition UnlockCondition { get; set; }
    void Check();
}

public delegate bool UnlockCondition();
