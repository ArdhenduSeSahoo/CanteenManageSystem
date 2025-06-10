using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CanteenManage.CanteenRepository.Contexts;
using CanteenManage.CanteenRepository.Models;
using CanteenManage.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CanteenManage.Services
{
    public class LoginService
    {
        private readonly CanteenManageDBContext canteenManageContext;
        private readonly AppConfigProvider appConfigProvider;
        public LoginService(CanteenManageDBContext canteenManageContext, AppConfigProvider appConfigProvider)
        {
            this.canteenManageContext = canteenManageContext;
            this.appConfigProvider = appConfigProvider;
        }

        public Employee IsValidUser(string userId, string password)
        {
            var userFound = canteenManageContext.Employees
                .Where(e =>
                e.EmployeeID.ToLower() == userId.ToLower()
                && e.Password == password
                )
                .FirstOrDefault();

            return userFound;
        }
        public async Task<Employee> GetOrAddEmployee(string userId, string name, string EmployEmail)
        {
            Employee? userFound = null;
            userFound = await canteenManageContext.Employees
                .Where(e =>
                e.EmployeeID.ToLower() == userId.ToLower()
                //&& e.Password == password
                )
                .FirstOrDefaultAsync();
            if (userFound == null)
            {
                Employee employee = new Employee()
                {
                    EmployeeID = userId,
                    Name = name,
                    IsActive = true,
                    EmployeeTypeId = (int)EmployTypeEnum.Employee,
                    Password = "",
                    Email = EmployEmail,
                    PhoneNumber = "",
                    IsLogin = false

                };
                canteenManageContext.Employees.Add(employee);
                await canteenManageContext.SaveChangesAsync();
                userFound = employee;
            }
            else
            {
                if (userFound.Name != name)
                {
                    userFound.Name = name;
                    canteenManageContext.Employees.Update(userFound);
                    await canteenManageContext.SaveChangesAsync();

                }
            }

            return userFound;
        }

        public string GenerateJSONWebToken(List<Claim> claims, DateTime expiresDateTime)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfigProvider.GetSecretKey() ?? CustomDataConstants.DefaultSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: appConfigProvider.GetTokenIssuer(),
                audience: appConfigProvider.GetTokenAudience(),

                expires: expiresDateTime,
                claims: claims,
                signingCredentials: credentials
                );
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Issuer = appConfigProvider.GetTokenIssuer(),
                Audience = appConfigProvider.GetTokenAudience(),
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.Now,
                Expires = expiresDateTime,
                SigningCredentials = credentials
            };
            var tokens = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(tokens);
        }

        public async Task LoginUpdateEmployee(string employee_e_id)
        {
            canteenManageContext.Employees.Where(e => e.EmployeeID == employee_e_id)
                .ExecuteUpdate(e => e.SetProperty(x => x.IsLogin, true));
        }
        public async Task LogOutUpdateEmployee(string employee_e_id)
        {
            canteenManageContext.Employees.Where(e => e.EmployeeID == employee_e_id)
                .ExecuteUpdate(e => e.SetProperty(x => x.IsLogin, false));
        }

    }
}
