using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class DriveItem : BaseModel
    {
        public System.IO.Stream Content;
        public IdentitySet CreatedBy;
        public Nullable<DateTimeOffset> CreatedDateTime;
        public String CTag;
        public String Description;
        public String ETag;
        public IdentitySet LastModifiedBy;
        public Nullable<DateTimeOffset> LastModifiedDateTime;
        public String Name;
        public ItemReference ParentReference;
        public Int64 Size;
        public String WebDavUrl;
        public String WebUrl;
        public Audio audio;
        public Deleted deleted;
        public File file;
        public FileSystemInfo fileSystemInfo;
        public Folder folder;
        public Image image;
        public GeoCoordinates location;
        public Photo photo;
        public SearchResult searchResult;
        public Shared shared;
        public SpecialFolder specialFolder;
        public Video video;
    }
}