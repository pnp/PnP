using Core.DataStorageModelsWeb.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Core.DataStorageModelsWeb.Services
{
    public class CallQueueService
    {
        private CloudQueueClient queueClient;

        private CloudQueue queue;

        public CallQueueService(string storageConnectionStringConfigName = "StorageConnectionString")
        {
            var connectionString = CloudConfigurationManager.GetSetting(storageConnectionStringConfigName);
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            this.queueClient = storageAccount.CreateCloudQueueClient();
            this.queue = queueClient.GetQueueReference("calls");
            this.queue.CreateIfNotExists();
        }

        public int? GetCallCount()
        {
            queue.FetchAttributes();
            return queue.ApproximateMessageCount;
        }

        public IEnumerable<Call> PeekCalls(UInt16 count)
        {
            var messages = queue.PeekMessages(count);

            var serializer = new JavaScriptSerializer();
            foreach (var message in messages)
            {
                Call call = null;
                try
                {
                    call = serializer.Deserialize<Call>(message.AsString);
                }
                catch { }
                if (call != null) yield return call;
            }
        }

        public void AddCall(Call call)
        {
            var serializer = new JavaScriptSerializer();
            var content = serializer.Serialize(call);
            var message = new CloudQueueMessage(content);
            queue.AddMessage(message);
        }

        public void DequeueCall()
        {
            var message = queue.GetMessage();
            queue.DeleteMessage(message);
        }

        public int SimulateCalls()
        {
            Random random = new Random();
            int count = random.Next(1, 6);
            for (int i = 0; i < count; i++)
            {
                int phoneNumber = random.Next();
                var call = new Call
                {
                    ReceivedDate = DateTime.Now,
                    PhoneNumber = phoneNumber.ToString("+1-000-000-0000")
                };
                AddCall(call);
            }
            return count;
        }
    }
}