﻿using System;
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
        [SerializeField]
        private Text leftCellsLabel;
        [SerializeField]
        private Text currentLevelLabel;





        private List<Cell> cells = new List<Cell>(); 
        private GameObjectPull<Cell> cellsPull; 
        private int[,] matrix;

        private int correctsCount;
        private int openedCount;
        private bool isPaused;
        private IEnumerator correctCoroutine;
        private int currentLevel;
        private int score;

        public int OpenedCount
        {
            get { return openedCount; }
            set 
            { 
                openedCount = value;
                leftCellsLabel.text = (correctsCount - openedCount).ToString();
            }
        }

        public int CurrentLevel
        {
            get { return currentLevel; }
            set
            {
                currentLevel = value;
                currentLevelLabel.text = currentLevel.ToString();
            }
        }

        private void Awake()
        {
            cellsPull = new GameObjectPull<Cell>(pullContainer, cellPrefab, 64, 10);
        }

        private void Start()
        {
            CurrentLevel = 1;
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
            CurrentLevel++;
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
           
            OpenedCount = 0;

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
            OpenedCount++;
            if (OpenedCount >= correctsCount)
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
            result.Show(success, CurrentLevel, score);
            leftCellsLabel.gameObject.SetActive(false);
            locker.gameObject.SetActive(true);
            isPaused = false;
            /*yield return new WaitForSeconds(1);
           
            if (success)
            {
                NextRound();
            }
            else
            {
                ReplayCurrent();
            }*/
        }

        private IEnumerator showCorrectCoroutine()
        {
            locker.gameObject.SetActive(true);
            setAllCorrectsState(true);
            yield return new WaitForEndOfFrame();
            isPaused = true;
            result.Hide();

            yield return new WaitForSeconds(2f);

            setAllCorrectsState(false);
            locker.gameObject.SetActive(false);
            leftCellsLabel.gameObject.SetActive(true);
            isPaused = false;

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
