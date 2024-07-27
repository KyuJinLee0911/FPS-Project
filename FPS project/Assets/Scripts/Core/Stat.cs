using System;
using System.Collections.Generic;

[Serializable]
public class Stat: Data<int>
{
    public Stat()
    {
        key = level;
    }
    public int level; // ID
    public float hp;
    public float defence;
    public int expToNextLevel;
}

[Serializable]
public class Data<T>
{
    public T key;
}