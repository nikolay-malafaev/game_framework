using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlainIdList", menuName = "Game/PlainIdList")]
public class PlainIdList : ScriptableObject, IIdList
{
    public List<string> IdList = new();
    
    public IEnumerable<string> GetIds()
    {
        return IdList;
    }
}