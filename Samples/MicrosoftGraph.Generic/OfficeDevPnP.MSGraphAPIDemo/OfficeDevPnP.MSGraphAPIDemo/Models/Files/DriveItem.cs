using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines any generic item of a drive
    /// </summary>
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
        public Nullable<Int64> Size;
        public String WebDavUrl;
        public String WebUrl;
        public Audio Audio;
        public Deleted Deleted;
        public File File;
        public FileSystemInfo fileSystemInfo;
        public Folder Folder;
        public Image Image;
        public GeoCoordinates Location;
        public Photo Photo;
        public SearchResult SearchResult;
        public Shared Shared;
        public SpecialFolder SpecialFolder;
        public Video Video;

        [JsonProperty("@name.conflictBehavior")]
        public String ConflictBehavior { get; set; }
    }
}