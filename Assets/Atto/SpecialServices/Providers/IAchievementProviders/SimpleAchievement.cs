using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAchievement : IAchievement
{
    public string name { get; set; }
    public bool unlocked { get; set; }
    public int progress { get; set; }
    public UnlockCondition UnlockCondition { get; set; }

    public void Check()
    {
        if (UnlockCondition != null && UnlockCondition())
        {
            unlocked = true;
        }
    }
}