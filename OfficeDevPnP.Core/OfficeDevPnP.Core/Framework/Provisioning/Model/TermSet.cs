using System;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Extensions;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class TermSet : IEquatable<TermSet>
    {
        #region Private Members
        private List<Term> _terms = new List<Term>(); 
        #endregion

        #region Public Members
        public Guid ID { get; set; }
        public string Name { get; set; }

        public List<Term> Terms
        {
            get { return _terms;}
            private set { _terms = value;}
        }
        #endregion

        #region Constructors

        public TermSet()
        {
        }

        public TermSet(Guid id, string name, List<Term> terms)
        {
            this.ID = id;
            this.Name = name;

            if (terms != null)
            {
                this.Terms.AddRange(terms);
            }
        }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}",
                this.ID.GetHashCode(),
                this.Name.GetHashCode(),
                 this.Terms.Aggregate(0, (acc, next) => acc += next.GetHashCode())
                ).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TermSet))
            {
                return (false);
            }
            return (Equals((TermSet)obj));
        }

        public bool Equals(TermSet other)
        {
            return (this.ID == other.ID &&
                this.Name == other.Name &&
                this.Terms.DeepEquals(other.Terms));
        }

        #endregion
    }
}
