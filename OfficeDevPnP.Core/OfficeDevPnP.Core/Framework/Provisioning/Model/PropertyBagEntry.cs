using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class PropertyBagEntry : ModelBase<PropertyBagEntry>, IModel
    {
        #region Properties

        #endregion

        #region Comparison code

        #endregion
    }

    public class ModelBase<T> : IModel, IEquatable<ModelBase<T>>
    {
        Func<int> hashFormatter;
        public ModelBase(Func<int> hashFormatter)
        {
            this.hashFormatter = hashFormatter;
        }

        //Default hashformatter. Other classes can just override their GetHashCode or inject a Func 
        public ModelBase()
        {
            this.hashFormatter = () =>
            {
                return (String.Format("{0}|{1}",
                    this.Key,
                    this.Value).GetHashCode());
            };
        }
        public override int GetHashCode()
        {
            return hashFormatter();
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Equals(IModel other)
        {
            return (this.Key == other.Key &&
                this.Value == other.Value);
        }


        public bool Equals(ModelBase<T> other)
        {
            return (this.Key == other.Key && this.Value == other.Value);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is T))
            {
                return(false);
            }
            return (Equals((ModelBase<T>)obj));
        }

    }

    public interface IModel
    {
        string Key { get; set; }
        string Value { get; set; }
    }

}
