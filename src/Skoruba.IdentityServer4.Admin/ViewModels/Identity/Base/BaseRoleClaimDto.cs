namespace Skoruba.IdentityServer4.Admin.ViewModels.Identity.Base
{
    public class BaseRoleClaimDto<TRoleId, TClaimId>
    {
        public TClaimId ClaimId { get; set; }

        public TRoleId RoleId { get; set; }
    }
}