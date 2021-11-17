using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    public class AppUI : MonoBehaviour
    {
        [SerializeField] private FilepathInputField folderField;
        [SerializeField] private ProgressBar progressBar;

        [SerializeField] private Button startButton, stopButton, openButton;

        private Worker worker;

        [Inject]
        private void Construct(Worker worker)
        {
            this.worker = worker;
        }

        private void Update()
        {
            folderField.Interactable = worker.state.type == WorkerState.Type.Idle;

            startButton.interactable = folderField.IsValid;

            startButton.gameObject.SetActive(worker.state.type == WorkerState.Type.Idle);
            stopButton.gameObject.SetActive(worker.state.type == WorkerState.Type.Downloading || worker.state.type == WorkerState.Type.Awaiting);
            openButton.gameObject.SetActive(worker.state.type == WorkerState.Type.Done);
        }

        public void ClickStart()
        {
            worker.Start(folderField.Text);
        }
        public void ClickStop()
        {
            worker.Stop();
        }
        public void ClickOpenExplorer()
        {
            Process.Start(folderField.Text);
        }
    }
}