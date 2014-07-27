namespace Contoso.Core
{
    using Contoso.Core.UserProfileService;

    /// <summary>
    /// The Profile Property Mapper
    /// </summary>
    public class ProfilePropertyMapper : PropertyBase
    {
        #region Methods

        /// <summary>
        /// Processes the property information
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <param name="value">The value.</param>
        /// <param name="action">The action being executed.</param>
        /// <returns>
        /// The parsed property value
        /// </returns>
        public override object Process(object propertyData, string value, BaseAction action)
        {
            var data = propertyData as PropertyData;

            if (data != null)
            {
                data.IsValueChanged = true;
                data.Values = new ValueData[1];
                data.Values[0] = new ValueData();
                data.Values[0].Value = value;

                return data;
            }

            return value;
        }

        #endregion Methods
    }
}