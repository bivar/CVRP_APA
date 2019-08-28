using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVRP_APA
{
    class Program
    {
        static Instance CreateDistanceTable(string fileName)
        {
            Instance obj;
            string readPath = @"..\..\" + fileName;
            using (StreamReader readFile = new StreamReader(readPath))
            {
                //Gets the name of the file
                string name = readFile.ReadLine();
                name = SetInstance(name);
                //Gets the dimension of the matrix
                string dimension = readFile.ReadLine();
                dimension = SetInstance(dimension);
                //Gets the number of available vehicles
                string vehicles = readFile.ReadLine();
                vehicles = SetInstance(vehicles);
                //Gets the capacity of the vehicles
                string capacity = readFile.ReadLine();
                capacity = SetInstance(capacity);
                //Set the previous values on the object values
                obj = new Instance(name, int.Parse(dimension), 
                                int.Parse(vehicles), int.Parse(capacity));
                //Gets the matrix of distances between each city
                readFile.ReadLine();
                for (int i = 0; i < obj.dimension; i++)
                {
                    string line = readFile.ReadLine();
                    string[] splitLine = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    for(int j = 0; j < splitLine.Length; j++)
                    {
                        obj.costMatrix[i, j] = int.Parse(splitLine[j]);
                    }
                }

            }

            return obj;
        }

        static string SetInstance(string line)
        {
            string[] splitLine = line.Split(':');
            string cleanLine = splitLine[1].Trim(' ');

            return cleanLine;
        }

        static void GreedyStep(Instance entry)
        {
            for(int i = 0; i < entry.dimension; i++)
            {
                //We are only allowed to have the number of routes less or equal to the number of vehicles
                string[] routes = new string[entry.vehicles];
                bool[] visited = new bool[entry.dimension];

                for(int j = 0; j < routes.Length; j++)
                {
                    int capacitySum = 0;

                }

            }
        }

        static void Main(string[] args)
        { 
           Instance firstEntry = CreateDistanceTable("P-n16-k8.txt");
        }

    }

    class Instance
    {
        public string nameFile;
        public int dimension;
        public int vehicles;
        public int capacity;
        public int[,] costMatrix;

        public Instance(string nameFile, int dimension, int vehicles, int capacity)
        {
            this.nameFile = nameFile;
            this.dimension = dimension;
            this.vehicles = vehicles;
            this.capacity = capacity;
            costMatrix = new int[dimension,dimension];
        }
    }

    public class HeapPair
    {
        public int distance;
        public int cityNum;

        public HeapPair(int distance, int cityNum)
        {
            this.distance = distance;
            this.cityNum = cityNum;
        }
    }

    public class MinHeap
    {
        private readonly HeapPair[] _elements;
        private int _size;

        public MinHeap(int size)
        {
            _elements = new HeapPair[size];
        }

        private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
        private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
        private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

        private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
        private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
        private bool IsRoot(int elementIndex) => elementIndex == 0;

        private HeapPair GetLeftChild(int elementIndex) => _elements[GetLeftChildIndex(elementIndex)];
        private HeapPair GetRightChild(int elementIndex) => _elements[GetRightChildIndex(elementIndex)];
        private HeapPair GetParent(int elementIndex) => _elements[GetParentIndex(elementIndex)];

        private void Swap(int firstIndex, int secondIndex)
        {
            var temp = _elements[firstIndex];
            _elements[firstIndex] = _elements[secondIndex];
            _elements[secondIndex] = temp;
        }

        public bool IsEmpty()
        {
            return _size == 0;
        }

        public int Peek()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            return _elements[0].distance;
        }

        public HeapPair Pop()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            var result = _elements[0];
            _elements[0] = _elements[_size - 1];
            _size--;

            ReCalculateDown();

            return result;
        }

        public void Add(int[] element)
        {
            if (_size == _elements.Length)
                throw new IndexOutOfRangeException();

            for(int i = 0; i < element.Length; i++)
            {

                _elements[_size] = new HeapPair(element[i], i);
                _size++;
            }

            ReCalculateUp();
        }

        private void ReCalculateDown()
        {
            int index = 0;
            while (HasLeftChild(index))
            {
                var smallerIndex = GetLeftChildIndex(index);
                if (HasRightChild(index) && GetRightChild(index) < GetLeftChild(index))
                {
                    smallerIndex = GetRightChildIndex(index);
                }

                if (_elements[smallerIndex] >= _elements[index])
                {
                    break;
                }

                Swap(smallerIndex, index);
                index = smallerIndex;
            }
        }

        private void ReCalculateUp()
        {
            var index = _size - 1;
            while (!IsRoot(index) && _elements[index] < GetParent(index))
            {
                var parentIndex = GetParentIndex(index);
                Swap(parentIndex, index);
                index = parentIndex;
            }
        }
    }
}
