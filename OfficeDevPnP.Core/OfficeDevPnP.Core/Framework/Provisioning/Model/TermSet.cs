using System;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Extensions;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public partial class TermSet : IEquatable<TermSet>
    {
        #region Private Members
        private List<Term> _terms = new List<Term>();
        private Guid _id;
        #endregion

        #region Public Members
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public int? Language { get; set; }

        public bool IsOpenForTermCreation { get; set; }

        public bool IsAvailableForTagging { get; set; }

        public string Owner { get; set; }

        public List<Term> Terms
        {
            get { return _terms; }
            private set { _terms = value; }
        }
        #endregion

        #region Constructors

        public TermSet()
        {
        }

        public TermSet(Guid id, string name, int? language, bool isAvailableForTagging, bool isOpenForTermCreation, List<Term> terms)
        {
            this.Id = id;
            this.Name = name;
            this.Language = language;
            this.IsAvailableForTagging = isAvailableForTagging;
            this.IsOpenForTermCreation = isOpenForTermCreation;
            if (terms != null)
            {
                this.Terms.AddRange(terms);
            }
        }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                this.Id,
                this.Name,
                this.Description,
                this.Language,
                this.IsOpenForTermCreation,
                this.IsAvailableForTagging,
                this.Owner,
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
            return (this.Id == other.Id &&
                    this.Name == other.Name &&
                    this.Description == other.Description &&
                    this.Language == other.Language &&
                    this.IsOpenForTermCreation == other.IsOpenForTermCreation &&
                    this.IsAvailableForTagging == other.IsAvailableForTagging &&
                    this.Owner == other.Owner &&
                    this.Terms.DeepEquals(other.Terms));
        }

        #endregion
    }
}
