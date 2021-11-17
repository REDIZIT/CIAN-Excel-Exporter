using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    public class AppUI : MonoBehaviour
    {
        [SerializeField] private FilepathInputField folderField;
        [SerializeField] private ProgressBar progressBar;

        [SerializeField] private Button startButton;

        private Worker worker;

        [Inject]
        private void Construct(Worker worker)
        {
            this.worker = worker;
        }

        private void Update()
        {
            startButton.interactable = folderField.IsValid;
        }

        public void ClickStart()
        {
            worker.Start(folderField.Text);
        }
        public void ClickStop()
        {

        }
        public void ClickOpenExplorer()
        {
            
        }
    }
}