namespace Skoruba.IdentityServer4.Admin.ViewModels.Identity.Base
{
    public class BaseUserClaimDto<TUserId, TClaimId>
    {
        public TClaimId ClaimId { get; set; }

        public TUserId UserId { get; set; }
    }
}