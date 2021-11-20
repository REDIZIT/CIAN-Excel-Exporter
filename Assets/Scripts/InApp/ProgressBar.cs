using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    [RequireComponent(typeof(Slider))]
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Text handledCountText, stateText, estTimeText;

        private Slider slider;
        private Worker worker;

        [Inject]
        private void Construct(Worker worker)
        {
            this.worker = worker;
        }

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void Update()
        {
            handledCountText.text = $"����������: {worker.state.currentUrlIndex}/{worker.state.urlsCount}";
            stateText.text = GetStateText();
            estTimeText.text = GetEstTimeText();
        }

        private string GetStateText()
        {
            return worker.state.type switch
            {
                WorkerState.Type.Idle => "�� �������",
                WorkerState.Type.Downloading => "���������",
                WorkerState.Type.Awaiting => $"������� ({Mathf.RoundToInt(worker.state.awaitTimeLeft)})",
                WorkerState.Type.Error => "������",
                WorkerState.Type.Done => "������",
                _ => worker.state.type.ToString(),
            };
        }
        private string GetEstTimeText()
        {
            int secondsLeft;
            string prefix;
            if (worker.state.type != WorkerState.Type.Done)
            {
                int urlsLeft = worker.state.urlsCount - worker.state.currentUrlIndex - 1;
                secondsLeft = urlsLeft * Worker.DELAY_SECONDS + worker.state.awaitTimeLeft;
                secondsLeft += worker.state.currentUrlIndex / Worker.CHILL_URLS_COUNT * Worker.DELAY_SECONDS;
                prefix = "��������: ";
            }
            else
            {
                secondsLeft = (int)(worker.state.finishTime - worker.state.startTime).TotalSeconds;
                prefix = "������ �������: ";
            }
            
            TimeSpan ts = TimeSpan.FromSeconds(secondsLeft);

            return prefix + string.Format("{0:%h}� {0:%m}� {0:%s}�", ts);
        }
    }
}