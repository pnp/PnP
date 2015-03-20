using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.DocumentSetsWeb.Models
{
    public class HomeViewModel
    {
        private List<ContentTypeModel> _allowedContentTypes = new List<ContentTypeModel>();
        private List<FieldModel> _sharedFields = new List<FieldModel>();
        private List<FieldModel> _welcomeFields = new List<FieldModel>();

        public List<FieldModel> WelcomeFields
        {
            get { return _welcomeFields; }
            set { _welcomeFields = value; }
        }


        public List<FieldModel> SharedFields
        {
            get { return _sharedFields; }
            set { _sharedFields = value; }
        }

        public List<ContentTypeModel> AllowedContentTypes
        {
            get { return _allowedContentTypes; }
            set { _allowedContentTypes = value; }
        }
        public string CurrentUsername { get; set; }
    }
}