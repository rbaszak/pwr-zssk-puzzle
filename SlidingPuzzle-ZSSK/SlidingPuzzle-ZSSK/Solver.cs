using System.Collections.Generic;

public class Solver
{
    public int size;
    public int[,] array;            //Initial shuffled array
    public int[,] result;           //Result array (should be 1,2,3...)
    public List<int> path;          //Current/result path
    public List<List<int>> paths;   //List of tested paths

    public void SetArrayToSolve(int[,] arr, int gridSize)
    {
        this.array = arr;
        this.size = gridSize;
    }

    public int[,] GetResultArray()
    {
        return result;
    }

    public List<int> GetResultPath()
    {
        return path;
    }
    public void BruteForceDFS()
    {
        //To implement
    }
    public void BruteForceBFS()
    {
        //To implement
    }
    public void DivideAndConquer()
    {
        //To implement
    }

}