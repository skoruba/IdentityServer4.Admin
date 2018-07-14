namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity.Base
{
    public class BaseRoleClaimDto<TRoleId, TClaimId>
    {
        public TClaimId ClaimId { get; set; }

        public TRoleId RoleId { get; set; }
    }
}