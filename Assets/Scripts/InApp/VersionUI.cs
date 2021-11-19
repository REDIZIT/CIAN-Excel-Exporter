using UnityEngine;
using UnityEngine.UI;

namespace InApp
{
    public class VersionUI : MonoBehaviour
    {
        [SerializeField] private Text versionText;

        private const string REPO_URL = "https://github.com/REDIZIT/CIAN-Excel-Exporter/releases";

        private void Start()
        {
            versionText.text = Application.version;
        }
        public void OpenGithub()
        {
            Application.OpenURL(REPO_URL);
        }
    }
}