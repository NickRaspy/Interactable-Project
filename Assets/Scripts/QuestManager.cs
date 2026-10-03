using System.Collections.Generic;
using InteractableProject.Data;
using InteractableProject.Objects;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableProject.Managers
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private QuestSO questSO;

        public Dictionary<string, Quest> Quests { get; } = new();

        private readonly List<string> questIDs = new();
        private readonly Dictionary<string, List<QuestObject>> questObjectsByQuestID = new();
        private int currentQuestIndex;
        private bool hasStarted;

        public Quest CurrentQuest =>
            currentQuestIndex < questIDs.Count &&
            Quests.TryGetValue(questIDs[currentQuestIndex], out var quest)
                ? quest
                : null;

        private readonly UnityEvent<Quest> onQuestStarted = new();
        private readonly UnityEvent<Quest> onQuestCompleted = new();
        private readonly UnityEvent onQuestsEnded = new();

        private void Awake()
        {
            if (questSO == null)
            {
                Debug.LogError($"{nameof(questSO)} is null", this);
                return;
            }

            if (questSO.quests == null)
            {
                Debug.LogError($"{nameof(questSO)} has no quest list", this);
                return;
            }

            foreach (var quest in questSO.quests)
            {
                Quests.Add(quest.id, new Quest(quest));
                questIDs.Add(quest.id);
            }
        }

        private void Start()
        {
            hasStarted = true;
            TryStartQuest();
        }

        public void RegisterQuestObject(QuestObject questObject)
        {
            if (questObject == null) return;

            var questID = questObject.RequiredQuestID;
            if (string.IsNullOrEmpty(questID))
            {
                Debug.LogError("QuestObject has no quest ID", questObject);
                return;
            }

            if (!questObjectsByQuestID.TryGetValue(questID, out var objects))
            {
                objects = new List<QuestObject>();
                questObjectsByQuestID.Add(questID, objects);
            }

            if (objects.Contains(questObject)) return;
            objects.Add(questObject);

            var quest = CurrentQuest;
            if (hasStarted && quest != null && questID == quest.Data.id)
                questObject.OnQuestStarted?.Invoke();
        }

        public void UnregisterQuestObject(QuestObject questObject)
        {
            var questID = questObject.RequiredQuestID;
            if (string.IsNullOrEmpty(questID)) return;
            if (!questObjectsByQuestID.TryGetValue(questID, out var objects)) return;

            objects.Remove(questObject);
            if (objects.Count == 0)
                questObjectsByQuestID.Remove(questID);
        }

        private void StartCurrentQuest()
        {
            var quest = CurrentQuest;
            if (quest == null) return;

            if (questObjectsByQuestID.TryGetValue(quest.Data.id, out var objects))
            {
                foreach (var questObject in objects.ToArray())
                {
                    if (questObject != null)
                        questObject.OnQuestStarted?.Invoke();
                }
            }

            onQuestStarted.Invoke(quest);
        }

        public void CompleteQuest(string questID)
        {
            var quest = CurrentQuest;
            if (quest == null || quest.Data.id != questID || quest.IsCompleted)
                return;

            quest.Complete();

            if (questObjectsByQuestID.TryGetValue(quest.Data.id, out var objects))
            {
                foreach (var questObject in objects.ToArray())
                {
                    if (questObject != null)
                        questObject.OnQuestCompleted?.Invoke();
                }
            }

            currentQuestIndex++;

            onQuestCompleted.Invoke(quest);

            TryStartQuest();
        }

        private void TryStartQuest()
        {
            if (CurrentQuest != null) StartCurrentQuest();
            else onQuestsEnded.Invoke();
        }

        public void AddQuestStartedAction(UnityAction<Quest> action) => onQuestStarted.AddListener(action);

        public void RemoveQuestStartedAction(UnityAction<Quest> action) => onQuestStarted.RemoveListener(action);

        public void AddQuestCompletedAction(UnityAction<Quest> action) => onQuestCompleted.AddListener(action);

        public void AddQuestsEndedAction(UnityAction action) => onQuestsEnded.AddListener(action);

        public void RemoveQuestsEndedAction(UnityAction action) => onQuestsEnded.RemoveListener(action);

        private void OnDestroy()
        {
            onQuestStarted.RemoveAllListeners();
            onQuestCompleted.RemoveAllListeners();
            onQuestsEnded.RemoveAllListeners();
        }
    }
}
