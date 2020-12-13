using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class K_Medoid
    {
        public static List<(int, int)> InitCenters(List<(int, int)> data, int k)
        {
            Random r = new Random();
            int index;
            List<(int, int)> centers = new List<(int, int)>();
            for (int i = 0; i < k; i++)
            {
                index = r.Next(data.Count);
                while (centers.Contains(data[index]))
                {
                    index = r.Next(data.Count);
                }
                centers.Add(data[index]);
                Console.WriteLine(data[index].Item1 + " " + data[index].Item2);
            }
            return centers;
        }

        public static int CalcDistance((int, int) center, (int, int) point)
        {
            int distance = 0;
            distance += (int)(Math.Pow((center.Item1 - point.Item1), 2) + Math.Pow((center.Item2 - point.Item2), 2));
            return (int)Math.Sqrt(distance);
        }

        public static int CalcTotalCost(List<(int, int)> data, List<(int, int)> centers, int[] clustering)
        {
            int totalCost = 0;
            for (int i = 0; i < centers.Count; i++)
            {
                for(int j = 0; j < data.Count; j++)
                {
                    if (clustering[j] == i)
                    {
                        totalCost += CalcDistance(centers[i], data[j]);
                    }
                }
            }
            return totalCost;
        }
        public static List<(int, int)> UpdateClustering(List<(int, int)> data, List<(int, int)> centers)
        {
            int newCost;
            int bestCost;
            int[] bestTotalCost = new int[centers.Count];
            int[] bestIndex = new int[centers.Count];
            int[] currClustering;
            int[] newClustering;
            List<(int, int)> newCenters = new List<(int, int)>();
            int bestTotalIndex;
            int minTotalCost;

            while (true)
            {
                currClustering = SetClustering(data, centers);
                minTotalCost = CalcTotalCost(data, centers, currClustering);
                for (int i = 0; i < centers.Count; i++)
                {
                    bestCost = minTotalCost;
                    for (int j = 0; j < data.Count; j++)
                    {
                        if (centers.Contains(data[j])) continue;
                        if (currClustering[j] != i) continue;

                        for (int k = 0; k < centers.Count; k++)
                        {
                            newCenters.Add(centers[k]);
                        }
                        newCenters[i] = data[j];
                        newClustering = SetClustering(data, newCenters);
                        newCost = CalcTotalCost(data, newCenters, newClustering);

                        if (newCost < bestCost)
                        {
                            bestCost = newCost;
                            bestIndex[i] = j;
                        }
                        newCenters.Clear();
                    }
                }

                for (int i = 0; i < centers.Count; i++)
                {
                    for (int k = 0; k < centers.Count; k++)
                    {
                        newCenters.Add(centers[k]);
                    }
                    newCenters[i] = data[bestIndex[i]];
                    newClustering = SetClustering(data, newCenters);
                    bestTotalCost[i] = CalcTotalCost(data, newCenters, newClustering);
                    newCenters.Clear();
                }
                bestTotalIndex = -1;
                for(int i = 0; i < centers.Count; i++)
                {
                    if (bestTotalCost[i] < minTotalCost)
                    {
                        minTotalCost = bestTotalCost[i];
                        bestTotalIndex = i;
                    }
                }

                if (bestTotalIndex == -1)
                {
                    break;
                }

                centers[bestTotalIndex] = data[bestIndex[bestTotalIndex]];
                Console.WriteLine("!!!");
            }
            return centers;
        }

        public static int[] SetClustering(List<(int, int)> data, List<(int, int)> centers)
        {
            int minDist = 0;
            int distance = 0;
            int minIndex = 0;
            int index = 0;
            int[] clustering = new int[data.Count];

            foreach (var point in data)
            {
                minDist = CalcDistance(centers[0], point);
                for (int i = 0; i < centers.Count; i++)
                {
                    distance = CalcDistance(centers[i], point);
                    if (distance < minDist)
                    {
                        minDist = distance;
                        minIndex = i;
                    }
                }
                clustering[index++] = minIndex;
                minIndex = 0;
            }
            return clustering;
        }

        public static (List<(int, int)>, int[], List<(int, int)>) Start(int k, List<(int, int)> data)
        {
            Random r = new Random();
            List<(int, int)> centers = InitCenters(data, k);

            centers = UpdateClustering(data, centers);
            int[] finalClustering = SetClustering(data, centers);

            return (data, finalClustering, centers);
        }
    }
}
