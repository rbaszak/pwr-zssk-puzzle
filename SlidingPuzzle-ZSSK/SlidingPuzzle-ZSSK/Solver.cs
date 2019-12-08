using SlidingPuzzle_ZSSK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public class Solver
{
    public enum dir {UP = 1, DOWN = 2, LEFT = 3, RIGHT = 4};
    public int size;
    public int[,] array;            //Initial shuffled array
    public int[,] result;           //Result array (should be 1,2,3...)
    public List<string> states = new List<string>();
    public List<string> path = new List<string>();          //Current/result path
    private static Mutex mutex = new Mutex();
    static bool equal = false;
    //public List<List<int>> paths = new List<List<int>>();   //List of tested paths

    public void SetArrayToSolve(int[,] arr, int gridSize)
    {
        this.array = arr;
        this.result = arr;
        this.size = gridSize;
    }

    public int[,] GetResultArray()
    {
        return result;
    }

    public List<string> GetResultPath()
    {
        return path;
    }

    public void BruteForceSingleCore(string newStart)
    {
        List<int> zeros = new List<int>();
        List<string> toAdd = new List<string>();
        List<string> newlyAdded = new List<string>();
        newlyAdded.Add(newStart);
        string goal = "012345678";
        equal = goal.Equals(result);

        #region Generating states

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        while (!equal)
        {
            foreach (var state in newlyAdded)
            {
                zeros = GetAdjacentToEmpty(state);

                foreach (var item in zeros)
                {
                    if (equal)
                    {
                        break;
                    }

                    string nextState = CalculateState(state, item);

                    int output = states.IndexOf(nextState);
                    if (output == -1)
                        toAdd.Add(nextState);

                    mutex.WaitOne();
                    equal = goal.Equals(nextState);
                    if (equal)
                    {
                        mutex.ReleaseMutex();

                        result = FromState(nextState);
                        Console.WriteLine("DONE MULTICORE");
                        break;
                    }
                    else mutex.ReleaseMutex();
                }
                if (equal)
                {
                    break;
                }

            }
            if (equal)
            {
                break;
            }
            mutex.WaitOne();
            states.AddRange(toAdd);
            mutex.ReleaseMutex();

            newlyAdded.Clear();
            newlyAdded.AddRange(toAdd);
            toAdd.Clear();

        }

        stopwatch.Stop();
        var time = stopwatch.Elapsed.TotalMilliseconds;
        Console.WriteLine("Time elapsed (milliseconds): " + time);

        #endregion
    }

    public void BruteForceBFSSingleCore()
    {
        string goal;

        #region Goal definition
        if (size == 3)
        {
            goal = "012345678";
        }
        else if (size == 4)
        {
            goal = "0123456789101112131415";
        }
        else
        {
            goal = "0123456789101112131415161718192021222324";
        }
        #endregion

        List<int> zeros = new List<int>();
        List<string> newlyAdded = new List<string>();

        equal = goal.Equals(ToState(result));

        zeros = GetAdjacentToEmpty(ToState(result));

        states.Add(ToState(result));

        bool newStates = false;

        while (!equal)
        {
            List<string> toAdd = new List<string>();

            if (newStates)
            {
                foreach (var state in newlyAdded)
                {
                    zeros = GetAdjacentToEmpty(state);

                    foreach (var item in zeros)
                    {
                        string nextState = CalculateState(state, item);
                        //Console.WriteLine(nextState);

                        int output = states.IndexOf(nextState);
                        if (output == -1)
                            toAdd.Add(nextState);

                        equal = goal.Equals(nextState);
                        if (equal)
                        {
                            result = FromState(nextState);
                            Console.WriteLine("DONE");
                            break;
                        }
                    }
                    if (equal)
                    {
                        break;
                    }

                }
                states.AddRange(toAdd);
                newlyAdded.Clear();
                newlyAdded.AddRange(toAdd);
                toAdd.Clear();
            }
            else
            {
                foreach (var state in states)
                {
                    zeros = GetAdjacentToEmpty(state);

                    foreach (var item in zeros)
                    {
                        string nextState = CalculateState(state, item);
                        //Console.WriteLine(nextState);

                        int output = states.IndexOf(nextState);
                        if (output == -1)
                            toAdd.Add(nextState);

                        equal = goal.Equals(nextState);
                        if (equal)
                        {
                            result = FromState(nextState);
                            Console.WriteLine("DONE");
                            break;
                        }
                    }
                    if (equal)
                    {
                        break;
                    }

                }
                states.AddRange(toAdd);
                newlyAdded.AddRange(toAdd);
                newStates = true;
                toAdd.Clear();
            }
            if (equal)
            {
                break;
            }

        }
    }

    public void BruteForceBFSMultiCore()
    {
        string goal;

        #region Goal definition
        if (size == 3)
        {
            goal = "012345678";
        }
        else if (size == 4)
        {
            goal = "0123456789101112131415";
        }
        else
        {
            goal = "0123456789101112131415161718192021222324";
        }
        #endregion

        #region Generating states
            List<int> zeros = new List<int>();
            List<string> newlyAdded = new List<string>();
            equal = goal.Equals(ToState(result));
            List<string> toAdd = new List<string>();
            states.Add(ToState(result));

            foreach (var state in states)
            {

                zeros = GetAdjacentToEmpty(state);

                foreach (var item in zeros)
                {
                    string nextState = CalculateState(state, item);

                    int output = states.IndexOf(nextState);
                    if (output == -1)
                        toAdd.Add(nextState);

                    equal = goal.Equals(nextState);
                    if (equal)
                    {
                        result = FromState(nextState);
                        Console.WriteLine("DONE");
                        break;
                    }
                }
                if (equal)
                {
                    break;
                }

            }
            states.AddRange(toAdd);
            newlyAdded.AddRange(toAdd);
            #endregion

            Console.WriteLine("Cores count: " + newlyAdded.Count);
          
            if (newlyAdded.Count == 2)
            {

                Thread thr1 = new Thread(() => BruteForceSingleCore(newlyAdded[0]));
                Thread thr2 = new Thread(() => BruteForceSingleCore(newlyAdded[1]));

                thr1.Start();
                thr2.Start();

                //thr1.Join();
                //thr2.Join();
            }
            else if (newlyAdded.Count == 4)
            {
                Thread thr1 = new Thread(() => BruteForceSingleCore(newlyAdded[0]));
                Thread thr2 = new Thread(() => BruteForceSingleCore(newlyAdded[1]));
                Thread thr3 = new Thread(() => BruteForceSingleCore(newlyAdded[2]));
                Thread thr4 = new Thread(() => BruteForceSingleCore(newlyAdded[3]));

                thr1.Start();
                thr2.Start();
                thr3.Start();
                thr4.Start();

                //thr1.Join();
                //thr2.Join();
                //thr3.Join();
                //thr4.Join();
            }
            else
            {
                Thread thr1 = new Thread(() => BruteForceSingleCore(newlyAdded[0]));
                Thread thr2 = new Thread(() => BruteForceSingleCore(newlyAdded[1]));
                Thread thr3 = new Thread(() => BruteForceSingleCore(newlyAdded[2]));


                thr1.Start();
                thr2.Start();
                thr3.Start();

               // thr1.Join();
            //thr2.Join();
            //thr3.Join();

        }
    }

    public void Move(int a, dir direction) //Move tile 'a' in a defined direction
    {
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

    public string CalculateState(string state, int b)
    {
        string nextState = "";
        string c = b.ToString();

        int index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if(state[index].Equals('8'))
                {
                    nextState += b.ToString();
                }
                else if(state[index].ToString().Equals(c))
                {
                    nextState += "8";
                }
                else
                {
                    nextState += state[index];
                }
                index++;
            }
        }

        return nextState;
    }

    public List<int> GetAdjacentToEmpty(string state) //Returns list of numbers of tiles adjacent to empty tile
    {
        var items = new List<int>();

        var arr = FromState(state);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (arr[j, i] == 8)
                {
                    if((j-1) >= 0)
                    {
                        items.Add(arr[j - 1, i]);
                    }

                    if((j+1) < size)
                    {
                        items.Add(arr[j + 1, i]);
                    }

                    if((i-1) >= 0)
                    {
                        items.Add(arr[j, i - 1]);
                    }

                    if((i+1) < size)
                    {
                        items.Add(arr[j, i + 1]);
                    }
                    break;
                }
            }
        }
        return items;
    }

    public string ToState(int[,] array)
    {
        string stateString = "";

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                stateString += array[i,j];
            }
        }

        return stateString;
    }

    public int[,] FromState(string state)
    {
        int[,] stateArray = new int[size,size];

        int index = 0;
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                stateArray[i, j] = int.Parse(state[index].ToString());
                index++;
            }
        }

        return stateArray;
    }
}