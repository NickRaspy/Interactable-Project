using System.Collections.Generic;
using UnityEngine;

namespace InteractableProject.Data
{
    [CreateAssetMenu(fileName = "Quest Data", menuName = "Quests/Quest Data", order = 0)]
    public class QuestSO : ScriptableObject
    {
        public List<QuestData> quests;
    }
}
