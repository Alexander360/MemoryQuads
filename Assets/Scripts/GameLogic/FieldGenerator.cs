using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public class FieldGenerator
    {
        public static int[,] GetNewField(int n, int m, int correctsCount)
        {
            int [,] matrix = new int[n, m];
            for (int k = 0; k < correctsCount; k++)
            {
                int j;
                int i;
                do
                {
                    i = Random.Range(0, n);
                    j = Random.Range(0, m);
                } while (matrix[i, j] != 0);

                matrix[i, j] = 1;
            }

            return matrix;
        }
    }
}