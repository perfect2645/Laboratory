namespace LeetCode.numbers
{
    internal class SumWithoutMathSymbol
    {
        public SumWithoutMathSymbol() 
        {
            var arr = new int[] { 2,3 };
            var result = SumArrayByBinary(arr);
        }

        private int SumArrayByBinary(int[] intArr)
        {
            int sum = 0;
            foreach (int i in intArr)
            {
                sum = SumByBinary(sum, i);
            }

            return sum;
        }

        private int SumByBinary(int a, int b)
        {
            if (b == 0)
            {
                return a;
            }

            var xor = a ^ b; // 异或操作
            var carry = (a & b) << 1; // 进位操作

            return SumByBinary(xor, carry); // 递归调用
        }
    }
}
