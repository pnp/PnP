using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OfficeDevPnP.Core.Framework.TimerJobs
{
    /// <summary>
    /// Class that holds the state information that's being stored in the web property bag of web that's being "processed"
    /// </summary>
    [DataContract]
    public class TimerJobRun
    {
        /// <summary>
        /// DateTime of the previous run attempt
        /// </summary>
        [DataMember]
        public DateTime? PreviousRun;
        /// <summary>
        /// Bool indicating if the previous run was successful
        /// </summary>
        [DataMember]
        public bool? PreviousRunSuccessful;
        /// <summary>
        /// Timer job version used during the previous run
        /// </summary>
        [DataMember]
        public string PreviousRunVersion;
        /// <summary>
        /// Property value collection used to store timer job custom properties
        /// </summary>
        [DataMember]
        public Dictionary<String, String> Properties;
    }
}
