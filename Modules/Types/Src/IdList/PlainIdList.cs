using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Types
{
    [CreateAssetMenu(fileName = "PlainIdList", menuName = "GameFramework/Types/PlainIdList")]
    public class PlainIdList : ScriptableObject, IIdList
    {
        public List<string> IdList = new();
    
        public IEnumerable<string> GetIds()
        {
            return IdList;
        }
    }
}