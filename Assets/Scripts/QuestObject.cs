using InteractableProject.Managers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace InteractableProject.Objects
{
    public class QuestObject : MonoBehaviour
    {
        private QuestManager questManager;

        [SerializeField] private string requiredQuestID;

        public string RequiredQuestID => requiredQuestID;

        public UnityEvent OnQuestStarted;
        public UnityEvent OnQuestCompleted;

        [Inject]
        private void Construct(QuestManager manager)
        {
            questManager = manager;
            questManager.RegisterQuestObject(this);
        }

        private void OnDestroy()
        {
            if (questManager != null)
                questManager.UnregisterQuestObject(this);
        }

        public void CompleteQuest()
        {
            if (questManager != null)
                questManager.CompleteQuest(requiredQuestID);
        }
    }
}
