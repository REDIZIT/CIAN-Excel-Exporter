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
        [SerializeField] private Slider progressBar;

        [SerializeField] private Button startButton, stopButton, openButton;
        [SerializeField] private GameObject buttonsGroup, errorGroup;

        private Worker worker;
        private Pathes pathes;

        [Inject]
        private void Construct(Worker worker, Pathes pathes)
        {
            this.worker = worker;
            this.pathes = pathes;
        }
        private void Update()
        {
            folderField.Interactable = worker.state.type == WorkerState.Type.Idle;

            startButton.interactable = folderField.IsValid;

            startButton.gameObject.SetActive(worker.state.type == WorkerState.Type.Idle);
            stopButton.gameObject.SetActive(worker.state.type == WorkerState.Type.Downloading || worker.state.type == WorkerState.Type.Awaiting);
            openButton.gameObject.SetActive(worker.state.type == WorkerState.Type.Done);

            buttonsGroup.SetActive(worker.state.type != WorkerState.Type.Error);
            errorGroup.SetActive(worker.state.type == WorkerState.Type.Error);

            UpdateProgressBar();
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
        public void ClickOpenLogs()
        {
            Process.Start(pathes.LogsFile);
        }

        private void UpdateProgressBar()
        {
            progressBar.value = worker.state.currentUrlIndex;
            progressBar.maxValue = worker.state.urlsCount;
        }
    }
}