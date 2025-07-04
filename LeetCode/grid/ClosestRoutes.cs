using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Enumerable;

namespace LeetCode.grid
{
    public class ClosestRoutes
    {
        public ClosestRoutes()
        {
            var initialSpots = new int[,]
            {
                { 0, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 1, 0 }
            };

            var result = GetClosestSteps(initialSpots);
            //Debug.WriteLine(initialSpots.Print()); 
        }

        private int[,] GetClosestSteps(int[,] initialSpots)
        {
            if (initialSpots == null)
            {
                throw new ArgumentNullException(nameof(initialSpots), "Initial spots cannot be null.");
            }

            var rows = initialSpots.GetLength(0);
            var columns = initialSpots.GetLength(1);

            if (columns <= 1 || rows <= 1)
            {
                throw new ArgumentOutOfRangeException("Columns and rows must be greater than 1.");
            }

            Debug.WriteLine(initialSpots.Print());

            int[,] result = new int[rows, columns];

            return result;
        }
    }
}
