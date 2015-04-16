using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Core.DataStorageModelsWeb.Models
{
    public class Customer : TableEntity
    {
        public Customer() { }

        public Customer(Guid id, string name, int score)
        {
            this.Id = id;
            this.Name = name;
            this.Score = score;

            this.RowKey = id.ToString("n");
            this.PartitionKey = name.Substring(0, 1);
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }
    }
}