namespace Governance.TimerJobs.Policy
{
    /// <summary>
    /// NoncomplianceType describes the site collection non-compliance type for UX or Email generator to consume
    /// </summary>
    public enum NoncomplianceType
    {
        Expiring, //Site collection is in expiration time span.
        MissClassification, //Site collection is unclassified. including: ToU, Audience Scope, Business impact
        NoAdditionalSiteAdmin,
        MembershipReviewDelay,
        HasBroadAccessGroup
    }
}