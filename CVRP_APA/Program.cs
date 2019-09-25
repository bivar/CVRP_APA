using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CVRP_APA
{
    class Program
    {
        static void Main(string[] args)
        {
            float[] greatDistance = new float[7];
            greatDistance[0] = 212f;
            greatDistance[1] = 216f;
            greatDistance[2] = 529f;
            greatDistance[3] = 510f;
            greatDistance[4] = 696f;
            greatDistance[5] = 741f;
            greatDistance[6] = 568f;

            int[] bestDistanceVNDConstructive = new int[7];
            float[] timeVNDConstructive = new float[7];
            float[] gapVNDConstructive = new float[7];

            int[] bestDistanceVND = new int[7];
            float[] timeVND = new float[7];
            float[] gapVND = new float[7];


            Instance[] vndEntries = new Instance[7];
            vndEntries[0] = CreateEntryFile("P-n19-k2.txt");
            vndEntries[1] = CreateEntryFile("P-n20-k2.txt");
            vndEntries[2] = CreateEntryFile("P-n23-k8.txt");
            vndEntries[3] = CreateEntryFile("P-n45-k5.txt");
            vndEntries[4] = CreateEntryFile("P-n50-k10.txt");
            vndEntries[5] = CreateEntryFile("P-n51-k10.txt");
            vndEntries[6] = CreateEntryFile("P-n55-k7.txt");

            for (int i = 0; i < 7; i++)
            {
                GreedyStep(vndEntries[i], true);
                bestDistanceVNDConstructive[i] = vndEntries[i].totalDistance;
                timeVNDConstructive[i] = vndEntries[i].constructiveTime;
                gapVNDConstructive[i] = gapMeasure(bestDistanceVNDConstructive[i], greatDistance[i]);

                VND(vndEntries[i]);
                bestDistanceVND[i] = vndEntries[i].totalDistance;
                timeVND[i] = vndEntries[i].graspVNDTime;
                gapVND[i] = gapMeasure(bestDistanceVND[i], greatDistance[i]);
            }

            float[] meanDistanceConstructive = new float[7];
            int[] bestDistanceConstructive = new int[7];
            float[] meanTimeConstructive = new float[7];
            float[] gapConstructive = new float[7];

            float[] meanDistanceGrasp = new float[7];
            int[] bestDistanceGrasp = new int[7];
            float[] meanTimeGrasp = new float[7];
            float[] gapGrasp = new float[7];

            //Instancias do Grasp
            List<Instance>[] allGRASPEntries = new List<Instance>[7];
            allGRASPEntries[0] = new List<Instance>();
            allGRASPEntries[1] = new List<Instance>();
            allGRASPEntries[2] = new List<Instance>();
            allGRASPEntries[3] = new List<Instance>();
            allGRASPEntries[4] = new List<Instance>();
            allGRASPEntries[5] = new List<Instance>();
            allGRASPEntries[6] = new List<Instance>();

            //grasp
            for (int i = 0; i < 10; i++)
            {
                Instance[] tmpEntries = new Instance[7];
                tmpEntries[0] = CreateEntryFile("P-n19-k2.txt");
                tmpEntries[1] = CreateEntryFile("P-n20-k2.txt");
                tmpEntries[2] = CreateEntryFile("P-n23-k8.txt");
                tmpEntries[3] = CreateEntryFile("P-n45-k5.txt");
                tmpEntries[4] = CreateEntryFile("P-n50-k10.txt");
                tmpEntries[5] = CreateEntryFile("P-n51-k10.txt");
                tmpEntries[6] = CreateEntryFile("P-n55-k7.txt");

                for (int j = 0; j < 7; j++)
                {
                    if (GRASPConstruiction(tmpEntries[j], true, 4))
                        allGRASPEntries[j].Add(tmpEntries[j]);
                }
            }

            for (int i = 0; i < 7; i++)
            {
                meanDistanceConstructive[i] = meanDistance(allGRASPEntries[i]);

                bestDistanceConstructive[i] = bestEntryInstance(allGRASPEntries[i]);

                meanTimeConstructive[i] = meanTime(allGRASPEntries[i], 3);

                gapConstructive[i] = gapMeasure(bestDistanceConstructive[i], greatDistance[i]);
            }

            //GRASP VIZINHANÇA
            for (int i = 0; i < 7; i++)
            {
                foreach (Instance entry in allGRASPEntries[i])
                {
                    VND(entry);
                }
            }

            for (int i = 0; i < 7; i++)
            {
                meanDistanceGrasp[i] = meanDistance(allGRASPEntries[i]);

                bestDistanceGrasp[i] = bestEntryInstance(allGRASPEntries[i]);

                meanTimeGrasp[i] = meanTime(allGRASPEntries[i], 4);

                gapGrasp[i] = gapMeasure(bestDistanceGrasp[i], greatDistance[i]);
            }

            Console.WriteLine("\n\t\t\t\t Heurística Construtiva VND \t\t\t\t\t\t\t VND");
            Console.WriteLine("\t\t\t----------------------------------------------------------\t-----------------------------------------------------------");
            Console.WriteLine("\t\t Ótimo \t Média Solução  Melhor Solução  Média Tempo \tGAP \t\t Média Solução  Melhor Solução  Média Tempo  GAP");
            for (int i = 0; i < 7; i++)
            {
                Console.WriteLine("Instancia " + i + "\t " + greatDistance[i] + " \t\t " + "---" + " \t\t " + bestDistanceVNDConstructive[i] + " \t     " + timeVNDConstructive[i] + "        "+
                    gapVNDConstructive[i] + "\t\t     " + "---" + " \t " + bestDistanceVND[i]  + " \t         " + timeVND[i] + " \t     " + gapVND[i]);
                //Console.WriteLine("Instancia " + i + "\t 0.0 \t\t 0.0 \t\t 0.0 \t     0.0 \t 0.0\t\t     0.0 \t 0.0 \t         0.0 \t     0.0");

            }

            Console.WriteLine("\n\t\t\t\t Heurística Construtiva GRASP \t\t\t\t\t\t\t GRASP");
            Console.WriteLine("\t\t\t----------------------------------------------------------\t-----------------------------------------------------------");
            Console.WriteLine("\t\t Ótimo \t Média Solução  Melhor Solução  Média Tempo \tGAP \t\t Média Solução  Melhor Solução  Média Tempo  GAP");
            for (int i = 0; i < 7; i++)
            {
                Console.WriteLine("Instancia " + i + "\t " + greatDistance[i] + "\t\t " + meanDistanceConstructive[i].ToString("0.0") + "       " + bestDistanceConstructive[i] + " \t     " + meanTimeConstructive[i].ToString("0.0") + "        " + gapConstructive[i].ToString("0.0") + "\t\t     " + meanDistanceGrasp[i].ToString("0.0") + " \t " + bestDistanceGrasp[i] + " \t         " + meanTimeGrasp[i].ToString("0.0") + " \t     " + gapGrasp[i].ToString("0.0"));
                //Console.WriteLine("Instancia " + i + "\t 0.0 \t\t 0.0 \t\t 0.0 \t     0.0 \t 0.0\t\t     0.0 \t 0.0 \t         0.0 \t     0.0");

            }
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
            int[] routesDistance = new int[entry.vehicles];
            int[] capacitySum = new int[entry.vehicles];
            int[] currentCity = new int[entry.vehicles];

            //We are only allowed to have the number of routes less or equal to the number of vehicles
            List<int>[] routes = new List<int>[entry.vehicles];
            bool[] visited = new bool[entry.dimension];

            MinHeap minheaps = null;
            MaxHeap maxheaps = null;

            Stopwatch greedyTime = new Stopwatch();

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

            if (greedyTime.IsRunning)
            {
                greedyTime.Reset();
            }

            greedyTime.Start();

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
                            if ((capacitySum[i] + minheaps.Peek()) <= entry.capacity)
                            {
                                city = minheaps.Pop();
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

            //PARAR DE CONTABILIZAR O TEMPO DO ALGORITMO
            greedyTime.Stop();

            int totalDistance = 0;

            for (int i = 0; i < routes.Length; i++)
            {
                routes[i].Add(0);
                routesDistance[i] += entry.costMatrix[routes[i][routes[i].Count - 2], 0];
                Console.Write("{");
                foreach (int a in routes[i])
                {
                    Console.Write(a + ", ");

                }
                Console.Write("}");
                Console.WriteLine();
                Console.WriteLine("Distancia dessa rota: " + routesDistance[i]);
                Console.WriteLine("Capacidade dessa rota: " + capacitySum[i]);
                totalDistance += routesDistance[i];
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
                        
            if(allCitiesVisited)
            {
                Console.WriteLine("Distancia total da rota: " + totalDistance);
                entry.routes = routes;
                entry.routesDistance = routesDistance;
                entry.routesCapacity = capacitySum;
                entry.totalDistance = totalDistance;
                entry.constructiveTime = greedyTime.ElapsedMilliseconds;
            }
            else
            {
                if (isMin)
                    GreedyStep(entry, false);
            }
            
        }

        static bool GRASPConstruiction(Instance entry, bool isMin, int listSize)
        {
            bool success = false;
            int[] routesDistance = new int[entry.vehicles];
            int[] capacitySum = new int[entry.vehicles];
            int[] currentCity = new int[entry.vehicles];

            Stopwatch graspConstructionTime = new Stopwatch();

            //Lista para salvar os x menores/naiores valores do passo guloso
            List<HeapPair> graspList = new List<HeapPair>(listSize);

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

            if (graspConstructionTime.IsRunning)
            {
                graspConstructionTime.Reset();
            }

            graspConstructionTime.Start();

            visited[0] = true;
            for (int i = 0; i < routes.Length; i++)
            {
                routes[i] = new List<int>();
                routes[i].Add(0);
            }

            bool allRoutesFull = false;
            //Atribuição dos primeiros valores na lista do grasp
            //E dentro de cada rota escolher um aleatório para ver se encaixa nas rotas
            //LEMBRAR DE AO ADICIONAR EM UM ROTA ATUALIZAR A LISTA
            Random random = new Random();
            for (int i = 0; i < listSize; i++)
            {
                if (isMin)
                {
                    graspList.Add(minheaps.Pop());
                }
                else
                {
                    graspList.Add(maxheaps.Pop());
                }
            }
            
            //EMBARALHAR OS VALORES DA LISTA
            for (int i = 0; i < listSize; i++)
            {
                var shuffleIndex = random.Next(listSize);
                var tmp = graspList[i];
                graspList[i] = graspList[shuffleIndex];
                graspList[shuffleIndex] = tmp;
            }

            HeapPair city = null;
            bool emptyHeap = false;
            int randomIndex = -1;
            while (!allRoutesFull)
            {
                for (int i = 0; i < routes.Length; i++)
                {
                    //CHECA SE A LISTA TA VAZIA
                    if (graspList.Count == 0 && city == null)
                    {
                        allRoutesFull = true;
                        break;
                    }

                    //PEGA UM VALOR ALEATORIO PARA TESTAR COM TODAS AS ROTAS
                    if(city == null)
                    {
                        randomIndex = random.Next(graspList.Count);
                        city = graspList[randomIndex];
                        graspList.RemoveAt(randomIndex);

                        //ENQUANTO O HEAP N ESTIVER VAZIO PEGAR O PROXIMO VALOR DELE
                        if (!emptyHeap)
                        {
                            try
                            {
                                if (isMin)
                                {
                                    Console.WriteLine("********** VALOR DO INDICE: " + randomIndex + " **********");
                                    //ATUALIZAR A LISTA PARA COLOCAR OUTRO VALOR NELA 
                                    graspList.Insert(randomIndex, minheaps.Pop());
                                }
                                else
                                {
                                    //ATUALIZAR A LISTA PARA COLOCAR OUTRO VALOR NELA 
                                    graspList.Insert(randomIndex, maxheaps.Pop());
                                }
                            }

                            catch (IndexOutOfRangeException)
                            {
                                Console.WriteLine("########## Não tem mais cidade no heap. ##########");
                                emptyHeap = true;
                            }
                        }
                    }

                    //CHECAR SE A DEMANDA DA CIDADE CABE NA ROTA ATUAL
                    if ((capacitySum[i] + city.demand) > entry.capacity)
                    {
                        //Console.WriteLine("########## NÃO CABE NA ROTA ########");
                        //Console.WriteLine("Cidade: " + city.cityNum);
                        bool fit = false;
                        for (int j = 0; j < routes.Length; j++)
                        {
                            //ALTERAR PARA PEGAR SÓ VALORES DA LISTA
                            if ((capacitySum[j] + city.demand) <= entry.capacity)
                            {
                                fit = true;
                                break;
                            }
                        }
                        if (!fit)
                        {
                            allRoutesFull = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //SE COUBER SÓ COLOCAR
                    Console.WriteLine("Cidade: " + city.cityNum);
                    Console.WriteLine("Demanda: " + city.demand);
                    routes[i].Add(city.cityNum);
                    capacitySum[i] += city.demand;
                    visited[city.cityNum] = true;
                    routesDistance[i] += entry.costMatrix[currentCity[i], city.cityNum];
                    currentCity[i] = city.cityNum;
                    city = null;
                    randomIndex = -1;
                    break;
                }
            }

            //PEGAR O TEMPO TOTAL DO ALGORITMO
            graspConstructionTime.Stop();
            int totalDistance = 0;

            for (int i = 0; i < routes.Length; i++)
            {
                routes[i].Add(0);
                routesDistance[i] += entry.costMatrix[routes[i][routes[i].Count - 2], 0];
                Console.Write("{");
                foreach (int a in routes[i])
                {
                    Console.Write(a + ", ");

                }
                Console.Write("}");
                Console.WriteLine();
                Console.WriteLine("Distancia dessa rota: " + routesDistance[i]);
                Console.WriteLine("Capacidade dessa rota: " + capacitySum[i]);
                totalDistance += routesDistance[i];
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

            if (allCitiesVisited)
            {
                Console.WriteLine("Distancia total da rota: " + totalDistance);
                entry.routes = routes;
                entry.routesDistance = routesDistance;
                entry.routesCapacity = capacitySum;
                success = true;
                //Salva a distancia total percorrida 
                entry.totalDistance = totalDistance;
                entry.graspTime = graspConstructionTime.ElapsedMilliseconds;
            }
            else
            {
                if (isMin)
                {
                    if(GRASPConstruiction(entry, false, listSize))
                    {
                        success = true;
                    }
                }
                    
            }

            return success;
        }
     
        static Instance FirstNeighbour(Instance entry)
        {
            int total = 0;
            bool improvement = false;

            for(int i = 0; i < entry.routes.Length; i++)
            {
                //pegando as rotas e as distancias iniciais de cada uma
                List<int> initialRoute = entry.routes[i];
                int initialD = entry.routesDistance[i];

                List<int> finalRoute = null;
                int finalD = int.MaxValue;
                //loop para todas as cidades dentro de cada rota, excluindo a primeira e a ultima
                for (int j = 1; j < (initialRoute.Count - 1); j++)
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
                    improvement = true;
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
            
            if (improvement)
            {
                Console.WriteLine("Distancia total :" + total);
                return entry;
            }
            else
            {
                return null;
            }
        }

        static Instance SecondNeighbour(Instance entry)
        {
            List<int> finalCurrentRoute = null;
            int finalCurrentD = int.MaxValue;
            int finalCurrentC = 0;
            int finalCurrentIndex = 0;

            List<int> finalOtherRoute = null;
            int finalOtherD = int.MaxValue;
            int finalOtherC = 0;
            int finalOtherIndex = 0;

            int finalCredit = int.MinValue;
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

                int semiFinalCredit = int.MinValue;
                int currentIndexPosition = -1;
                //loop para todas as cidades dentro de cada rota, excluindo a primeira e a ultima
                for (int j = 1; j < (auxRoute.Count - 1); j++)
                {
                    int cityR = auxRoute[j];

                    //Excluir a distancia da cidade escolhida com a sua antecessora e sucessora, além disso somar a distancia entre a antecessora e a sucessora para pegar seu credito
                    int auxD = initialD - entry.costMatrix[auxRoute[j - 1], cityR] - entry.costMatrix[cityR, auxRoute[j + 1]] + entry.costMatrix[auxRoute[j - 1], auxRoute[j + 1]];
                    int currentCredit = initialD - auxD;

                    //ISSO AQUI SALVA DENTRE TODAS AS OUTRAS ROTAS POSSIVEIS
                    List<int> quarterFinalOtherRoute = null;
                    int quarterFinalOtherD = int.MaxValue;
                    int quarterFinalOtherC = 0;
                    int quarterFinalOtherIndex = 0;

                    int quarterFinalCredit = int.MinValue;
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
                                int octavesFinalCredit = int.MinValue;

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
                                    if ((totalCredit > 0) && (totalCredit > octavesFinalCredit))
                                    {
                                        otherIndexPosition = l;
                                        octavesFinalOtherD = testD;
                                        octavesFinalCredit = totalCredit;
                                    }

                                }

                                if (otherIndexPosition != -1)
                                {
                                    if (octavesFinalCredit > quarterFinalCredit)
                                    {
                                        otherRoute.Insert(otherIndexPosition + 1, cityR);
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

                    if(quarterFinalCredit > semiFinalCredit && quarterFinalOtherRoute != null)
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
                    if(semiFinalCredit > finalCredit)
                    {
                        auxRoute.RemoveAt(currentIndexPosition);
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
                int initialSumDistance = 0;
                initialSumDistance = entry.routesDistance[finalCurrentIndex] + entry.routesDistance[finalOtherIndex];

                if((finalCurrentD + finalOtherD) < initialSumDistance)
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

                    int total = 0;
                    foreach (int distance in entry.routesDistance)
                    {
                        total += distance;
                    }

                    Console.WriteLine("Total: " + total);
                    return entry;
                }
            }

            return null;
        }

        static Instance ThirdNeighbour(Instance entry)
        {
            //List<int> finalCurrentRoute = null;
            int finalCurrentD = -1;
            int finalCurrentC = -1;
            int finalCurrentRouteIndex = -1;
            int finalCurrentCityIndex = -1;

            //List<int> finalOtherRoute = null;
            int finalOtherD = -1;
            int finalOtherC = -1;
            int finalOtherRouteIndex = -1;
            int finalOtherCityIndex = -1;

            int finalCredit = int.MinValue;
            for (int i = 0; i < entry.routes.Length; i++)
            {
                //pegando as rotas e as distancias iniciais de cada uma
                int initialD = entry.routesDistance[i];
                List<int> currentRoute = CloneList(entry.routes[i]);

                //tenho que salvar a rota e distancia tanto da rota que eu tirei a cidade, quanto da rota que eu coloquei a cidade retirada
                int semiFinalCurrentD = -1;
                int semiFinalCurrentC = -1;
                int semiFinalCurrentCityIndex = -1;

                //List<int> semiFinalOtherRoute = null;
                int semiFinalOtherD = -1;
                int semiFinalOtherC = -1;
                int semiFinalOtherRouteIndex = -1;
                int semiFinalOtherCityIndex = -1;

                int semiFinalCredit = int.MinValue;
                //loop para todas as cidades dentro de cada rota, excluindo a primeira e a ultima
                for (int j = 1; j < (currentRoute.Count - 1); j++)
                {
                    int cityR = currentRoute[j];
                    int currentRouteC = entry.routesCapacity[i] - entry.demand[cityR];
                    int quarterFinalCurrentD = -1;
                    int quarterFinalCurrentC = -1;

                    //ISSO AQUI SALVA DENTRE TODAS AS OUTRAS ROTAS POSSIVEIS
                    //List<int> quarterFinalOtherRoute = null;
                    int quarterFinalOtherD = -1;
                    int quarterFinalOtherC = -1;
                    int quarterFinalOtherRouteIndex = -1;
                    int quarterFinalOtherCityIndex = -1;

                    int quarterFinalCredit = int.MinValue;
                    //Loop para todas as outras rotas diferentes da que foi escolhida
                    for (int k = 0; k < entry.routes.Length; k++)
                    {
                        if (k != i)
                        {
                            //pegando as cidades outras rotas e sua distancia total
                            List<int> otherRoute = CloneList(entry.routes[k]);
                            int otherD = entry.routesDistance[k];

                            int octavesFinalCurrentD = -1;
                            int octavesFinalCurrentC = -1;
                            
                            //Variavel para salvar a melhor distancia depois colocar a nova cidade
                            int octavesFinalOtherD = -1;
                            int octavesFinalOtherC = -1;
                            int octavesFinalOtherCityIndex = -1;

                            int octavesFinalCredit = int.MinValue;
                            //LOOP ENTRE TODAS AS CIDADES, EXCLUINDO A PRIMEIRA E A ÚLTIMA
                            for (int l = 1; l < (otherRoute.Count - 1); l++)
                            {
                                int otherCity = otherRoute[l];
                                int otherRouteC = entry.routesCapacity[k] - entry.demand[otherCity];

                                if ((currentRouteC + entry.demand[otherCity] <= entry.capacity) && (otherRouteC + entry.demand[cityR] <= entry.capacity))
                                {
                                    //Excluir a distancia da cidade escolhida com a sua antecessora e sucessora, além disso somar a distancia entre a antecessora e a sucessora para pegar seu credito
                                    int currentD = initialD - entry.costMatrix[currentRoute[j - 1], cityR] - entry.costMatrix[cityR, currentRoute[j + 1]] + 
                                        entry.costMatrix[currentRoute[j - 1], otherCity] + entry.costMatrix[otherCity, currentRoute[j + 1]];
                                    int currentCredit = initialD - currentD;

                                    //Excluir a distancia entre as duas cidades da rota escolhida e adicionar a distancia das cidades da rota escolhida para a cidade a entrar na rota 
                                    int testD = otherD - entry.costMatrix[otherRoute[l - 1], otherCity] - entry.costMatrix[otherCity, otherRoute[l + 1]] + 
                                        entry.costMatrix[otherRoute[l - 1], cityR] + entry.costMatrix[cityR, otherRoute[l + 1]];
                                    int otherCredit = otherD - testD;
                                    int totalCredit = otherCredit + currentCredit;

                                    //comparação de credito pra saber se vale a pena trocar, depois checa se a nova distancia seria melhor do que a salva anteriormente
                                    if ((totalCredit > 0) && (totalCredit > octavesFinalCredit))
                                    {
                                        octavesFinalCurrentC = currentRouteC + entry.demand[otherCity];
                                        octavesFinalCurrentD = currentD;

                                        octavesFinalOtherC = otherRouteC + entry.demand[cityR];
                                        octavesFinalOtherD = testD;
                                        octavesFinalOtherCityIndex = l;

                                        octavesFinalCredit = totalCredit;
                                    }
                                }
                            }

                            if ((octavesFinalOtherCityIndex != -1) && (octavesFinalCredit > quarterFinalCredit))
                            {
                                quarterFinalCurrentD = octavesFinalCurrentD;
                                quarterFinalCurrentC = octavesFinalCurrentC;

                                quarterFinalOtherD = octavesFinalOtherD;
                                quarterFinalOtherC = octavesFinalOtherC;
                                quarterFinalCredit = octavesFinalCredit;
                                quarterFinalOtherCityIndex = octavesFinalOtherCityIndex;
                                quarterFinalOtherRouteIndex = k;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    if ((quarterFinalCredit > semiFinalCredit) && (quarterFinalOtherRouteIndex != -1))
                    {
                        semiFinalCredit = quarterFinalCredit;

                        semiFinalCurrentD = quarterFinalCurrentD;
                        semiFinalCurrentC = quarterFinalCurrentC;
                        semiFinalCurrentCityIndex = j;

                        semiFinalOtherD = quarterFinalOtherD;
                        semiFinalOtherC = quarterFinalOtherC;
                        semiFinalOtherRouteIndex = quarterFinalOtherRouteIndex;
                        semiFinalOtherCityIndex = quarterFinalOtherCityIndex;
                        
                    }
                }

                if ((semiFinalCurrentCityIndex != -1) && (semiFinalCredit > finalCredit))
                {
                    finalCredit = semiFinalCredit;

                    finalCurrentD = semiFinalCurrentD;
                    finalCurrentC = semiFinalCurrentC;
                    finalCurrentRouteIndex = i;
                    finalCurrentCityIndex = semiFinalCurrentCityIndex;

                    finalOtherD = semiFinalOtherD;
                    finalOtherC = semiFinalOtherC;
                    finalOtherRouteIndex = semiFinalOtherRouteIndex;
                    finalOtherCityIndex = semiFinalOtherCityIndex;
                }
                else
                {
                    continue;
                }
            }

            if (finalCurrentRouteIndex != -1)
            {
                int initialSumDistance = 0;
                Console.WriteLine("Indice da primeira rota: " + finalCurrentRouteIndex);
                Console.WriteLine("Indice da segunda rota: " + finalOtherRouteIndex);
                initialSumDistance = entry.routesDistance[finalCurrentRouteIndex] + entry.routesDistance[finalOtherRouteIndex];

                if ((finalCurrentD + finalOtherD) < initialSumDistance)
                {
                    Console.WriteLine("\nTERCEIRO VIZINHO:");

                    int currentCity = entry.routes[finalCurrentRouteIndex][finalCurrentCityIndex];
                    int otherCity = entry.routes[finalOtherRouteIndex][finalOtherCityIndex];
                    entry.routes[finalCurrentRouteIndex].RemoveAt(finalCurrentCityIndex);
                    entry.routes[finalCurrentRouteIndex].Insert(finalCurrentCityIndex, otherCity);
                    entry.routes[finalOtherRouteIndex].RemoveAt(finalOtherCityIndex);
                    entry.routes[finalOtherRouteIndex].Insert(finalOtherCityIndex, currentCity);

                    Console.Write("{");
                    foreach (int a in entry.routes[finalCurrentRouteIndex])
                    {
                        Console.Write(a + ", ");
                    }
                    Console.WriteLine();
                    entry.routesDistance[finalCurrentRouteIndex] = finalCurrentD;
                    entry.routesCapacity[finalCurrentRouteIndex] = finalCurrentC;
                    Console.WriteLine("Distancia final da rota " + finalCurrentRouteIndex + ": " + entry.routesDistance[finalCurrentRouteIndex]);

                    Console.Write("{");
                    foreach (int a in entry.routes[finalOtherRouteIndex])
                    {
                        Console.Write(a + ", ");
                    }
                    Console.WriteLine();
                    entry.routesDistance[finalOtherRouteIndex] = finalOtherD;
                    entry.routesCapacity[finalOtherRouteIndex] = finalOtherC;
                    Console.WriteLine("Distancia final da rota " + finalOtherRouteIndex + ": " + entry.routesDistance[finalOtherRouteIndex]);

                    int total = 0;
                    foreach (int distance in entry.routesDistance)
                    {
                        total += distance;
                    }

                    Console.WriteLine("Total: " + total);
                    return entry;
                }
            }

            return null;
        }

        static void VND(Instance entry)
        {
            Instance resultEntry = entry;
            bool improvement = true;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (improvement)
            {
                int k = 1;
                improvement = false;
                while(k <= 3)
                {
                    Instance aux = null;
                    if(k == 1)
                    {
                        aux = FirstNeighbour(resultEntry);
                    }
                    else if(k == 2)
                    {
                        aux = SecondNeighbour(resultEntry);
                    }
                    else if(k == 3)
                    {
                        aux = ThirdNeighbour(resultEntry);
                    }

                    if(aux != null)
                    {
                        resultEntry = aux;
                        improvement = true;
                    }
                    else
                    {
                        k++;
                    }
                }
            }

            int totalDistance = 0;

            for (int i = 0; i < entry.routes.Length; i++)
            {
                totalDistance += entry.routesDistance[i];
            }

            entry.totalDistance = totalDistance; 

            timer.Stop();

            entry.graspVNDTime = timer.ElapsedMilliseconds;
        }

        static List<int> CloneList(List<int> list)
        {
            int[] a = new int[list.Count];
            list.CopyTo(a);
            return a.OfType<int>().ToList();
        }

        static int bestEntryInstance(List<Instance> instancesEntries)
        {
            int bestResult = int.MaxValue;

            foreach (Instance entry in instancesEntries)
            {
                if (entry.totalDistance < bestResult)
                {
                    bestResult = entry.totalDistance;
                }
            }

            return bestResult;
        }

        static float meanDistance(List<Instance> instancesEntries)
        {
            float meanDistance = 0.0f;

            foreach (Instance entry in instancesEntries)
            {
                meanDistance += entry.totalDistance;
            }

            meanDistance = meanDistance / instancesEntries.Count;

            return meanDistance;
        }

        static float meanTime(List<Instance> instancesEntries, int type)
        {
            float meanTime = 0.0f;

            foreach (Instance entry in instancesEntries)
            {
                switch (type)
                {
                    case 1:
                        meanTime += entry.constructiveTime;
                        break;
                    case 3:
                        meanTime += entry.graspTime;
                        break;
                    case 4:
                        meanTime += entry.graspVNDTime;
                        break;
                }

            }

            meanTime = meanTime / instancesEntries.Count;

            return meanTime;
        }

        static float gapMeasure(int bestInstance, float greatDistance)
        {
            float gap = (((bestInstance - greatDistance) / greatDistance) * 100f);

            return gap;
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
        public int totalDistance = 0;
        public float constructiveTime = 0;
        public float graspTime = 0;
        public float graspVNDTime = 0;

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
