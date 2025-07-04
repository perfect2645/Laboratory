﻿using System.Collections.Generic;
using System.Text;

namespace Utils.Enumerable
{
    public static class ArrayExt
    {
        /// <summary>
        /// 打印一维或多维数组到控制台
        /// </summary>
        public static string? Print(this Array array)
        {
            if (array == null)
            {
                return null;
            }

            char separator = ' ';

            int rank = array.Rank;
            if (rank == 1)
            {
                var sb = new StringBuilder();
                sb.AppendJoin(separator, array);
                return sb.ToString();
            }
            else if (rank == 2)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);
                var sb = new StringBuilder();
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        sb.Append(array.GetValue(i, j));
                        if (j < cols - 1)
                        {
                            sb.Append(separator);
                        }
                    }
                    if (i < rows - 1)
                    {
                        sb.AppendLine();
                    }
                }

                return sb.ToString();
            }
            else
            {
                return "Array rank > 2 is not support";
            }
        }
    }
}
