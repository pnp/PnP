using NETCore.RemoteEventReceiver.Models;

namespace NETCore.RemoteEventReceiver.Services
{
    public class DemoEventReceiver : IRemoteEventService
    {
        /// <summary>
        /// Handles events that occur before an action occurs, such as when a user adds or deletes a list item.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        /// <returns>Holds information returned from the remote event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();
            return result;
        }

        /// <summary>
        /// Handles events that occur after an action occurs, such as after a user adds an item to a list or deletes an item from a list.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            switch (properties.EventType)
            {
                case SPRemoteEventType.ItemAdded:
                case SPRemoteEventType.ItemUpdated:
                    HandleItemAddedOrUpdated(properties);
                    break;
            }
        }

        private void HandleItemAddedOrUpdated(SPRemoteEventProperties properties)
        {
            string webUrl = properties.ItemEventProperties.WebUrl;
            string fileUrl = properties.ItemEventProperties.AfterUrl;
            string listName = properties.ItemEventProperties.ListTitle;
            int itemId = properties.ItemEventProperties.ListItemId;

            System.Console.WriteLine($"{webUrl} {fileUrl} {listName} {itemId}");
        }
    }
}
