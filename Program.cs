using System;
using System.Threading;

class Banker {
    [cite_start]// Definições conforme o enunciado [cite: 11, 12]
    public const int NUMBER_OF_CUSTOMERS = 5;
    public const int NUMBER_OF_RESOURCES = 3;

    [cite_start]public int[] available = new int[NUMBER_OF_RESOURCES]; [cite: 15]
    [cite_start]public int[,] maximum = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES]; [cite: 17]
    [cite_start]public int[,] allocation = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES]; [cite: 19]
    [cite_start]public int[,] need = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES]; [cite: 21]

    private readonly object padlock = new object(); // Mutex para evitar condições de corrida [cite: 35]

    public Banker(int[] initialResources) {
        [cite_start]available = (int[])initialResources.Clone(); [cite: 41]
        Random rand = new Random();
        
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++) {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                maximum[i, j] = rand.Next(1, available[j] + 1); // Inicialização aleatória [cite: 41]
                need[i, j] = maximum[i, j];
                allocation[i, j] = 0;
            }
        }
    }

    [cite_start]// Algoritmo de Segurança (Seção 7.5.3.1) [cite: 26]
    private bool IsSafeState() {
        int[] work = (int[])available.Clone();
        bool[] finish = new bool[NUMBER_OF_CUSTOMERS];

        for (int k = 0; k < NUMBER_OF_CUSTOMERS; k++) {
            for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++) {
                if (!finish[i]) {
                    bool canAllocate = true;
                    for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                        if (need[i, j] > work[j]) canAllocate = false;
                    }

                    if (canAllocate) {
                        for (int j = 0; j < NUMBER_OF_RESOURCES; j++) work[j] += allocation[i, j];
                        finish[i] = true;
                    }
                }
            }
        }

        foreach (bool f in finish) if (!f) return false;
        return true;
    }

    public int RequestResources(int customerNum, int[] request) {
        [cite_start]lock (padlock) { // Proteção contra acesso concorrente [cite: 34]
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                if (request[j] > need[customerNum, j] || request[j] > available[j]) return -1;
            }

            // Simula a alocação
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++) {
                available[j] -= request[j];
                allocation[customerNum, j] += request[j];
                need[customerNum, j] -= request[j];
            }

            [cite_start]if (IsSafeState()) return 0; [cite: 33]

            [cite_start]// Rollback se for inseguro [cite: 27]
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
            [cite_start]return 0; [cite: 33]
        }
    }
}