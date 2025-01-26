using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolGroup", menuName = "Scriptable Objects/Bool Group", order = 1)]
public class BoolGroup : ScriptableObject
{
    public bool[] boolGroup;

    public Vector2Int spawnPlace;

    public bool CompareGroup(bool[] comp)
    {
        bool check = true;
        for(int i=0;i<4;i++)
        {
            if(comp[i] != boolGroup[i])
            {
                check = false;
            }
        }
        return check;
    }
}
