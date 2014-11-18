/* Based on reflectored code coming from Microsoft.IdentityModel.Protocols.WSTrust.Bindings.WSTrustBindingBase class */

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace OfficeDevPnP.Core.IdentityModel.WSTrustBindings
{
    public abstract class WSTrustBinding : Binding
    {
        private bool _enableRsaProofKeys;
        private SecurityMode _securityMode;
        private TrustVersion _trustVersion;

        protected abstract void ApplyTransportSecurity(HttpTransportBindingElement transport);
        protected abstract SecurityBindingElement CreateSecurityBindingElement();

        protected WSTrustBinding(SecurityMode securityMode) : this(securityMode, TrustVersion.WSTrust13)
        { 
        }

        protected WSTrustBinding(SecurityMode securityMode, TrustVersion trustVersion)
        {
            this._securityMode = SecurityMode.Message;
            this._trustVersion = TrustVersion.WSTrust13;
          
            if (trustVersion == null)
            {
                throw new ArgumentNullException("trustVersion");
            }
            
            this.ValidateTrustVersion(trustVersion);
            ValidateSecurityMode(securityMode);
            this._securityMode = securityMode;
            this._trustVersion = trustVersion;
        }

        protected virtual SecurityBindingElement ApplyMessageSecurity(SecurityBindingElement securityBindingElement)
        {
            if (securityBindingElement == null)
            {
                throw new ArgumentNullException("securityBindingElement");
            }
            
            if (TrustVersion.WSTrustFeb2005 == this._trustVersion)
            {
                securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;
            }
            else
            {
                securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
            }
            
            if (this._enableRsaProofKeys)
            {
                RsaSecurityTokenParameters item = new RsaSecurityTokenParameters
                {
                    InclusionMode = SecurityTokenInclusionMode.Never,
                    RequireDerivedKeys = false
                };
                securityBindingElement.OptionalEndpointSupportingTokenParameters.Endorsing.Add(item);
            }
            
            return securityBindingElement;
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();
            elements.Clear();
            if ((SecurityMode.Message == this._securityMode) || (SecurityMode.TransportWithMessageCredential == this._securityMode))
            {
                elements.Add(this.ApplyMessageSecurity(this.CreateSecurityBindingElement()));
            }
            elements.Add(this.CreateEncodingBindingElement());
            elements.Add(this.CreateTransportBindingElement());
            return elements.Clone();
        }

        protected virtual MessageEncodingBindingElement CreateEncodingBindingElement()
        {
            return new TextMessageEncodingBindingElement { ReaderQuotas = { MaxArrayLength = 0x200000, MaxStringContentLength = 0x200000 } };
        }

        protected virtual HttpTransportBindingElement CreateTransportBindingElement()
        {
            HttpTransportBindingElement element;
            
            if (SecurityMode.Message == this._securityMode)
            {
                element = new HttpTransportBindingElement();
            }
            else
            {
                element = new HttpsTransportBindingElement();
            }
            
            element.MaxReceivedMessageSize = 0x200000L;
            
            if (SecurityMode.Transport == this._securityMode)
            {
                this.ApplyTransportSecurity(element);
            }
            
            return element;
        }

        protected static void ValidateSecurityMode(SecurityMode securityMode)
        {
            if (((securityMode != SecurityMode.None) && (securityMode != SecurityMode.Message)) && ((securityMode != SecurityMode.Transport) && (securityMode != SecurityMode.TransportWithMessageCredential)))
            {
                throw new ArgumentOutOfRangeException("securityMode");
            }
            
            if (securityMode == SecurityMode.None)
            {
                throw new InvalidOperationException("ID3224");
            }
        }

        protected void ValidateTrustVersion(TrustVersion trustVersion)
        {
            if ((trustVersion != TrustVersion.WSTrust13) && (trustVersion != TrustVersion.WSTrustFeb2005))
            {
                throw new ArgumentOutOfRangeException("trustVersion");
            }
        }

        public bool EnableRsaProofKeys
        {
            get
            {
                return this._enableRsaProofKeys;
            }
            set
            {
                this._enableRsaProofKeys = value;
            }
        }

        public override string Scheme
        {
            get
            {
                TransportBindingElement element = this.CreateBindingElements().Find<TransportBindingElement>();

                if (element == null)
                {
                    return string.Empty;
                }
                
                return element.Scheme;
            }
        }

        public SecurityMode SecurityMode
        {
            get
            {
                return this._securityMode;
            }
            set
            {
                ValidateSecurityMode(value);
                this._securityMode = value;
            }
        }

        public TrustVersion TrustVersion
        {
            get
            {
                return this._trustVersion;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                
                this.ValidateTrustVersion(value);
                this._trustVersion = value;
            }
        }
    }
}