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
        static void Main(string[] args)
        {
            Instance firstEntry = CreateEntryFile("P-n55-k7.txt");
            GreedyStep(firstEntry, true);
            FirstNeighbour(firstEntry);
            //SecondNeighbour(firstEntry);
        }

        static Instance CreateEntryFile(string fileName)
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

                //Skips the "DEMAND_SECTION:" line
                readFile.ReadLine();
                //Gets the vector of demand from each city
                for (int i = 0; i < obj.dimension; i++)
                {
                    string line = readFile.ReadLine();
                    string[] splitLine = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    obj.demand[i] = int.Parse(splitLine[1]);
                }

                //Skips the empty line
                readFile.ReadLine();
                //Skips the "EDGE_WEIGHT_SECTION" line
                readFile.ReadLine();
                //Gets the matrix of distance between each city
                for (int i = 0; i < obj.dimension; i++)
                {
                    string line = readFile.ReadLine();
                    string[] splitLine = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < splitLine.Length; j++)
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

        static MinHeap CreateMinHeaps(Instance entry)
        {
            MinHeap heaps = new MinHeap(entry.dimension-1);
            int[] elements = new int[entry.dimension-1];

            for (int i = 1; i < entry.dimension; i++)
            {
                elements[i-1] = entry.demand[i];
            }

            heaps.Add(elements);
            return heaps;
        }

        static MaxHeap CreateMaxHeaps(Instance entry)
        {
            MaxHeap heaps = new MaxHeap(entry.dimension-1);
            int[] elements = new int[entry.dimension-1];

            for (int i = 1; i < entry.dimension; i++)
            {
                elements[i-1] = entry.demand[i];
            }

            heaps.Add(elements);
            return heaps;
        }
        
        static void GreedyStep(Instance entry, bool isMin)
        {
            //Distancia total percorrido por todas as rotas
            int totalRoutes = 0;
            int[] routesDistance = new int[entry.vehicles];
            int[] capacitySum = new int[entry.vehicles];
            int[] currentCity = new int[entry.vehicles];

            //We are only allowed to have the number of routes less or equal to the number of vehicles
            List<int>[] routes = new List<int>[entry.vehicles];
            bool[] visited = new bool[entry.dimension];

            MinHeap minheaps = null;
            MaxHeap maxheaps = null;

            if (isMin)
            {
                //Criação do heaps minimo da capcidade em relação a cada cidade
                minheaps = CreateMinHeaps(entry);
            }
            else
            {
                Console.WriteLine("\n COMEÇOU O MAXIMO \n");
                //Criação do heaps maximo da capcidade em relação a cada cidade
                maxheaps = CreateMaxHeaps(entry);
            }
            
            visited[0] = true;
            for (int i = 0; i < routes.Length; i++)
            {
                routes[i] = new List<int>();
                routes[i].Add(0);
            }

            bool allRoutesFull = false;
            while (!allRoutesFull)
            {
                for (int i = 0; i < routes.Length; i++)
                {
                    try
                    {
                        HeapPair city = null;
                        if (isMin)
                        {
                            if((capacitySum[i] + minheaps.Peek()) <= entry.capacity)
                            {
                                city= minheaps.Pop();
                            }
                            else
                            {
                                if (i == routes.Length - 1)
                                {
                                    allRoutesFull = true;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if ((capacitySum[i] + maxheaps.Peek()) <= entry.capacity)
                            {
                                city = maxheaps.Pop();
                            }
                            else
                            {
                                if (i == routes.Length - 1)
                                {
                                    allRoutesFull = true;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        Console.WriteLine("Cidade: " + city.cityNum);
                        Console.WriteLine("Demanda: " + city.demand);
                        routes[i].Add(city.cityNum);
                        capacitySum[i] += city.demand;
                        visited[city.cityNum] = true;
                        routesDistance[i] += entry.costMatrix[currentCity[i], city.cityNum];
                        totalRoutes += entry.costMatrix[currentCity[i], city.cityNum];
                        currentCity[i] = city.cityNum;
                        break;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Todas as cidades já foram visitadas");
                        Console.WriteLine("i: " + i);
                        allRoutesFull = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < routes.Length; i++)
            {
                routes[i].Add(0);
                Console.Write("{");
                foreach (int a in routes[i])
                {
                    Console.Write(a + ", ");

                }
                Console.Write("}");
                Console.WriteLine();
                Console.WriteLine("Distancia dessa rota: " + routesDistance[i]);
                Console.WriteLine("Capacidade dessa rota: " + capacitySum[i]);
            }

            bool allCitiesVisited = true;
            for (int k = 0; k < entry.dimension; k++)
            {   
                if (visited[k] == false)
                {
                    allCitiesVisited = false;
                    Console.WriteLine("Não visitou a cidade: " + k);
                }
            }
            Console.WriteLine("Distancia total: " + totalRoutes);

            if(allCitiesVisited)
            {
                entry.routes = routes;
                entry.routesDistance = routesDistance;
                entry.routesCapacity = capacitySum;
            }
            else
            {
                if (isMin)
                    GreedyStep(entry, false);
            }
            
        }

        static void FirstNeighbour(Instance entry)
        {
            int total = 0;
            for(int i = 0; i < entry.routes.Length; i++)
            {
                //pegando as rotas e as distancias iniciais de cada uma
                List<int> initialRoute = entry.routes[i];
                int initialD = entry.routesDistance[i];

                List<int> finalRoute = null;
                int finalD = int.MaxValue;
                //loop para todas as cidades dentro de cada rota, excluindo a primeira e a ultima
                for (int j = 1; j < (initialRoute.Count - 2); j++)
                {
                    List<int> auxRoute = CloneList(initialRoute);
                    int cityR = auxRoute[j];
                    auxRoute.RemoveAt(j);

                    //Excluir a distancia da cidade escolhida com a sua antecessora e sucessora, além disso somar a distancia entre a antecessora e a sucessora
                    int auxD = initialD - entry.costMatrix[auxRoute[j - 1], cityR] - entry.costMatrix[cityR, auxRoute[j]] + entry.costMatrix[auxRoute[j - 1], auxRoute[j]];

                    int semiFinalD = int.MaxValue;
                    int indexPosition = -1;
                    //Loop para todas as possiveis rotas que a cidade escolhida pode entrar
                    for(int k = 0; k < (auxRoute.Count - 1); k++)
                    {
                        //Excluir a distancia entre as duas cidades da rota escolhida e adicionar a distancia das cidades da rota escolhida para a cidade a entrar na rota 
                        int testD = auxD - entry.costMatrix[auxRoute[k], auxRoute[k+1]] + entry.costMatrix[auxRoute[k], cityR] + entry.costMatrix[cityR, auxRoute[k+1]];

                        if(testD < semiFinalD)
                        {
                            semiFinalD = testD;
                            indexPosition = k;
                        }
                    }

                    if(indexPosition != -1)
                    {
                        auxRoute.Insert(indexPosition + 1, cityR);
                        if (semiFinalD < finalD)
                        {
                            finalRoute = auxRoute;
                            finalD = semiFinalD;
                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                }

                if(finalD < initialD)
                {
                    Console.WriteLine("PRIMEIRO VIZINHO:");
                    entry.routes[i] = finalRoute;
                    Console.Write("{");
                    foreach (int a in entry.routes[i])
                    {
                        Console.Write(a + ", ");
                    }
                    Console.WriteLine();
                    entry.routesDistance[i] = finalD;
                    Console.WriteLine("Distancia final da rota " + i + " :"  + entry.routesDistance[i]);
                }

                total += entry.routesDistance[i];
            }

            Console.WriteLine("Distancia total :" + total);
        }

        static void SecondNeighbour(Instance entry)
        {
            List<int> finalCurrentRoute = null;
            int finalCurrentD = int.MaxValue;
            int finalCurrentC = 0;
            int finalCurrentIndex = 0;

            List<int> finalOtherRoute = null;
            int finalOtherD = int.MaxValue;
            int finalOtherC = 0;
            int finalOtherIndex = 0;

            int finalCredit = int.MaxValue;
            for (int i = 0; i < entry.routes.Length; i++)
            {
                //pegando as rotas e as distancias iniciais de cada uma
                int initialD = entry.routesDistance[i];
                List<int> auxRoute = CloneList(entry.routes[i]);

                //tenho que salvar a rota e distancia tanto da rota que eu tirei a cidade, quanto da rota que eu coloquei a cidade retirada
                int semiFinalCurrentD = int.MaxValue;
                int semiFinalCurrentC = 0;

                List<int> semiFinalOtherRoute = null;
                int semiFinalOtherD = int.MaxValue;
                int semiFinalOtherC = 0;
                int semiFinalOtherIndex = 0;

                int semiFinalCredit = int.MaxValue;
                int currentIndexPosition = -1;
                //loop para todas as cidades dentro de cada rota, excluindo a primeira e a ultima
                for (int j = 1; j < (auxRoute.Count - 1); j++)
                {
                    int cityR = auxRoute[j];

                    //Excluir a distancia da cidade escolhida com a sua antecessora e sucessora, além disso somar a distancia entre a antecessora e a sucessora para pegar seu credito
                    int auxD = initialD - entry.costMatrix[auxRoute[j - 1], cityR] - entry.costMatrix[cityR, auxRoute[j + 1]] + entry.costMatrix[auxRoute[j - 1], auxRoute[j + 1]];
                    int currentCredit = auxD - initialD;

                    //ISSO AQUI SALVA DENTRE TODAS AS OUTRAS ROTAS POSSIVEIS
                    List<int> quarterFinalOtherRoute = null;
                    int quarterFinalOtherD = int.MaxValue;
                    int quarterFinalOtherC = 0;
                    int quarterFinalOtherIndex = 0;

                    int quarterFinalCredit = int.MaxValue;
                    //Loop para todas as outras rotas diferentes da que foi escolhida
                    for (int k = 0; k < entry.routes.Length; k++)
                    {
                        if(k != i)
                        {
                            if (entry.routesCapacity[k] + entry.demand[cityR] <= entry.capacity)
                            {
                                //pegando as cidades outras rotas e sua distancia total
                                List<int> otherRoute = CloneList(entry.routes[k]);
                                int otherD = entry.routesDistance[k];

                                //Variavel para salvar a melhor distancia depois colocar a nova cidade
                                int octavesFinalOtherD = int.MaxValue;
                                int octavesFinalCredit = int.MaxValue;

                                //Variaveis pra saber qual posição da outra rota colocar a cidade removida
                                int otherIndexPosition = -1;
                                //loop entre todas as distancias entre as cidades
                                for (int l = 0; l < (otherRoute.Count - 1); l++)
                                {
                                    //Excluir a distancia entre as duas cidades da rota escolhida e adicionar a distancia das cidades da rota escolhida para a cidade a entrar na rota 
                                    int testD = otherD - entry.costMatrix[otherRoute[l], otherRoute[l + 1]] + entry.costMatrix[otherRoute[l], cityR] + entry.costMatrix[cityR, otherRoute[l + 1]];
                                    int otherCredit = otherD - testD;
                                    int totalCredit = otherCredit + currentCredit;

                                    //comparação de credito pra saber se vale a pena trocar, depois checa se a nova distancia seria melhor do que a salva anteriormente
                                    if ((totalCredit > 0) && (totalCredit < octavesFinalCredit))
                                    {
                                        otherIndexPosition = l;
                                        octavesFinalOtherD = testD;
                                        octavesFinalCredit = totalCredit;
                                    }

                                }

                                if (otherIndexPosition != -1)
                                {
                                    otherRoute.Insert(otherIndexPosition + 1, cityR);

                                    if (octavesFinalCredit < quarterFinalCredit)
                                    {
                                        quarterFinalOtherRoute = otherRoute;
                                        quarterFinalOtherD = octavesFinalOtherD;
                                        quarterFinalOtherC = entry.routesCapacity[k] + entry.demand[cityR];
                                        quarterFinalCredit = octavesFinalCredit;
                                        quarterFinalOtherIndex = k;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    if(quarterFinalCredit < semiFinalCredit && quarterFinalOtherRoute != null)
                    {
                        semiFinalCredit = quarterFinalCredit;

                        semiFinalOtherRoute = quarterFinalOtherRoute;
                        semiFinalOtherD = quarterFinalOtherD;
                        semiFinalOtherC = quarterFinalOtherC;
                        semiFinalOtherIndex = quarterFinalOtherIndex;

                        currentIndexPosition = j;
                        semiFinalCurrentD = auxD;
                        semiFinalCurrentC = entry.routesCapacity[i] - entry.demand[cityR];
                    }
                }

                if(currentIndexPosition != -1)
                {
                    auxRoute.RemoveAt(currentIndexPosition);
                    if(semiFinalCredit < finalCredit)
                    {
                        finalCredit = semiFinalCredit;

                        finalCurrentRoute = auxRoute;
                        finalCurrentD = semiFinalCurrentD;
                        finalCurrentC = semiFinalCurrentC;
                        finalCurrentIndex = i;

                        finalOtherRoute = semiFinalOtherRoute;
                        finalOtherD = semiFinalOtherD;
                        finalOtherC = semiFinalOtherC;
                        finalOtherIndex = semiFinalOtherIndex;
                    }
                }
                else
                {
                    continue;
                }
            }

            if (finalCurrentRoute != null)
            {
                Console.WriteLine("SEGUNDO VIZINHO:");
                entry.routes[finalCurrentIndex] = finalCurrentRoute;
                Console.Write("{");
                foreach (int a in entry.routes[finalCurrentIndex])
                {
                    Console.Write(a + ", ");
                }
                Console.WriteLine();
                entry.routesDistance[finalCurrentIndex] = finalCurrentD;
                entry.routesCapacity[finalCurrentIndex] = finalCurrentC;
                Console.WriteLine("Distancia final da rota " + finalCurrentIndex + ": " + entry.routesDistance[finalCurrentIndex]);

                entry.routes[finalOtherIndex] = finalOtherRoute;
                Console.Write("{");
                foreach (int a in entry.routes[finalOtherIndex])
                {
                    Console.Write(a + ", ");
                }
                Console.WriteLine();
                entry.routesDistance[finalOtherIndex] = finalOtherD;
                entry.routesCapacity[finalOtherIndex] = finalOtherC;
                Console.WriteLine("Distancia final da rota " + finalOtherIndex + ": " + entry.routesDistance[finalOtherIndex]);

                Console.WriteLine("Total: " + (finalOtherD + finalCurrentD));
            }
        }

        static List<int> CloneList(List<int> list)
        {
            int[] a = new int[list.Count];
            list.CopyTo(a);
            return a.OfType<int>().ToList();
        }
    }

    class Instance
    {
        public string nameFile;
        public int dimension;
        public int vehicles;
        public int capacity;
        public int[,] costMatrix;
        public int[] demand;
        public List<int>[] routes;
        public int[] routesDistance;
        public int[] routesCapacity;

        public Instance(string nameFile, int dimension, int vehicles, int capacity)
        {
            this.nameFile = nameFile;
            this.dimension = dimension;
            this.vehicles = vehicles;
            this.capacity = capacity;
            costMatrix = new int[dimension,dimension];
            demand = new int[dimension];
            routes = new List<int>[vehicles];
            routesDistance = new int[vehicles];
            routesCapacity = new int[vehicles];
        }
    }

    public class HeapPair
    {
        public int demand;
        public int cityNum;

        public HeapPair(int cityNum, int demand)
        {
            this.cityNum = cityNum+1;
            this.demand = demand;
        }
    }

    public class MinHeap
    {
        private HeapPair[] _elements;
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

            return _elements[0].demand;
        }

        public HeapPair Pop()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            var result = _elements[0];
            _elements[0] = _elements[_size - 1];
            _elements[_size - 1] = result;
            _size--;

            ReCalculateDown(0);

            return result;
        }

        public void Add(int[] element)
        {
            if (_size == _elements.Length)
                throw new IndexOutOfRangeException();

            for(int i = 0; i < element.Length; i++)
            {
                _elements[_size] = new HeapPair(i, element[i]);
                _size++;
            }

            HeapSort();
        }

        private void ReCalculateDown(int index)
        {
            while (HasLeftChild(index))
            {
                var smallerIndex = GetLeftChildIndex(index);
                if (HasRightChild(index) && GetRightChild(index).demand < GetLeftChild(index).demand)
                {
                    smallerIndex = GetRightChildIndex(index);
                }

                if (_elements[smallerIndex].demand >= _elements[index].demand)
                {
                    break;
                }

                Swap(smallerIndex, index);
                index = smallerIndex;
            }
        }

        private void HeapSort()
        {
            //pegando o piso da metade do tamanho do vetor, eu sei quem é o ultimo pai
            for(int index = (_size / 2) - 1; index >= 0; index--)
            {
                var smallerIndex = GetLeftChildIndex(index);
                
                if (HasRightChild(index) && GetRightChild(index).demand < GetLeftChild(index).demand)
                {
                    smallerIndex = GetRightChildIndex(index);
                }

                if (_elements[smallerIndex].demand < _elements[index].demand)
                {
                    Swap(smallerIndex, index);
                    ReCalculateDown(smallerIndex);
                }
            }
        }

        
    }

    public class MaxHeap
    {
        private HeapPair[] _elements;
        private int _size;

        public MaxHeap(int size)
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

            return _elements[0].demand;
        }

        public HeapPair Pop()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            var result = _elements[0];
            _elements[0] = _elements[_size - 1];
            _elements[_size - 1] = result;
            _size--;

            ReCalculateDown(0);

            return result;
        }

        public void Add(int[] element)
        {
            if (_size == _elements.Length)
                throw new IndexOutOfRangeException();

            for (int i = 0; i < element.Length; i++)
            {
                _elements[_size] = new HeapPair(i, element[i]);
                _size++;
            }

            HeapSort();
        }

        private void ReCalculateDown(int index)
        {
            while (HasLeftChild(index))
            {
                var biggerIndex = GetLeftChildIndex(index);
                if (HasRightChild(index) && GetRightChild(index).demand > GetLeftChild(index).demand)
                {
                    biggerIndex = GetRightChildIndex(index);
                }

                if (_elements[biggerIndex].demand < _elements[index].demand)
                {
                    break;
                }

                Swap(biggerIndex, index);
                index = biggerIndex;
            }
        }

        private void HeapSort()
        {
            //pegando o piso da metade do tamanho do vetor, eu sei quem é o ultimo pai
            for (int index = (_size / 2) - 1; index >= 0; index--)
            {
                var biggerIndex = GetLeftChildIndex(index);

                if (HasRightChild(index) && GetRightChild(index).demand > GetLeftChild(index).demand)
                {
                    biggerIndex = GetRightChildIndex(index);
                }

                if (_elements[biggerIndex].demand > _elements[index].demand)
                {
                    Swap(biggerIndex, index);
                    ReCalculateDown(biggerIndex);
                }
            }
        }
    }
}
