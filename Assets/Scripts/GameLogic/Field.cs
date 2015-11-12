using System;
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
        [SerializeField]
        private Button locker;
        [SerializeField]
        private ResultScreen result;



        private List<Cell> cells = new List<Cell>(); 
        private GameObjectPull<Cell> cellsPull; 
        private int[,] matrix;

        private int correctsCount;
        private int openedCount;
        private bool isPaused;
        private IEnumerator correctCoroutine;
        private int level;
        private int score;


        private void Awake()
        {
            cellsPull = new GameObjectPull<Cell>(pullContainer, cellPrefab, 64, 10);
        }

        private void Start()
        {
            level = 1;
            score = 0;
            correctsCount = 3;
            StartNewGame(3, 3);
        }

        public void ReplayCurrent()
        {
            if (isPaused) return;
            StartNewGame(matrix.GetLength(0), matrix.GetLength(1));
        }

        public void NextRound()
        {
            if (isPaused) return;
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            correctsCount ++;
            level++;
            if (n*m/2 <= correctsCount)
            {
                m++;
                n++;
            }

            StartNewGame(n, m);
        }

        public void StartNewGame(int n, int m)
        {
            matrix = FieldGenerator.GetNewField(n, m, correctsCount);
            grid.constraintCount = m;
            PlaceCells();
            ShowCorrects();
            isPaused = false;
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
            if (correctCoroutine != null)
            {
                StopCoroutine(correctCoroutine);
            }
            correctCoroutine = showCorrectCoroutine();
            StartCoroutine(correctCoroutine);
        }

        

        public void OnCorrectCellChecked()
        {
            if (isPaused) return;
            openedCount++;
            if (openedCount >= correctsCount)
            {
                StartCoroutine(endGameCoroutine(true));
            }
        }

        public void OnWrongCellChecked()    
        {
            if (isPaused) return;
            Debug.Log("Fail");
            setAllCorrectsState(true);
            StartCoroutine(endGameCoroutine(false));
        }

        private IEnumerator endGameCoroutine(bool success)
        {
            isPaused = true;
            yield return new WaitForSeconds(0.5f);
            if (success) calculateScore();
            result.Show(success, level, score);
            locker.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            isPaused = false;
            if (success)
            {
                NextRound();
            }
            else
            {
                ReplayCurrent();
            }
        }

        private IEnumerator showCorrectCoroutine()
        {
            locker.gameObject.SetActive(true);
            setAllCorrectsState(true);
            yield return new WaitForEndOfFrame();
            result.Hide();

            yield return new WaitForSeconds(2f);

            setAllCorrectsState(false);
            locker.gameObject.SetActive(false);

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
        private void calculateScore()
        {
            score += correctsCount;
        }
    }
}
