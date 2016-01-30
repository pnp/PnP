using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines the properties of an audio file in OneDrive for Business
    /// </summary>
    public class Audio
    {
        public String Album;
        public String AlbumArtist;
        public String Artist;
        public Int64 Bitrate;
        public String Composers; 
        public String Copyright; 
        public Int16 Disc; 
        public Int16 DiscCount; 
        public Int64 Duration; 
        public String Genre; 
        public Boolean HasDrm;
        public Boolean IsVariableBitrate; 
        public String Title; 
        public Int32 Track;
        public Int32 TrackCount;
        public Int32 Year; 
    }
}