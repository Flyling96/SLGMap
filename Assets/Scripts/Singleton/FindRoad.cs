using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindRoad : Singleton<FindRoad> {

    public List<HexCell> Dijkstra(HexCell start,HexCell end,List<HexCell> cells)
    {
        int[,] distance = new int[cells.Count,cells.Count];
        int[] distanceToStart = new int[cells.Count];
        int[] previous = new int[cells.Count];
        int startIndex = cells.IndexOf(start);
        Stack<int> resultInt = new Stack<int>();
        List<int> alreadyThrough = new List<int>();
        for (int i =0;i<cells.Count;i++)
        {
            for(int j=0;j<cells.Count;j++)
            {
                distance[i,j] = int.MaxValue;
            }
            previous[i] = startIndex;
        }

        distance[startIndex, startIndex] = 0;
        for (int i=0;i<cells.Count;i++)
        {
            for(int j=(int)HexDirection.NE;j<=(int)HexDirection.NW;j++)
            {
                if(cells.Contains(cells[i].GetNeighbor((HexDirection)j)))
                {
                    distance[i, cells.IndexOf(cells[i].GetNeighbor((HexDirection)j))] = cells[i].coordinates.DistanceToOther(cells[i].GetNeighbor((HexDirection)j).coordinates);
                }
            }
        }

        for(int i=0;i<cells.Count;i++)
        {
            distanceToStart[i] = distance[startIndex, i];
        }


        for(int i = 0;i< cells.Count-1;i++)
        {
            int min = int.MaxValue;
            int point = 0;
            for(int j=1;j<cells.Count;j++)
            {
                if(!alreadyThrough.Contains(j) && distanceToStart[j]<min)
                {
                    min = distanceToStart[j];
                    point = j;
                }
            }
            alreadyThrough.Add(point);

            for (int j=0;j<cells.Count;j++)
            {
                if(distance[point,j]<int.MaxValue)
                {
                    if(distanceToStart[j]>distanceToStart[point]+distance[point,j])
                    {
                        distanceToStart[j] = distanceToStart[point] + distance[point, j];
                        previous[j] = point;
                    }
                }
            }
        }

        int previousIndex = cells.IndexOf(end);
        while (previous[previousIndex]!= startIndex)
        {
            resultInt.Push(previousIndex);
            previousIndex = previous[previousIndex];
        }
        resultInt.Push(previousIndex);

        Debug.Log(distanceToStart[cells.IndexOf(end)]);

        List<HexCell> result = new List<HexCell>();

        result.Add(start);
        while(resultInt.Count!=0)
        {
            result.Add(cells[resultInt.Pop()]);
        }

        return result;
    }


    public void BFS(HexCell start,HexCell end,List<HexCell> cells)
    {
        Queue<int> BfsQueue = new Queue<int>();
    }
}
