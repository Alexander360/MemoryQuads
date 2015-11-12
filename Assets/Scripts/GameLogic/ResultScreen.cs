using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.GameLogic
{
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField]
        private Text result;
        [SerializeField]
        private Text scoreLabel;
        [SerializeField]
        private Text levelLabel;

        public void Show(bool isVictory, int level, int score)
        {
            gameObject.SetActive(true);
            if (isVictory)
            {
                result.text = "Correct!";
            }
            else
            {
                result.text = "Fail!";
            }
            levelLabel.text = level.ToString();
            scoreLabel.text = score.ToString();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}