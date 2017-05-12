using Microsoft.IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    public class SimpleSymmetricKeySecurityToken : System.IdentityModel.Tokens.SecurityToken
    {
        private string id;

        private System.DateTime effectiveTime;

        private byte[] key;

        private System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> securityKeys;

        public override string Id
        {
            get
            {
                return this.id;
            }
        }

        public override System.DateTime ValidFrom
        {
            get
            {
                return this.effectiveTime;
            }
        }

        public override System.DateTime ValidTo
        {
            get
            {
                return System.DateTime.MaxValue;
            }
        }

        public int KeySize
        {
            get
            {
                return this.key.Length * 8;
            }
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> SecurityKeys
        {
            get
            {
                return this.securityKeys;
            }
        }

        public SimpleSymmetricKeySecurityToken(byte[] key) : this(UniqueId.CreateUniqueId(), key)
        {
        }

        public SimpleSymmetricKeySecurityToken(string id, byte[] key)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("id", id);
            Utility.VerifyNonNullArgument("key", key);
            if (key.Length <= 0)
            {
                throw new System.ArgumentException("The key length must be greater then zero.");
            }
            this.id = id;
            this.effectiveTime = System.DateTime.UtcNow;
            this.key = new byte[key.Length];
            System.Buffer.BlockCopy(key, 0, this.key, 0, key.Length);
            this.securityKeys = this.CreateSymmetricSecurityKeys(this.key);
        }

        public byte[] GetKeyBytes()
        {
            int num = this.key.Length;
            byte[] array = new byte[num];
            System.Buffer.BlockCopy(this.key, 0, array, 0, num);
            return array;
        }

        private System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> CreateSymmetricSecurityKeys(byte[] key)
        {
            return new System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey>(new System.Collections.Generic.List<System.IdentityModel.Tokens.SecurityKey>
            {
                new System.IdentityModel.Tokens.InMemorySymmetricSecurityKey(key)
            });
        }

        public override bool MatchesKeyIdentifierClause(System.IdentityModel.Tokens.SecurityKeyIdentifierClause keyIdentifierClause)
        {
            Utility.VerifyNonNullArgument("keyIdentifierClause", keyIdentifierClause);
            return keyIdentifierClause is SymmetricIssuerKeyIdentifierClause || base.MatchesKeyIdentifierClause(keyIdentifierClause);
        }
    }
}
