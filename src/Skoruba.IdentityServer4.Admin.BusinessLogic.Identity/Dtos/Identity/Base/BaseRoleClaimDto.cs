namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Base
{
    public class BaseRoleClaimDto<TRoleId>
    {
        public int ClaimId { get; set; }

        public TRoleId RoleId { get; set; }
    }
}