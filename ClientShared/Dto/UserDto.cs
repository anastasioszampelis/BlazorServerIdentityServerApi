using ClientShared.Dto.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientShared.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRoleDto UserRole {get;set;}
        public string Email { get; set; }
        public bool Active { get; set; }

    }
}
