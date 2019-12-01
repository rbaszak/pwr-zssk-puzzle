using System;
using System.Collections.Generic;

public class Solver
{
    public enum dir {UP = 1, DOWN = 2, LEFT = 3, RIGHT = 4};
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
        int[,] set = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
        result = array;
        while(array != set)
        {

        }
    }

    public void Move(int a, dir direction) //Move tile 'a' in a defined direction
    {
        result = array;
        int[,] temp = result;
        int coordsX = 999;
        int coordsY = 999;
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                if(temp[i,j] == a)
                {
                    coordsY = i;
                    coordsX = j;
                    break;
                }
            }
        }

        int tempVal = 999;

        if(direction == dir.UP)
        {
            try
            {
                tempVal = temp[coordsY - 1, coordsX];
                temp[coordsY - 1, coordsX] = a;
                temp[coordsY, coordsX] = tempVal;
            }
            catch
            {
                Console.WriteLine("Nie można pójść dalej.");
            }
        }
        if(direction == dir.DOWN)
        {
            try
            {
                tempVal = temp[coordsY + 1, coordsX];
                temp[coordsY + 1, coordsX] = a;
                temp[coordsY, coordsX] = tempVal;
            }
            catch
            {
                Console.WriteLine("Nie można pójść dalej.");
            }
        }
        if(direction == dir.LEFT)
        {
            try
            {
                tempVal = temp[coordsY, coordsX - 1];
                temp[coordsY, coordsX - 1] = a;
                temp[coordsY, coordsX] = tempVal;
            }
            catch
            {
                Console.WriteLine("Nie można pójść dalej.");
            }
        }
        if(direction == dir.RIGHT)
        {
            try
            {
                tempVal = temp[coordsY, coordsX + 1];
                temp[coordsY, coordsX + 1] = a;
                temp[coordsY, coordsX] = tempVal;
            }
            catch
            {
                Console.WriteLine("Nie można pójść dalej.");
            }
        }
        result = temp;
    }

    public void GetConnectedToZero()
    {

    }
}