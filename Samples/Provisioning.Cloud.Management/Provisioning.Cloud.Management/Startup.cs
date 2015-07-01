//Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Provisioning.Cloud.Management.Startup))]
namespace Provisioning.Cloud.Management
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}