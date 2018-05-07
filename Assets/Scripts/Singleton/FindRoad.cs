using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FindRoad : Singleton<FindRoad> {



    /// <summary>
    /// 曼哈顿距离
    /// </summary>
    public float ManhattanDistance(HexCell start, HexCell end)
    {
        return Mathf.Abs(start.coordinates.X - end.coordinates.X) + Mathf.Abs(start.coordinates.Y - end.coordinates.Y);
    }

    /// <summary>
    /// 欧几里得距离
    /// </summary>
    public  float EuclideanDistance(HexCell start, HexCell end)
    {
        float disX = start.transform.position.x - end.transform.position.x;
        float disY = start.transform.position.z - end.transform.position.z;
        return Mathf.Sqrt(disX * disX + disY * disY);
    }

    /// <summary>
    /// 八方向距离
    /// </summary>
    public float EightDirectionsDistance(HexCell start, HexCell end)
    {
        int disX = Mathf.Abs(start.coordinates.X - end.coordinates.X);
        int disY = Mathf.Abs(start.coordinates.Z - end.coordinates.Z);
        if (disX > disY) return 1.414213562f * disY + (disX - disY);
        return 1.414213562f * disX + (disY - disX);
    }

    /// <summary>
    /// 六方向距离
    /// </summary>
    public float SixDirectsDistance(HexCell start, HexCell end)
    {
        return start.coordinates.DistanceToOther(end.coordinates);
    }



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

    public float h = 1;

    int[,] distance;
    public void Init(List<HexCell> cells)
    {
        distance = new int[cells.Count, cells.Count]; 
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells.Count; j++)
            {
                if (i == j)
                {
                    distance[i, j] = 0;
                }
                else
                {
                    distance[i, j] = int.MaxValue;
                }
            }

            for (int j = (int)HexDirection.NE; j <= (int)HexDirection.NW; j++)
            {
                if (cells.Contains(cells[i].GetNeighbor((HexDirection)j)))
                {
                    // distance[i, cells.IndexOf(cells[i].GetNeighbor((HexDirection)j))] = cells[i].coordinates.DistanceToOther(cells[i].GetNeighbor((HexDirection)j).coordinates);
                    distance[i, cells.IndexOf(cells[i].GetNeighbor((HexDirection)j))] = CellDistance(cells[i], (HexDirection)j);
                }
            }
        }
    }

    public List<HexCell> AStar(HexCell start,HexCell end,List<HexCell> cells)
    {
        float time = Time.realtimeSinceStartup;
        int[] distanceToStart = new int[cells.Count];
        int[] previous = new int[cells.Count];
        float[] cost = new float[cells.Count];
        int startIndex = cells.IndexOf(start);
        PriorityQueue<KeyValuePair<int, float>> queue = new PriorityQueue<KeyValuePair<int, float>>(new disCompare());//优先队列

        Stack<int> resultInt = new Stack<int>();
        for (int i = 0; i < cells.Count; i++)
        {
            previous[i] = startIndex;
            cost[i] = float.MaxValue;
        }


        for (int i = 0; i < cells.Count; i++)
        {
            distanceToStart[i] = distance[startIndex, i];
            if (distanceToStart[i] < int.MaxValue)
            {
                cost[i] = distance[startIndex, i] ;
            }
            queue.Push(new KeyValuePair<int, float>(i, cost[i]));
        }


        for (int i = 0; i < cells.Count - 1; i++)
        {
            float min = queue.Top().Value;
            int point = queue.Top().Key;
            queue.Pop();

            if (point == cells.IndexOf(end))
            {
                break;
            }

            for (int j = 0; j < cells.Count; j++)
            {
                if (distance[point, j] < int.MaxValue)
                {
                    if (distanceToStart[j] > distanceToStart[point] + distance[point, j])
                    {
                        distanceToStart[j] = distanceToStart[point] + distance[point, j];
                        previous[j] = point;
                        cost[j] = distanceToStart[j] + SixDirectsDistance(cells[j], end)* h;
                        queue.Push(new KeyValuePair<int, float>(j, cost[j]));
                    }
                }
            }
        }

        int previousIndex = cells.IndexOf(end);
        while (previous[previousIndex] != startIndex)
        {
            resultInt.Push(previousIndex);
            previousIndex = previous[previousIndex];
        }
        resultInt.Push(previousIndex);

        List<HexCell> result = new List<HexCell>();

        result.Add(start);
        while (resultInt.Count != 0)
        {
            result.Add(cells[resultInt.Pop()]);
        }
        Debug.Log(Time.realtimeSinceStartup - time);
        return result;
    }







    public class disCompare : IComparer<KeyValuePair<int, float>>
    {
        public int Compare(KeyValuePair<int, float> x, KeyValuePair<int, float> y)
        {
            if (x.Value > y.Value)
                return -1;
            else
                return 1;
        }
    }

    /// <summary>
    /// 基于二叉堆的优先队列
    /// </summary>
    public class PriorityQueue<T>
    {
        IComparer<T> comparer;
        T[] heap;

        public int Count { get; private set; }

        public PriorityQueue() : this(null) { }
        public PriorityQueue(int capacity) : this(capacity, null) { }
        public PriorityQueue(IComparer<T> comparer) : this(16, comparer) { }

        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
            this.heap = new T[capacity];
        }

        public void Push(T v)
        {
            if (Count >= heap.Length) Array.Resize(ref heap, Count * 2);
            heap[Count] = v;
            SiftUp(Count++);
        }

        public T Pop()
        {
            var v = Top();
            heap[0] = heap[--Count];
            if (Count > 0) SiftDown(0);
            return v;
        }

        public T Top()
        {
            if (Count > 0) return heap[0];
            throw new InvalidOperationException("优先队列为空");
        }

        void SiftUp(int n)
        {
            var v = heap[n];
            for (var n2 = n / 2; n > 0 && comparer.Compare(v, heap[n2]) > 0; n = n2, n2 /= 2) heap[n] = heap[n2];
            heap[n] = v;
        }

        void SiftDown(int n)
        {
            var v = heap[n];
            for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2)
            {
                if (n2 + 1 < Count && comparer.Compare(heap[n2 + 1], heap[n2]) > 0) n2++;
                if (comparer.Compare(v, heap[n2]) >= 0) break;
                heap[n] = heap[n2];
            }
            heap[n] = v;
        }
    }


    int CellDistance(HexCell cell, HexDirection dir)
    {
        if(cell.GetEdgeType(cell.isStepDirection[(int)dir],dir)==HexEdgeType.Cliff)
        {
            return int.MaxValue;
        }
        else
        {
            return 1;
        }
    }
}
