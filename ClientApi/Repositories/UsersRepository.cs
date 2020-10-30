using ClientApi.Models;
using ClientShared.Dto;
using ClientShared.Dto.Enums;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Repositories
{
    public interface IUsersRepository
    {
        public Tuple<bool, List<UserDto>, string> GetUsers();
    }
    public class UsersRepository : IUsersRepository
    {
        public Tuple<bool, List<UserDto>, string> GetUsers()
        {
            try
            {
                //throw new NotImplementedException();
                using (var connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=test;Database=BlazorOidc"))
                {
                    var usersResponse = new List<UserDto>();
                    connection.Open();
                    var usersSelected = connection.Query<User>("Select * from Users;").ToList();
                    foreach(var item in usersSelected)
                    {
                        usersResponse.Add(new UserDto()
                        {
                            Id = item.Id,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Email = item.Email,
                            Active = item.Active,
                            UserRole = (UserRoleDto)item.UserRoleId
                        });
                    }
                    return new Tuple<bool, List<UserDto>, string>(true, usersResponse, string.Empty);
                }

            }
            catch (Exception ex)
            {
                return new Tuple<bool, List<UserDto>, string>(false, null, ex.Message);
            }
            
        }
    }
}
