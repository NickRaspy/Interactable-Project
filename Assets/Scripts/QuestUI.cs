using InteractableProject.Data;
using InteractableProject.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace InteractableProject.UI
{
    public class QuestUI : MonoBehaviour
    {
        [Inject] private QuestManager questManager;

        [SerializeField] private GameObject questUIObject;
        [SerializeField] private TMP_Text questText;
        [SerializeField] private string questEndedMessage = "";
        
        [SerializeField] private InputActionReference questUIAction;

        private void Awake()
        {
            questManager.AddQuestStartedAction(OnQuestStarted);
            questManager.AddQuestsEndedAction(OnQuestsEnded);
        }

        private void OnDestroy()
        {
            if (questManager == null) return;

            questManager.RemoveQuestStartedAction(OnQuestStarted);
            questManager.RemoveQuestsEndedAction(OnQuestsEnded);
        }

        private void OnQuestStarted(Quest quest)
        {
            UpdateText(quest.Data.description);
            ShowQuestUI();
        }

        private void OnQuestsEnded()
        {
            UpdateText(questEndedMessage);
            ShowQuestUI();
        }
        
        private void OnEnable()
        {
            questUIAction.action.performed += HideQuestUI_Action;
            questUIAction.action.Enable();
        }

        private void OnDisable()
        {
            questUIAction.action.performed -= HideQuestUI_Action;
            questUIAction.action.Disable();
        }
        
        private void HideQuestUI_Action(InputAction.CallbackContext ctx) => HideQuestUI();
        
        public void ShowQuestUI() => questUIObject.SetActive(true);
        
        public void HideQuestUI() => questUIObject.SetActive(false);

        private void UpdateText(string text) => questText.text = text;

        private void OnGUI()
        {
#if UNITY_EDITOR
            if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space) HideQuestUI();
#endif
        }
    }
}
