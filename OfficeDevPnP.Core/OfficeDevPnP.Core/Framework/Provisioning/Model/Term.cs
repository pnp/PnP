using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Extensions;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class Term : IEquatable<Term>
    {
        #region Private Members
        private List<Term> _terms = new List<Term>();
        private List<TermLabel> _labels = new List<TermLabel>();
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private Dictionary<string, string> _localProperties = new Dictionary<string, string>();
        #endregion

        #region Public Members
        public Guid ID { get; set; }
        public string Name { get; set; }
        public String Description { get; set; }
        public String Owner { get; set; }
        public Boolean? IsAvailableForTagging { get; set; }
        public int? Language { get; set; }
        public String CustomSortOrder { get; set; }

        public List<Term> Terms
        {
            get { return _terms; }
            private set { _terms = value; }
        }

        public List<TermLabel> Labels
        {
            get { return _labels; }
            private set { _labels = value; }
        }

        public Dictionary<string, string> Properties
        {
            get { return _properties; }
            private set { _properties = value; }
        }

        public Dictionary<string, string> LocalProperties
        {
            get { return _localProperties; }
            private set { _localProperties = value; }
        }
        #endregion

        #region Constructors

        public Term()
        {
        }

        public Term(Guid id, string name, int? language, List<Term> terms, List<TermLabel> labels, Dictionary<string, string> properties, Dictionary<string, string> localProperties)
        {
            this.ID = id;
            this.Name = name;
            this.Language = language;

            if (terms != null)
            {
                this.Terms.AddRange(terms);
            }

            if (labels != null)
            {
                this.Labels.AddRange(labels);
            }
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    this.Properties.Add(property.Key, property.Value);
                }
            }
            if (localProperties != null)
            {
                foreach (var property in localProperties)
                {
                    this.Properties.Add(property.Key, property.Value);
                }
            }
        }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}",
                this.ID,
                this.Name,
                this.Description,
                this.Language,
                this.Owner,
                this.IsAvailableForTagging,
                this.CustomSortOrder,
                this.Labels.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Terms.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Properties.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.LocalProperties.Aggregate(0, (acc, next) => acc += next.GetHashCode())
                ).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Term))
            {
                return (false);
            }
            return (Equals((Term)obj));
        }

        public bool Equals(Term other)
        {
            return (this.ID == other.ID &&
                this.Name == other.Name &&
                this.Description == other.Description &&
                this.Language == other.Language &&
                this.Owner == other.Owner &&
                this.IsAvailableForTagging == other.IsAvailableForTagging &&
                this.CustomSortOrder == other.CustomSortOrder &&
                this.Labels.DeepEquals(other.Labels) &&
                this.Terms.DeepEquals(other.Terms) &&
                this.Properties.DeepEquals(other.Properties) &&
                this.LocalProperties.DeepEquals(other.LocalProperties));
        }

        #endregion
    }
}
