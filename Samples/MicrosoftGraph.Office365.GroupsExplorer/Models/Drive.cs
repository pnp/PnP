using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{
	public class Drive
	{
		public string driveType { get; set; }  // OneDrive personal drives will return personal. OneDrive for Business will return business.
		public string id { get; set; }
		public IdentitySet owner { get; set; }
		public Quota quota { get; set; }
	}

	public class DriveItem
	{
		public Audio audio { get; set; }
		public string cTag { get; set; }
		public string content { get; set; }  //Stream 
		public IdentitySet createdBy { get; set; }
		public string createdDateTime { get; set; } //DateTimeOffset 
		public Deleted deleted { get; set; }
		public string eTag { get; set; }
		public File file { get; set; }
		public FileSystemInfo fileSystemInfo { get; set; }
		public FolderMetadata folder { get; set; }
		public string id { get; set; }
		public ImageMetadata image { get; set; }
		public IdentitySet lastModifiedBy { get; set; }
		public string lastModifiedDateTime { get; set; } // DateTimeOffset 
		public LocationCoordinates location { get; set; }
		public string name { get; set; }
		public ItemReference parentReference { get; set; }
		public Photo photo { get; set; }
		public SearchResult searchResult { get; set; }
		public Int64 size { get; set; }
		public SpecialFolder specialFolder { get; set; }
		public Video video { get; set; }
		public string webUrl { get; set; }

	}
}