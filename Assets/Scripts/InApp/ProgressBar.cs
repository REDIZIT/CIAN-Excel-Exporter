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
            handledCountText.text = $"Обработано: -1/{worker.UrlsCount}";

            //worker.state.CurrentIndex = 1;
        }
    }
}