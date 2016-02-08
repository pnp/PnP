using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Base class for any entity in the model
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// The unique ID of the entity
        /// </summary>
        public String Id { get; set; }
    }
}