namespace Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity.Base
{
    public class BaseRoleClaimDto<TRoleId>
    {
        public int ClaimId { get; set; }

        public TRoleId RoleId { get; set; }
    }
}