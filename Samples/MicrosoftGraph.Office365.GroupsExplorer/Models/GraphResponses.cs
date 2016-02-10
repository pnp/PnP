using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{
	public class GetGroupsResponse
	{
		public Group[] value { get; set; }
	}

	public class GetConversationsResponse
	{
		public Conversation[] value { get; set; }
	}

	public class GetThreadsResponse
	{
		public ConversationThread[] value { get; set; }
	}

	public class GetPostsResponse
	{
		public Post[] value { get; set; }
	}

	public class GetEventsResponse
	{
		public Event[] value { get; set; }
	}

	public class GetFilesResponse
	{
		public DriveItem[] value { get; set; }
	}
}