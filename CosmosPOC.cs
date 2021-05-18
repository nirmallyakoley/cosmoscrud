using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace DemoFunctionApp
{
   public class CosmosPOC
    {
        //string EndpointUri = "https://poc-cosmos-crud.documents.azure.com:443/";
        //string PrimaryKey = "5WPxvzuQKVELbyTxMKb5dk9k9A8TWoNuy4g6AXythG3z1c7tYwyQ7mV7jgXtzgkzcR4RSfyuQvz6GendHNlRtA==";
        string EndpointUri = Environment.GetEnvironmentVariable("EndpointUri");
        string PrimaryKey = Environment.GetEnvironmentVariable("CosmosKey");
        // The database we will create
        Database database;
        // The name of the database and container we will create
        string databaseId = Environment.GetEnvironmentVariable("CosmosDBName");
        string containerId = Environment.GetEnvironmentVariable("ContainerName");
        CosmosClient cosmosClient;
        // The container we will create.
        Container container;
        public void Initialize()
        {
            // Create a new instance of the Cosmos Client in Gateway mode
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Gateway
            });


        }
        public async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
           
        }
        public async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/id");
           
        }
        public async Task AddItemsToContainerAsync(Customer objCustomer)
        {
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Customer> responseCustomer = await this.container.ReadItemAsync<Customer>(objCustomer.id.ToString(), new PartitionKey(objCustomer.id));
                
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                try
                {
                    // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                    ItemResponse<Customer> responseCustomer = await this.container.CreateItemAsync<Customer>(objCustomer, new PartitionKey(objCustomer.id));
                }
                catch(CosmosException ex1) {
                    throw ex1;
                }
              
            }
           
            
            
        }

        public async Task DeleteItemAsync(string id)
        {    // Delete an item. Note we must provide the partition key value and id of the item to delete
            try
            {
                ItemResponse<Customer> wakefieldFamilyResponse = await this.container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
            }
            catch {
                throw;
            }

        }

        public async Task<List<Customer>> QueryItemsAsync(string id)
        {
            try
            {
                var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{id}'";

                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<Customer> queryResultSetIterator = this.container.GetItemQueryIterator<Customer>(queryDefinition);

                List<Customer> customers = new List<Customer>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Customer> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Customer customer in currentResultSet)
                    {
                        customers.Add(customer);
                    }
                }

                return customers;
            }
            catch
            {
                throw;
            }
        }

        public async Task ReplaceItemAsync(Customer customer)
        {
            try
            {
                ItemResponse<Customer> customerResponse = await this.container.ReadItemAsync<Customer>(customer.id, new PartitionKey(customer.id));
                Customer itemBody = customerResponse.Resource;

                itemBody.ContactName = customer.ContactName;
                // update grade of child

                // replace the item with the updated content
                customerResponse = await this.container.ReplaceItemAsync<Customer>(itemBody, itemBody.id, new PartitionKey(itemBody.id));
            }
            catch
            {
                throw;
            }
            
        }


    }

}

