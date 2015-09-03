using System;
using System.Reflection;

namespace Governance.TimerJobs.RemediationUx.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}