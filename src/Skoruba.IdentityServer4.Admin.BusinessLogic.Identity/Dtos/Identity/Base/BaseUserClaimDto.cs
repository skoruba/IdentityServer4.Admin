namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Base
{
    public class BaseUserClaimDto<TUserId>
    {
        public int ClaimId { get; set; }

        public TUserId UserId { get; set; }
    }
}