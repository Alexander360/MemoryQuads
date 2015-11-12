using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
    [RequireComponent(typeof(Toggle))]
    public class Cell : MonoBehaviour
    {
        private Toggle toggle;
        private Field field;
        private bool isCorrect;
        private bool internalChange;
        private bool wasPressed;
        public bool IsCorrect
        {
            get { return isCorrect; }
        }

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(onToggle);
        }

        public void UpdateData(bool isCorrect, Field field)
        {
            this.isCorrect = isCorrect;
            this.field = field;
            toggle.graphic.color = isCorrect ? Color.green : Color.red;
            toggle.interactable = true;
            Toggle(false);
        }

        public void Toggle(bool on)
        {
            internalChange = true;
            toggle.isOn = on;
            internalChange = false;
        }

        private void onToggle(bool isOn)
        {
            if (internalChange) return;
            toggle.interactable = false;
            if (IsCorrect)
            {
                field.OnCorrectCellChecked();
            }
            else
            {
                field.OnWrongCellChecked();
            }
        }
    }
}