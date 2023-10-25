using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WishApp.Core.ServicesWithInterface;
using WishApp.Core.Models;
using WishApp.Core.Utils;
using WishApp.Data;
using WishApp.Data.Models.Auth;
using System.Net;
using System.Reflection.Metadata;

namespace WishApp.Core.Services;

public class AuthService
{
    IIdentityService _identityService;
    AppDbContext _db;
    IStaticProvider _st;
    MasterDataService _md;
    AppSetting _appSetting;
    IHttpClientService _httpClientService;

    public AuthService(IIdentityService identityService, AppDbContext db, IStaticProvider st, MasterDataService md, AppSetting appSetting, IHttpClientService httpClientService) {
        _identityService = identityService;
        _db = db;
        _st = st;
        _md = md;
        _appSetting = appSetting;
        _httpClientService = httpClientService;
    }

    private LoginResult GenerateLoginResult(UserClaim userClaim, bool isNew) {
        var persistedClaims = new List<Claim> {
                new(AppConst.Claim.UserId, userClaim.Id),
                new(AppConst.Claim.UserName, userClaim.UserName),
                new(AppConst.Claim.Email, userClaim.Email ?? ""),
                new(AppConst.Claim.UserType, ((int)userClaim.UserType).ToString()),
            };

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddDays(1),
            claims: persistedClaims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSetting.JwtIssuerSigningKey)),
                SecurityAlgorithms.HmacSha256)
        );

        return new LoginResult {
            User = userClaim,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            IsNew = isNew
        };
    }

    private UserClaim GenerateUserClaim(User user) {
        return new UserClaim {
            Id = user.Id,
            UserName = user.UserName!,
            Name = user.Name,
            UserType = user.UserType,
            PictureUrl = user.PictureUrl,
            Email = user.Email
        };
    }

    public async Task<ResponseResult<LoginResult>> Login(LoginParam param) {
        try {
            var user = _db.Users.FirstOrDefault(a => a.UserName == param.UserName);

            if(user == null) {
                throw new Exception("UserName or Password is incorrect");
            }

            if(!await _identityService.CheckPasswordAsync(user, param.Password)) {
                throw new Exception("UserName or Password is incorrect");
            }

            var userClaim = new UserClaim {
                Id = user.Id,
                UserName = user.UserName!
            };

            var loginResult = GenerateLoginResult(userClaim, false);

            return new ResponseResult<LoginResult> {
                Success = true,
                Result = loginResult
            };
        }
        catch(Exception ex) {
            return new ResponseResult<LoginResult> {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ResponseResult<LoginResult>> LoginAGuest() {
        try {
            var ts = _st.UtcNow.ToString("yyyyMMddHHmmss");
            var nameStamp = $"Guest-{ts[10]}{ts[8]}{ts[7]}{ts[12]}{ts[9]}{ts[2]}{ts[3]}{ts[0]}{ts[1]}{ts[11]}{ts[4]}{ts[13]}{ts[6]}{ts[5]}";

            var user = new User {
                Id = nameStamp,
                Name = nameStamp,
                UserName = nameStamp,
                UserType = UserType.Guest
            };

            var result = await _identityService.CreateAsync(user, _st.NewGuid + "-A");

            if(!result.Succeeded) {
                throw new Exception(result.Errors.First().Description);
            }

            var userClaim = GenerateUserClaim(user);

            var loginResult = GenerateLoginResult(userClaim, true);

            return new ResponseResult<LoginResult> {
                Success = true,
                Result = loginResult
            };
        }
        catch(Exception ex) {
            return new ResponseResult<LoginResult> {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ResponseResult<LoginResult>> LoginAsGoogleAccount(string idToken, string? guestId) {
        try {
            var url = $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}";

            var googleClaim = await (new Func<Task<GoogleIdClaim>>(async () => {
                var response = await _httpClientService.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                if(!response.IsSuccessStatusCode) {
                    throw new Exception($"Failed to verify Google ID Token. StatusCode: {response.StatusCode}. Error message: {responseString}");
                }

                var result = System.Text.Json.JsonSerializer.Deserialize<GoogleIdClaim>(responseString);

                if(result?.aud != _appSetting.GoogleSsoClientId) {
                    throw new Exception($"Token is not intended for this application");
                }

                return result;
            })());

            var (userClaim, isNew) = await (new Func<Task<(UserClaim, bool)>>(async () => {
                var existingUser = _db.Users.FirstOrDefault(a => a.Email == googleClaim.email);

                if(existingUser != null) {
                    return (GenerateUserClaim(existingUser), false);
                }
                else {
                    var newUser = new User {
                        Id = _st.NewGuid,
                        Name = googleClaim.name,
                        UserName = googleClaim.email,
                        Email = googleClaim.email,
                        PictureUrl = googleClaim.picture,
                        UserType = UserType.Member
                    };

                    var result = await _identityService.CreateAsync(newUser, _st.NewGuid + "-A");

                    if(!result.Succeeded) {
                        throw new Exception(result.Errors.First().Description);
                    }

                    if(!string.IsNullOrEmpty(guestId)) {
                        await _md.TransferDataOwnership(guestId, newUser.Id);
                    }

                    return (GenerateUserClaim(newUser), true);
                }
            })());

            var loginResult = GenerateLoginResult(userClaim, isNew);

            return new ResponseResult<LoginResult> {
                Success = true,
                Result = loginResult
            };
        }
        catch(Exception ex) {
            return new ResponseResult<LoginResult> {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
