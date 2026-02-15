using Crops_Price_Tracker.Infrastructure;
using Crops_Price_Tracker.Models.UserData;
using Crops_Price_Tracker.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Crops_Price_Tracker.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<OtpVerification> _otp;
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _config;

        public AuthService(MongoDBService mongo, IOptions<MongoSettings> settings, IConfiguration config)
        {
            _otp = mongo.Database.GetCollection<OtpVerification>(settings.Value.OtpCollection);
            _users = mongo.Database.GetCollection<User>(settings.Value.UserCollection);
            _config = config;
        }

        // ---------------- GENERATE OTP ----------------
        public async Task GenerateOtp(string mobile)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            await _otp.InsertOneAsync(new OtpVerification
            {
                Mobile = mobile,
                Otp = otp,
                Expiry = DateTime.UtcNow.AddMinutes(5)
            });

            Console.WriteLine($"OTP for {mobile}: {otp}");
        }

        // ---------------- VERIFY OTP ----------------
        public async Task<User?> VerifyOtp(string mobile, string otp)
        {

            var record = await _otp
                .Find(x => x.Mobile == mobile && x.Otp == otp)
                .SortByDescending(x => x.Expiry )
                .FirstOrDefaultAsync();

            //Invalid or expired OTP
            if (record == null || record.Expiry <= DateTime.UtcNow)
                return null;

            //  Check or create user
            var user = await _users
                .Find(x => x.Mobile == mobile)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                user = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Mobile = mobile,
                    CreatedAt = DateTime.UtcNow
                };

                await _users.InsertOneAsync(user);
            }

            //  Remove all OTPs for that user
            await _otp.DeleteManyAsync(x => x.Mobile == mobile);

            return user;
        }



    //    public string GenerateJwt(User user)
    //    {
    //        var key = _config["Jwt:Key"];
    //        if (string.IsNullOrEmpty(key))
    //            throw new Exception("JWT Key is missing");

    //        var claims = new[]
    //        {
    //    new Claim("userId", user.UserId),
    //    new Claim("mobile", user.Mobile)
    //};

    //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    //        var token = new JwtSecurityToken(
    //            claims: claims,
    //            expires: DateTime.UtcNow.AddHours(6),
    //            signingCredentials: credentials
    //        );

    //        return new JwtSecurityTokenHandler().WriteToken(token);
    //    }

    }
}
