# Trabalho Prático 1: Algoritmo do Banqueiro

### Desenvolvedores: 
* Pedro Martins Assunção de Oliveira;
* Ana Luiza Damasceno Miranda;

## Descrição do Projeto:
Este projeto consiste na implementação do Algoritmo do Banqueiro, conforme descrito na seção do livro apresentada. O software funciona 
como um simulador de gerenciamento de recursos, onde um "Banqueiro" analisa as solicitações de  diversos clientes para garantir que o 
sistema nunca entre em estado de deadlock.  

## Conceitos Aplicados
* Multithreading: Implementação de n threads de clientes que operam de forma assíncrona.  
* Sincronização: Uso de travas exclusivas (lock) para evitar condições de corrida ao acessar dados compartilhados.  
* Prevenção de Deadlocks: Execução do algoritmo de segurança para validar se uma concessão de recursos mantém o sistema em um estado seguro.  

## Estruturas de Dados Utilizadas
A implementação utiliza matrizes e vetores para o controle rigoroso dos recursos:  
* available: Quantidade de instâncias disponíveis de cada recurso.  
* maximum: Demanda máxima de cada cliente por tipo de recurso.  
* allocation: Quantidade de recursos atualmente alocados para cada cliente.  
* need: Necessidade remanescente de cada cliente para concluir sua tarefa.  

## Instruções de Uso
* Pré-requisitos: NET SDK instalado na máquina.
* Execução: O programa deve ser iniciado via linha de comando, informando o número de instâncias iniciais para cada tipo de recurso. 
* Exemplo de comando para 3 tipos de recursos: *dotnet run 10 5 7*