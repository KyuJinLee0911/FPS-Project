using System;
using System.Collections.Generic;

[Serializable]
public class Stat
{
    public int level; // ID
    public float hp;
    public float defence;
    public int expToNextLevel;
}

[Serializable]
public class StatData
{
    public List<Stat> stats;
}