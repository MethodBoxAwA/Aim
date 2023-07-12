namespace CSharpTest
{
    class Program
    {
        static int[] QuickSort(int[] arr, int left, int right)
        {
            if (left < right)
            {
                int pivot = Partition(arr, left, right);
                QuickSort(arr, left, pivot - 1);
                QuickSort(arr, pivot + 1, right);
            }
            return arr;

            (int a,int b) = (114,514);
        }

        static void Main(string[] args)
        {
            int[] arr = { 5, 4, 3, 2, 1 };
            QuickSort(arr, 0, arr.Length - 1);
            System.Console.WriteLine(arr);
        }
    
}