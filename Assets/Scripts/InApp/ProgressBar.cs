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
            handledCountText.text = $"Обработано: {worker.state.currentUrlIndex}/{worker.state.urlsCount}";
            stateText.text = GetStateText();
            estTimeText.text = GetEstTimeText();
        }

        private string GetStateText()
        {
            return worker.state.type switch
            {
                WorkerState.Type.Idle => "Не запущен",
                WorkerState.Type.Downloading => "Скачивает",
                WorkerState.Type.Awaiting => $"Ожидает ({Mathf.RoundToInt(worker.state.awaitTimeLeft)})",
                WorkerState.Type.Error => "Ошибка",
                WorkerState.Type.Done => "Готово",
                _ => worker.state.type.ToString(),
            };
        }
        private string GetEstTimeText()
        {
            int secondsLeft;
            string prefix;
            if (worker.state.type != WorkerState.Type.Done)
            {
                secondsLeft = (worker.state.urlsCount - worker.state.currentUrlIndex - 1) * Worker.DELAY_SECONDS + worker.state.awaitTimeLeft;
                prefix = "Осталось: ";
            }
            else
            {
                secondsLeft = (int)(DateTime.Now - worker.state.startTime).TotalSeconds;
                prefix = "Прошло времени: ";
            }
            
            TimeSpan ts = TimeSpan.FromSeconds(secondsLeft);

            return prefix + string.Format("{0:%h}ч {0:%m}м {0:%s}с", ts);
        }
    }
}