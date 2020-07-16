using System;
using System.Collections.Generic;
using System.Text;

namespace MathOperation.Helpers
{ 
    public static class MatrixExtensions
    {
        public static T[] GetRow<T>(this T[,] matrix, int rowIndex)
        {
            int rowLength = matrix.GetLength(1);
            T[] rowVector = new T[rowLength];

            for (int i = 0; i < rowLength; i++)
            {
                rowVector[i] = matrix[rowIndex, i];
            }

            return rowVector;
        }

        public static List<T> GetColumn<T>(this T[,] matrix, int columnIndex)
        {
            int colLength = matrix.GetLength(0);
            List<T> colVector = new List<T>();

            for (int i = 0; i < colLength; i++)
            {
                if(matrix[i, columnIndex] != null)
                    colVector.Add(matrix[i, columnIndex]);
            }

            return colVector;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void ClearColumn<T>( this T[,] table, int columnIndex, int Row)
        {
            for (int i = 0; i < Row; i++)
                table[i, columnIndex] = default(T);
        }
    }
}
