using System;

namespace InteractableProject.Data
{
    public class Quest
    {
        public QuestData Data { get; }
        public bool IsCompleted { get; private set; }

        public Quest(QuestData data)
        {
            Data = data;
        }

        public void Complete() => IsCompleted = true;
    }

    [Serializable]
    public struct QuestData
    {
        public string id;
        public string description;
    }
}
