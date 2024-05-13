using UnityEngine;

[System.Serializable]
public abstract class SkillBase : MonoBehaviour
{
    public abstract Skill skillType { get; }

    [Range(0, maxLevel)] public int level = 0;
    public Vector2 powerRange = new Vector2(1, 2);

    public virtual float skillDuration  => -1;
    public virtual float power => Mathf.Lerp(powerRange.x, powerRange.y, (float)(level - 1f) / (float)(maxLevel - 1f));

    const int maxLevel = 3;
}

public enum Skill { Undefined, Walk, Run, Jump, Fly, Crawl, Roll }