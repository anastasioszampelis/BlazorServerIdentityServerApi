using ClientShared.Dto.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientShared.Dto
{
    public class UserAuthorizationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserRoleDto UserRole { get; set; }
        public DateTime Date { get; set; }
        public AuthorizationTypeDto AuthorizationType { get; set; }
    }
}
