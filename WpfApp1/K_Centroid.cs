using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class K_Centroid
    {
        private static int[] InitClustering(int dataLen, int k)
        {
            Random random = new Random();
            int[] clustering = new int[dataLen];
            for (int i = 0; i < dataLen; i++)
                clustering[i] = random.Next(0, k);
            return clustering;
        }

        public static List<(int, int)> CreateData()
        {
            int[,] map = new int[10001, 10001];
            List<(int, int)> data = new List<(int, int)>(40020);
            List<(int, int)> initPoints = new List<(int, int)>(20);
            Random r = new Random();
            int x, y;
            for (int i = 0; i < 20; i++)
            {
                x = r.Next(9901);
                y = r.Next(9901);
                while (map[y, x] == 1)
                {
                    x = r.Next(9901);
                    y = r.Next(9901);
                }
                map[y, x] = 1;
                (int, int) point;
                point.Item1 = y;
                point.Item2 = x;
                initPoints.Add(point);
                data.Add(point);
            }
            int X_offset, Y_offset, rndInd;
            for (int i = 0; i < 40000; i++)
            {
                rndInd = r.Next(20);
                y = initPoints[rndInd].Item1;
                x = initPoints[rndInd].Item2;
                X_offset = r.Next(-100, 100);
                Y_offset = r.Next(-100, 100);
                while (y + Y_offset < 0 || y + Y_offset >= 10001 || x + X_offset < 0 || x + X_offset >= 10001)
                {
                    X_offset = r.Next(-100, 100);
                    Y_offset = r.Next(-100, 100);
                }
                (int, int) point;
                point.Item1 = y + Y_offset;
                point.Item2 = x + X_offset;
                map[y + Y_offset, x + X_offset] = 1;
                data.Add(point);
            }
            return data;
        }

        public static (int[], List<(int, int)>) UpdateClustering(List<(int, int)> centers, int[] clustering, List<(int, int)> data)
        {
            int dataLen = clustering.Length;
            int[] newClustering;
            List<(int, int)> newCenters = new List<(int, int)>();
            int[] clasterCounts = new int[centers.Count];
            int[] sumX = new int[centers.Count];
            int[] sumY = new int[centers.Count];
            for (int i = 0; i < dataLen; i++)
            {
                clasterCounts[clustering[i]]++;
                sumX[clustering[i]] += data[i].Item2;
                sumY[clustering[i]] += data[i].Item1;
            }
            for (int i = 0; i < centers.Count; i++)
            {
                (int, int) newCenter;
                if (clasterCounts[i] > 0)
                {
                    newCenter.Item2 = (int)(sumX[i] / clasterCounts[i]);
                    newCenter.Item1 = (int)(sumY[i] / clasterCounts[i]);
                    newCenters.Add(newCenter);
                }
                else
                {
                    newCenters.Add(centers[i]);
                }
            }
            newClustering = CalcNewClustering(newCenters, data);
            return (newClustering, newCenters);
        }

        public static int[] CalcNewClustering(List<(int, int)> centers, List<(int, int)> data)
        {
            int minIndex = 0;
            int minDist;
            int distance;
            int index = 0;
            int[] newClustering = new int[data.Count];
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
                newClustering[index++] = minIndex;
                minIndex = 0;
            }
            return newClustering;
        }

        public static int CalcDistance((int, int) center, (int, int) point)
        {
            int squaredDistance = 0;
            squaredDistance += (int)(Math.Pow((center.Item1 - point.Item1), 2) + Math.Pow((center.Item2 - point.Item2), 2));
            return squaredDistance;
        }

        public static bool SameCenters(List<(int, int)> prevCenters, List<(int, int)> newCenters)
        {
            for (int i = 0; i < prevCenters.Count; i++)
            {
                if (prevCenters[i] != newCenters[i]) return false;
            }
            return true;
        }

        public static (List<(int, int)>, int[], List<(int, int)>) Start(int k)
        {
            Random r = new Random();
            List<(int, int)> data = CreateData();
            int[] clustering = InitClustering(data.Count, k);
            List<(int, int)> centers = new List<(int, int)>();
            int x, y;
            for (int i = 0; i < k; i++)
            {
                x = r.Next(10002);
                y = r.Next(10002);
                (int, int) center;
                center.Item1 = y;
                center.Item2 = x;
                centers.Add(center);
            }
            List<(int, int)> prevCenters = new List<(int, int)>();
            for (int i = 0; i < centers.Count; i++)
            {
                prevCenters.Add(centers[i]);
            }
            var updatedCenter = UpdateClustering(centers, clustering, data);
            while (!SameCenters(prevCenters, updatedCenter.Item2))
            {
                clustering = updatedCenter.Item1;
                centers = updatedCenter.Item2;
                updatedCenter = UpdateClustering(centers, clustering, data);
                prevCenters.Clear();
                for (int i = 0; i < centers.Count; i++)
                {
                    prevCenters.Add(centers[i]);
                }
            }
            Console.WriteLine();
            return (data, clustering, centers);
        }
    }
}
