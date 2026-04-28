using System;
using System.Threading;

class Banker {
    // Definições conforme o enunciado
    public const int NUMBER_OF_CUSTOMERS = 5;
    public const int NUMBER_OF_RESOURCES = 3;

    public int[] available = new int[NUMBER_OF_RESOURCES];
    public int[,] maximum = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    public int[,] allocation = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    public int[,] need = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];

    private readonly object padlock = new object(); // Mutex

    public Banker(int[] initialResources) {
        available = (int[])initialResources.Clone();
        Random rand = new Random();
        
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++) {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                maximum[i, j] = rand.Next(1, available[j] + 1); // Inicialização aleatória
                need[i, j] = maximum[i, j];
                allocation[i, j] = 0;
            }
        }
    }

    // Algoritmo de Segurança
    private bool IsSafeState() {
        int[] work = (int[])available.Clone();
        bool[] finish = new bool[NUMBER_OF_CUSTOMERS];

        for (int k = 0; k < NUMBER_OF_CUSTOMERS; k++) {
            for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++) {
                if (!finish[i]) {
                    bool canAllocate = true;

                    for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                        if (need[i, j] > work[j]) {
                            canAllocate = false;
                            break;
                        }
                    }

                    if (canAllocate) {
                        for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                            work[j] += allocation[i, j];
                        }
                        finish[i] = true;
                    }
                }
            }
        }

        foreach (bool f in finish) {
            if (!f) return false;
        }
        return true;
    }

    public int RequestResources(int customerNum, int[] request) {
        lock (padlock) { // Proteção contra acesso concorrente
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                if (request[j] > need[customerNum, j] || request[j] > available[j])
                    return -1;
            }

            // Simula alocação
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                available[j] -= request[j];
                allocation[customerNum, j] += request[j];
                need[customerNum, j] -= request[j];
            }

            if (IsSafeState())
                return 0;

            // Rollback
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                available[j] += request[j];
                allocation[customerNum, j] -= request[j];
                need[customerNum, j] += request[j];
            }

            return -1;
        }
    }

    public int ReleaseResources(int customerNum, int[] release) {
        lock (padlock) {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                available[j] += release[j];
                allocation[customerNum, j] -= release[j];
                need[customerNum, j] += release[j];
            }
            return 0;
        }
    }

    public void CustomerThread(object id) {
        int customerNum = (int)id;
        Random rand = new Random();

        for (int ciclo = 0; ciclo < 10; ciclo++) {
            int[] request = new int[NUMBER_OF_RESOURCES];

            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                request[j] = rand.Next(0, need[customerNum, j] + 1);
            }
            Console.WriteLine($"Cliente {customerNum} solicitou: [{string.Join(",", request)}]");

            if (RequestResources(customerNum, request) == 0) {
                Console.WriteLine($"[OK] Cliente {customerNum} alocou: [{string.Join(",", request)}]");
                Thread.Sleep(rand.Next(1000, 3000));

                int[] release = new int[NUMBER_OF_RESOURCES];

                for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                    release[j] = rand.Next(0, allocation[customerNum, j] + 1);
                }

                ReleaseResources(customerNum, release);
                Console.WriteLine($"[LIBEROU] Cliente {customerNum}: [{string.Join(",", release)}]");
            }

            Thread.Sleep(rand.Next(500, 2000));
        }
    }

    static void Main(string[] args) {
        if (args.Length < NUMBER_OF_RESOURCES) {
            Console.WriteLine("Erro: Passe os recursos iniciais via linha de comando.");
            return;
        }

        int[] initialResources = Array.ConvertAll(args, int.Parse);
        Banker banker = new Banker(initialResources);

        Thread[] threads = new Thread[NUMBER_OF_CUSTOMERS];

        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++) {
            threads[i] = new Thread(banker.CustomerThread);
            threads[i].Start(i);
        }
    }
}