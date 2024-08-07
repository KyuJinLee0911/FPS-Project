using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature : MonoBehaviour
{
    public string key;
    public string description;
    public virtual void DoFeature() { }
    public virtual void RemoveFeature() { }
}
