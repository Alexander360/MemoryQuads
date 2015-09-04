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

        public bool IsCorrect
        {
            get { return isCorrect; }
        }

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        public void UpdateData(bool isCorrect, Field field)
        {
            this.isCorrect = isCorrect;
            this.field = field;
            toggle.graphic.color = isCorrect ? Color.green : Color.red;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(onToggle);
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
            Debug.Log("toggle " + isOn);
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