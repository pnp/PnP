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
        #endregion

        #region Public Members
        public Guid ID { get; set; }
        public string Name { get; set; }

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
        #endregion

        #region Constructors

        public Term()
        {
        }

        public Term(Guid id, string name, List<Term> terms, List<TermLabel> labels, Dictionary<string, string> properties)
        {
            this.ID = id;
            this.Name = name;

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
                foreach (var key in properties.Keys)
                {
                    this.Properties.Add(key, properties[key]);
                }
            }
        }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}|{3}|{4}",
                this.ID.GetHashCode(),
                this.Name.GetHashCode(),
                this.Labels.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Terms.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Properties.Aggregate(0, (acc, next) => acc += next.GetHashCode())
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
                this.Labels.DeepEquals(other.Labels) &&
                this.Terms.DeepEquals(other.Terms) &&
                this.Properties.DeepEquals(other.Properties));
        }

        #endregion

    }
}
