
using System;
using System.Collections.Generic;

public interface IAchievementService<EntryType>
{
    void DownloadAchievements();
    void UploadAchievements();

    List<EntryType> GetAchievementList();

    void DefineAchievement(EntryType entry);
    void UnlockAchievement(EntryType entry);
    void UpdateAchievementProgress(EntryType entry, int progress);
    void CheckAchievementUnlocks();
}
