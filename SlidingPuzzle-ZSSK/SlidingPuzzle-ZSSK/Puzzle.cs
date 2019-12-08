public class Puzzle
{
    public int size = 0;
    int[,] array;
    public void SetArraySize(int n)
    {
        this.size = n;
        this.array = new int[n, n];
    }

    public void ArrayInit()
    {
        int num = 0;

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                array[i, j] = num;
                num++;
            }
        }
    }
    public int[,] GetArray()
    {
        return array;
    }
}