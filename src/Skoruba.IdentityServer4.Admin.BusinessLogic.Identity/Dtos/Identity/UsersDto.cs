using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
	public class UsersDto<TUserDto, TUserDtoKey> where TUserDto : UserDto<TUserDtoKey>
    {
		public UsersDto()
		{
			Users = new List<TUserDto>();
		}

	    public int PageSize { get; set; }

	    public int TotalCount { get; set; }

        public List<TUserDto> Users { get; set; }
	}
}