using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.DataStorageModelsWeb.Services
{
    public class SurveyRatingsService
    {
        private CloudTableClient tableClient;

        private CloudTable surveyRatingsTable;
        public SurveyRatingsService(string storageConnectionStringConfigName = "StorageConnectionString")
        {
            var connectionString = Util.GetConfigSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            this.tableClient = storageAccount.CreateCloudTableClient();
            this.surveyRatingsTable = this.tableClient.GetTableReference("SurveyRatings");
            this.surveyRatingsTable.CreateIfNotExists();
        }

        public float GetUserScore(string userName)
        {
            var query = new TableQuery<Models.Customer>()
                .Select(new List<string> { "Score" })
                .Where(TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, userName));

            var items = surveyRatingsTable
                .ExecuteQuery(query)
                .ToArray();

            if (items.Length == 0)           
                return AddSurveyRatings(userName);

            return (float)items.Average(c => c.Score);
        }

        private float AddSurveyRatings(string userName)
        {
            float sum = 0;
            int count = 4;
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                var score = random.Next(80, 100);
                var customer = new Models.Customer(Guid.NewGuid(), userName, score);

                var insertOperation = TableOperation.Insert(customer);
                surveyRatingsTable.Execute(insertOperation);

                sum += score;
            }
            return sum / count;
        }
    }
}