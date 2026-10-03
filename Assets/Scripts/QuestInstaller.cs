using System;
using InteractableProject.Managers;
using UnityEngine;
using Zenject;

namespace InteractableProject.Installers
{
    public class QuestInstaller : MonoInstaller
    {
        [SerializeField] private QuestManager questManager;

        public override void InstallBindings()
        {
            if (!questManager)
                throw new InvalidOperationException("QuestInstaller requires a QuestManager reference.");

            Container.Bind<QuestManager>().FromInstance(questManager).AsSingle();
        }
    }
}
