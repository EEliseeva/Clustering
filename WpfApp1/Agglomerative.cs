using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Agglomerative
    {

        public static int Size = 5001;//10001;
        public static int Offset = 50; //100
        public static int NumOfPoints = 20000; //40000
        public static int RandPoints = 20;

        public static short[,] DistMatrix;
        public static List<List<(int, int)>> Clusters;

        public static List<(int, int)> CreateData()
        {
            int[,] map = new int[Size + 1, Size + 1];
            List<(int, int)> data = new List<(int, int)>(NumOfPoints + RandPoints);
            List<(int, int)> initPoints = new List<(int, int)>(RandPoints);
            Random r = new Random();
            int x, y;
            for (int i = 0; i < RandPoints; i++)
            {
                x = r.Next(Size + 1);
                y = r.Next(Size + 1);
                while (map[y, x] == 1)
                {
                    x = r.Next(Size + 1);
                    y = r.Next(Size + 1);
                }
                map[y, x] = 1;
                (int, int) point;
                point.Item1 = y;
                point.Item2 = x;
                initPoints.Add(point);
                data.Add(point);
            }
            int X_offset, Y_offset, rndInd;
            for (int i = 0; i < NumOfPoints; i++)
            {
                rndInd = r.Next(RandPoints);
                y = initPoints[rndInd].Item1;
                x = initPoints[rndInd].Item2;
                X_offset = r.Next(-Offset, Offset);
                Y_offset = r.Next(-Offset, Offset);
                while (y + Y_offset < 0 || y + Y_offset >= Size || x + X_offset < 0 || x + X_offset >= Size)
                {
                    X_offset = r.Next(-Offset, Offset);
                    Y_offset = r.Next(-Offset, Offset);
                }
                (int, int) point;
                point.Item1 = y + Y_offset;
                point.Item2 = x + X_offset;
                data.Add(point);
            }
            return data;
        }

        public static short CalcDistancePoints((int, int) center, (int, int) point)
        {
            int distance = 0;
            distance += (int)(Math.Pow((center.Item1 - point.Item1), 2) + Math.Pow((center.Item2 - point.Item2), 2));
            return Convert.ToInt16(Math.Sqrt(distance));
        }

        public static short CalcDistanceClusters(List<(int, int)> first, List<(int, int)> second)
        {
            int[] clasterCounts = new int[2];
            int[] sumX = new int[2];
            int[] sumY = new int[2];
            for (int i = 0; i < first.Count; i++)
            {
                clasterCounts[0]++;
                sumX[0] += first[i].Item2;
                sumY[0] += first[i].Item1;
            }
            for (int i = 0; i < second.Count; i++)
            {
                clasterCounts[1]++;
                sumX[1] += second[i].Item2;
                sumY[1] += second[i].Item1;
            }
            (int, int) firstCenter, secondCenter;
            firstCenter.Item1 = (int)(sumY[0] / clasterCounts[0]);
            firstCenter.Item2 = (int)(sumX[0] / clasterCounts[0]);

            secondCenter.Item1 = (int)(sumY[1] / clasterCounts[1]);
            secondCenter.Item2 = (int)(sumX[1] / clasterCounts[1]);

            return CalcDistancePoints(firstCenter, secondCenter);
        }

        public static void CreateFirstMatrix(List<(int, int)> data)
        {

            for (int i = 0; i < DistMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < DistMatrix.GetLength(1); j++)
                {
                    DistMatrix[i, j] = -1;
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                List<(int, int)> newCluster = new List<(int, int)>();
                newCluster.Add(data[i]);
                Clusters.Add(newCluster);
                for (int j = 0; j < i; j++)
                {
                    if (i == j) continue;
                    DistMatrix[i, j] = CalcDistancePoints(data[i], data[j]);
                }
            }
        }

        public static void CreateClusterMatrix()
        {
            for (int i = 0; i < DistMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < DistMatrix.GetLength(1); j++)
                {
                    DistMatrix[i, j] = -1;
                }
            }
            for (int i = 0; i < Clusters.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (i == j) continue;
                    DistMatrix[i, j] = CalcDistanceClusters(Clusters[i], Clusters[j]);
                }
            }
        }

        public static void UpdateClusters()
        {
            int minDist = DistMatrix[1, 0];
            List<(int, int)> minIndex = new List<(int, int)>();
            minIndex.Add((1,0));
            List<List<(int, int)>> minClusters = new List<List<(int, int)>>();
            for (int i = 0; i < Clusters.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (i == j) continue;
                    if (DistMatrix[i, j] < minDist && DistMatrix[i, j] != -1)
                    {
                        minDist = DistMatrix[i, j];
                        minIndex.Clear();
                        minIndex.Add((i, j));
                    }
                }
            }

            minClusters.Add(Clusters[minIndex[0].Item1]);
            minClusters.Add(Clusters[minIndex[0].Item2]);

            for (int i = 0; i < Clusters.Count; i++)
            {
                DistMatrix[minIndex[0].Item1, i] = -1;
                DistMatrix[i, minIndex[0].Item2] = -1;
                DistMatrix[minIndex[0].Item2, i] = -1;
                DistMatrix[i, minIndex[0].Item1] = -1;
            }

            for (int i = 0; i < Clusters.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (i == j) continue;
                    if (DistMatrix[i, j] == minDist && minIndex[0] != (i, j))
                    {
                        for (int k = 0; k < Clusters.Count; k++)
                        {
                            DistMatrix[i, k] = -1;
                            DistMatrix[k, j] = -1;
                            DistMatrix[j, k] = -1;
                            DistMatrix[k, i] = -1;
                        }
                        minIndex.Add((i, j));
                        minClusters.Add(Clusters[i]);
                        minClusters.Add(Clusters[j]);
                    }
                }
            }

            foreach (var index in minIndex)
            {
                List<(int, int)> newCluster = new List<(int, int)>();
                foreach(var point in Clusters[index.Item1])
                {
                    newCluster.Add(point);
                }
                foreach (var point in Clusters[index.Item2])
                {
                    newCluster.Add(point);
                }
                Clusters.Add(newCluster);
            }

            foreach (var cluster in minClusters)
            {
                Clusters.Remove(cluster);
            }
        }

        public static List<List<(int, int)>> Start()
        {
            DistMatrix = new short[NumOfPoints + RandPoints, NumOfPoints + RandPoints];
            Clusters = new List<List<(int, int)>>(NumOfPoints + RandPoints);
            Random r = new Random();
            List<(int, int)> data = CreateData();
            CreateFirstMatrix(data);
            UpdateClusters();
            int numberOfClust = 7;
            while(Clusters.Count > numberOfClust)
            {
                CreateClusterMatrix();
                UpdateClusters();
                Console.WriteLine(Clusters.Count);
            }
            return Clusters;
        }
    }
}
