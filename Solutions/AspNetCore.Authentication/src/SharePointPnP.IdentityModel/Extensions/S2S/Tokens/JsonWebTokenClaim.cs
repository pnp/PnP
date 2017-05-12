namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    public class JsonWebTokenClaim
    {
        private string _claimType;

        private string _value;

        public string ClaimType
        {
            get
            {
                return this._claimType;
            }
        }

        public string Value
        {
            get
            {
                return this._value;
            }
        }

        public JsonWebTokenClaim(string claimType, string value)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("claimType", claimType);
            this._claimType = claimType;
            this._value = value;
        }
    }
}
