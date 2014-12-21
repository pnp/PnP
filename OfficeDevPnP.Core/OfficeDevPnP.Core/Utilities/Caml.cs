using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Utilities {
    /// <summary>
    /// Use this class to build your CAML xml and avoid XML issues.
    /// </summary>
    /// <example>
    /// CAML.ViewQuery(
    ///     CAML.Where(
    ///         CAML.And(
    ///             CAML.Eq(CAML.FieldValue("Project", "Integer", "{0}")),
    ///             CAML.Geq(CAML.FieldValue("StartDate","DateTime", CAML.Today()))
    ///         )
    ///     ),
    ///     CAML.OrderBy(
    ///         new OrderByField("StartDate", false),
    ///         new OrderByField("Title")
    ///     ),
    ///     rowLimit: 5
    /// );
    /// </example>
    public static class CAML {
        const string VIEW_XML_WRAPPER = "<View><Query>{0}{1}</Query><RowLimit>{2}</RowLimit></View>";
        const string FIELD_VALUE = "<FieldRef Name='{0}' {1}/><Value Type='{2}'>{3}</Value>";
        const string FIELD_VALUE_ID = "<FieldRef ID='{0}' {1} /><Value Type='{2}'>{3}</Value>";
        const string WHERE_CLAUSE = "<Where>{0}</Where>";
        const string GENERIC_CLAUSE = "<{0}>{1}</{0}>";
        const string CONDITION_CLAUSE = "<{0}>{1}{2}</{0}>";

        public static readonly string Me = "<UserId />";
        public static readonly string Month = "<Month />";
        public static readonly string Now = "<Now />";

        /// <summary>
        /// Creates the &lt;Today /&gt; node.
        /// </summary>
        /// <param name="offset">Time offset from today (+5 days or -5 days, for example).</param>
        /// <returns></returns>
        public static string Today(int? offset = null) {
            if (offset.HasValue)
                return string.Format("<Today Offset='{0}' />", offset.Value);
            return "<Today />";
        }

        /// <summary>
        /// Root &lt;View&gt; and &lt;Query&gt; nodes.
        /// </summary>
        /// <param name="whereClause">&lt;Where&gt; node.</param>
        /// <param name="orderByClause">&lt;OrderBy&gt; node.</param>
        /// <param name="rowLimit">&lt;RowLimit&gt; node.</param>
        /// <returns>String to be used in CAML queries</returns>
        public static string ViewQuery(string whereClause = "", string orderByClause = "", int rowLimit = 100) {
            return string.Format(VIEW_XML_WRAPPER, whereClause, orderByClause, rowLimit);
        }

        /// <summary>
        /// Creates both a &lt;FieldRef&gt; and &lt;Value&gt; nodes combination for Where clauses.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValueType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FieldValue(string fieldName, string fieldValueType, string value, string additionalFieldRefParams = "") {
            return string.Format(FIELD_VALUE, fieldName, additionalFieldRefParams, fieldValueType, value);
        }

        /// <summary>
        /// Creates both a &lt;FieldRef&gt; and &lt;Value&gt; nodes combination for Where clauses.
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="fieldValueType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FieldValue(Guid fieldId, string fieldValueType, string value, string additionalFieldRefParams = "") {
            return string.Format(FIELD_VALUE_ID, fieldId.ToString(), additionalFieldRefParams, fieldValueType, value);
        }

        public static string OrderBy(params OrderByField[] fieldRefs) {
            var sb = new StringBuilder();
            foreach (var field in fieldRefs){
                sb.Append(field.ToString());
            }
            return string.Format(GENERIC_CLAUSE, CamlClauses.OrderBy, sb.ToString());
        }

        public static string Where(string conditionClause) {
            return string.Format(GENERIC_CLAUSE, CamlClauses.Where, conditionClause);
        }

        #region Conditions
        public static string And(string clause1, params string[] conditionClauses) {
            return Condition(CamlConditions.And, clause1, conditionClauses);
        }

        public static string Or(string clause1, params string[] conditionClauses) {
            return Condition(CamlConditions.Or, clause1, conditionClauses);
        } 
        #endregion

        #region Comparisons
        public static string BeginsWith(string fieldValue) {
            return Comparison(CamlComparisions.BeginsWith, fieldValue);
        }
        public static string Contains(string fieldValue) {
            return Comparison(CamlComparisions.Contains, fieldValue);
        }
        public static string DateRangesOverlap(string fieldValue) {
            return Comparison(CamlComparisions.DateRangesOverlap, fieldValue);
        }
        public static string Eq(string fieldValue) {
            return Comparison(CamlComparisions.Eq, fieldValue);
        }
        public static string Geq(string fieldValue) {
            return Comparison(CamlComparisions.Geq, fieldValue);
        }
        public static string Gt(string fieldValue) {
            return Comparison(CamlComparisions.Gt, fieldValue);
        }
        public static string In(string fieldValue) {
            return Comparison(CamlComparisions.In, fieldValue);
        }
        public static string Includes(string fieldValue) {
            return Comparison(CamlComparisions.Includes, fieldValue);
        }
        public static string IsNotNull(string fieldValue) {
            return Comparison(CamlComparisions.IsNotNull, fieldValue);
        }
        public static string IsNull(string fieldValue) {
            return Comparison(CamlComparisions.IsNull, fieldValue);
        }
        public static string Leq(string fieldValue) {
            return Comparison(CamlComparisions.Leq, fieldValue);
        }
        public static string Lt(string fieldValue) {
            return Comparison(CamlComparisions.Lt, fieldValue);
        }
        public static string Neq(string fieldValue) {
            return Comparison(CamlComparisions.Neq, fieldValue);
        }
        public static string NotIncludes(string fieldValue) {
            return Comparison(CamlComparisions.NotIncludes, fieldValue);
        }
        #endregion

        static string Comparison(CamlComparisions comparison, string fieldValue) {
            return string.Format(GENERIC_CLAUSE, comparison, fieldValue);
        }

        static string Condition(CamlConditions condition, string clause1, params string[] comparisonClauses) {
            var formattedString = clause1;

            foreach (var clause in comparisonClauses) {
                formattedString = string.Format(CONDITION_CLAUSE, condition, formattedString, clause);
            }

            return formattedString;
        }

        enum CamlComparisions {
            BeginsWith, Contains, DateRangesOverlap,
            Eq, Geq, Gt, In, Includes, IsNotNull, IsNull,
            Leq, Lt, Neq, NotIncludes
        }
        enum CamlConditions { And, Or }
        enum CamlClauses { Where, OrderBy, GroupBy }
    }

    public class OrderByField {
        const string ORDERBY_FIELD = "<FieldRef Name='{0}' Ascending='{1}' />";
        public OrderByField(string name) : this(name, true) { }
        public OrderByField(string name, bool ascending) {
            Name = name;
            Ascending = ascending;
        }
        public string Name { get; set; }
        public bool Ascending { get; set; }
        public override string ToString() {
            return string.Format(ORDERBY_FIELD, Name, Ascending.ToString().ToUpper());
        }
    }
}
