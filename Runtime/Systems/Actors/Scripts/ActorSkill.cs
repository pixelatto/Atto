[System.Serializable]
public class ActorSkill
{
    public ActorSkillTypes skillType;
    public float skillPower = 1f;
}

public enum ActorSkillTypes { Undefined, Run, Sprint, Levitate, Fly, Roll, Crawl }