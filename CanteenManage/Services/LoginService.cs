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
                e.EmployID == userId
                //&& e.Password == password
                )
                .FirstOrDefault();

            return userFound;
        }
        public async Task<Employee> IsValidEmployee(string userId, string name)
        {
            Employee? userFound = null;
            userFound = await canteenManageContext.Employees
                .Where(e =>
                e.EmployID == userId
                //&& e.Password == password
                )
                .FirstOrDefaultAsync();
            if (userFound == null)
            {
                Employee employee = new Employee()
                {
                    EmployID = userId,
                    Name = name,
                    IsActive = true,
                    EmployTypeId = (int)EmployTypeEnum.Employee,
                    Password = "",

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

        public string GenerateJSONWebToken(List<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfigProvider.GetSecretKey() ?? CustomDataConstants.DefaultSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: appConfigProvider.GetTokenIssuer(),
                audience: appConfigProvider.GetTokenAudience(),
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
