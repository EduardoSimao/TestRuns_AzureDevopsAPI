# TestRuns_AzureDevopsAPI
USANDO AZURE DEVOPS REST API PARA ADICIONAR EVIDENCIA DO TESTE NO TEST RUNS COM C#.


* Execute os comandos abaixo no Prompt de comando do Visual Studio.
    * Para executar entre na pasta do projeto e depois na pasta ROBOTESTE_ILibrarySystem:
    ```bash
     * Rodar os teste e Gerar arquivo testresult.trx
    ```
    ```bash
    vstest.console bin\Debug\ROBOTESTE_ILibrarySystem.exe /logger:trx;LogFileName=testresult.trx
    ```

    * Para sincronizar o resultado no Azure Devops
    ```bash
     * Lembre-se de configurar o arquivo do SpecSync com a URL do projeto criado no Azure Devops
    ```
    ```bash
    specsync4azuredevops.cmd publish-test-results
    ```
    
    * Subir cenarios de teste pro TestPlans
    ```bash
      * Da um push dos cenarios criados com o SpecFlow pro Azure Devops
    ```
    ```bash
    specsync4azuredevops.cmd push
    ```
