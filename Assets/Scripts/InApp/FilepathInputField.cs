using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace InApp
{
    [RequireComponent(typeof(InputField))]
    public class FilepathInputField : MonoBehaviour
    { 
        public string Text
        {
            set { field.text = value; }
            get { return field.text; }
        }
        public bool Interactable
        {
            set { field.interactable = value; }
            get { return field.interactable; }
        }

        public bool IsValid => Directory.Exists(Text) && new DirectoryInfo(Text).FullName.Contains(pathes.ProgramFolder) == false;

        [SerializeField] private Image outline;
        [SerializeField] private Color correctColor, invalidColor;

        private InputField field;
        private Pathes pathes;


        [Inject]
        private void Construct(Pathes pathes)
        {
            this.pathes = pathes;
        }

        private void Awake()
        {
            field = GetComponent<InputField>();
        }

        private void Update()
        {
            outline.color = IsValid || string.IsNullOrWhiteSpace(Text) ? correctColor : invalidColor;
        }
    }
}