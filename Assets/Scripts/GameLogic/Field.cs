using System.Collections;
using System.Collections.Generic;
using Assets.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
    public class Field : MonoBehaviour
    {
        [SerializeField]
        private Cell cellPrefab;
        [SerializeField]
        private GridLayoutGroup grid;
        [SerializeField]
        private GameObject pullContainer;

        private List<Cell> cells = new List<Cell>(); 
        private GameObjectPull<Cell> cellsPull; 
        private int[,] matrix;

        private int correctsCount;
        private int openedCount;

        private void Awake()
        {
            cellsPull = new GameObjectPull<Cell>(pullContainer, cellPrefab, 64, 10);
        }

        private void Start()
        {
            correctsCount = 3;
            StartNewGame(3, 3);
        }

        public void NextRound()
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            correctsCount ++;
            if (n*m/2 <= correctsCount)
            {
                if (n == m) m++;
                else n++;
            }

            StartNewGame(n, m);
        }

        public void StartNewGame(int n, int m)
        {
            matrix = FieldGenerator.GetNewField(n, m, correctsCount);
            grid.constraintCount = m;
            PlaceCells();
            ShowCorrects();
        }

        public void PlaceCells()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cellsPull.ReleaseObject(cells[i]);
            }
            cells.Clear();
           
            openedCount = 0;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    placeCell(matrix[i,j] != 0);
                }
            }
        }

        public void ShowCorrects()
        {
            StartCoroutine(showCorrectCoroutine());
        }

        

        public void OnCorrectCellChecked()
        {
            openedCount++;
            if (openedCount >= correctsCount)
                EndRound();
        }

        public void OnWrongCellChecked()
        {
            Debug.Log("Fail");
            setAllCorrectsState(true);
        }

        public void EndRound()
        {
            Debug.Log("Success");
        }

        private IEnumerator showCorrectCoroutine()
        {
            setAllCorrectsState(true);


            yield return new WaitForSeconds(2f);

            setAllCorrectsState(false);

        }

        private void setAllCorrectsState(bool show)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].IsCorrect)
                    cells[i].Toggle(show);
            }
        }

        private void placeCell(bool isCorrect)
        {
            Cell cell = cellsPull.GetObject();
            cell.gameObject.AppendTo(grid.gameObject);
            cell.gameObject.SetActive(true);
            cell.UpdateData(isCorrect, this);
            cells.Add(cell);
        }
    }
}
