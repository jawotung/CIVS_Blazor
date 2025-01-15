using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class AuthRepositories : IAuthRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public AuthRepositories(
            AppDbContext dBContext, 
            ADClass addClass, 
            CommonClass commonClass,
            IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }


        Dictionary<EnumValidateCredDetails, object> dResult;

        public async Task<ReturnLoginCredentials> ValidateCredentials(ParamLoginCredentials value)
        {
            var obj = new ReturnLoginCredentials();
            obj.StatusCode = "01";
            try
            {
                if (UserNotFoundDisabledDeactivated(value.UserId))
                {
                    _commonClass.Log(string.Format("Login :{0} UserId Not Found or Activated or Disabled.", value.UserId));
                    obj.StatusMessage = "Invalid Credentials.";
                }
                else
                {
                    value.Password = value.Password;//.Encrypt();
                    dResult = _adClass.ValidateADCredInfo(value.UserId, value.Password);
                    if (dResult[EnumValidateCredDetails.isValid].GetBoolValue() == true)
                    {
                        string clientIP = value.IpAddress;
                        string lastSession = GetUserSession(value.UserId);
                        DateTime? lastLoginDate = GetUserLastLoginDate(value.UserId);
                        string userAgent = value.UserAgent;

                        //lastSession has value
                        if (lastSession != "")
                        {
                            //check if session form same machine ip
                            bool loginFrSameIP = lastSession.Contains(clientIP);
                            double lastLoginTime = DateTime.Now.Subtract(lastLoginDate.Value).TotalMinutes;

                            _commonClass.Log(string.Format("Login :{0} User has active session on {1} using {2} at {3}.", value.UserId, clientIP, userAgent, lastLoginDate));

                            obj.StatusMessage = "User has active session.";
                            //lastlogin not yet 10 min for now
                            if (lastLoginTime < 10)
                            {
                                switch (loginFrSameIP)
                                {
                                    case true:
                                        _commonClass.Log(string.Format("Login :{0} User on IP {1} using {2} active session since {3} minutes.", value.UserId, clientIP, userAgent, lastLoginTime));
                                        break;
                                    case false:
                                        _commonClass.Log(string.Format("Login :{0} User attempt login on IP {1} using {2}.", value.UserId, clientIP, userAgent));
                                        break;
                                }
                            } 
                        }
                        else
                        {
                            string session = $"{userAgent} {clientIP}";
                            await SetLastLoginDateAsync(value.UserId);
                            await SetLastLoginSessionAsync(value.UserId, session);
                            obj.UserId = value.UserId;
                            FillLoginModel(obj);
                            obj.StatusCode = "00";
                            obj.StatusMessage = "SUCCESS";
                        }
                    }
                    else
                    {
                        obj.StatusCode = "01";
                        obj.StatusMessage = dResult[EnumValidateCredDetails.ErrorMessage].GetValue();
                        _commonClass.Log(string.Format("Login :{0} {1}", value.UserId, dResult[EnumValidateCredDetails.ErrorMessage].GetValue()));
                        //string key = dResult[ValidateCredDetails.ErrorMessage].GetValue().Contains("Password") ? "Password" : "UserId";
                        //ModelState.AddModelError(key, dResult[ValidateCredDetails.ErrorMessage].GetValue());
                        //ModelState.AddModelError("UserId", dResult[EnumValidateCredDetails.ErrorMessage].GetValue());
                        //ModelState.SetModelValue("UserID", "", "");
                        //ModelState.SetModelValue("Password", "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }

            return obj;
        }

        public async Task<ReturnGenericStatus> AuthLogout(string? sMode)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID; // User.Claims.SingleOrDefault(e => e.Type == "UserID").Value;
                _commonClass.Log(string.Format("{0}Logout {1}.", sMode ?? "", userId));

                if (!userClaims.IsNull())
                {
                    await SetLastLoginSessionAsync(userId);
                }
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
            }catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }
        public async Task<ReturnGenericStatus> TagInactiveUsers(int? mode)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                DateTime thresholdDate = DateTime.Today.AddDays(-30);
                IQueryable<UserModel> userModel = _dBContext.UserModels;
                //switch (mode ?? 0)
                //{
                //    case 1:
                //        userModel = _dBContext.UserModels.Where(u => DateTime.Today.Subtract(u.LastLoginDate.Date).TotalDays > 30 && u.Isdisabled == false);
                //        break;
                //    case 2:
                //        userModel = _dBContext.UserModels.Where(u => DateTime.Today.Subtract(u.LastLoginDate.Date).TotalDays > 90 && u.Isdeleted == false);
                //        break;
                //}
                switch (mode ?? 0)
                {
                    case 1:
                        userModel = _dBContext.UserModels
                            .Where(u => EF.Functions.DateDiffDay(u.LastLoginDate.Date, thresholdDate) > 30 && u.Isdisabled == false);
                        break;
                    case 2:
                        userModel = _dBContext.UserModels
                            .Where(u => EF.Functions.DateDiffDay(u.LastLoginDate.Date, thresholdDate) > 90 && u.Isdeleted == false);
                        break;
                }

                if (mode == 1 || mode == 2)
                {
                    obj.StatusCode = "00";
                    obj.StatusMessage = string.Format("Number of Users Affected : {0}", userModel.Count());

                    if (mode == 1)
                        await userModel.ForEachAsync(x => x.Isdisabled = true);

                    if (mode == 2)
                        await userModel.ForEachAsync(x => x.Isdeleted = true);

                    await _dBContext.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        public async Task<ReturnGenericStatus> ClearAllSession()
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var userClaims = _userClaims.GetClaims();
                var usersToUpdate = await _dBContext.UserModels
                                .Where(u => EF.Functions.DateDiffMinute(u.LastLoginDate.Date, DateTime.Today) >= 10
                                         && EF.Functions.DateDiffDay(u.LastLoginDate.Date, DateTime.Today) >= 0
                                         && u.LastLoginSession != ""
                                         && u.Isdisabled == false)
                                .ToListAsync();

                if (usersToUpdate.Any())
                {
                    foreach (var user in usersToUpdate)
                    {
                        user.LastLoginSession = "";
                    }

                    _dBContext.Update(usersToUpdate);

                    //var authToUpdate = await _dBContext.UserAuthenticationModels
                    //           .Where(u => u.CreatedAt.HasValue
                    //                    && EF.Functions.DateDiffMinute(u.CreatedAt.Value, DateTime.Now) >= 10
                    //                    && EF.Functions.DateDiffDay(u.CreatedAt.Value.Date, DateTime.Today) >= 0
                    //                    && u.IsValid == true)
                    //           .ToListAsync();
                    //if (authToUpdate.Any())
                    //{
                    //    foreach (var auth in authToUpdate)
                    //    {
                    //        auth.IsValid = false;
                    //    }
                    //    _dBContext.Update(authToUpdate);
                    //}
                    //await _dBContext.SaveChangesAsync();
                }

                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        public async Task<ReturnGenericStatus> SaveApiAuthentication(ParamSaveApiAuthentication value)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var userAuthToUpdated = await _dBContext.UserAuthenticationModels
                                                .Where(u => u.UserId == value.UserId && u.IsValid == true)
                                                .ToListAsync();
                foreach (var userAuth in userAuthToUpdated)
                {
                    userAuth.UpdatedBy = "SYSTEM_USER";
                    userAuth.UpdatedAt = DateTime.Now;
                }
                await _dBContext.SaveChangesAsync();

                var newPostImage = new UserAuthenticationModel
                {
                    AuthType = "JWT",
                    UserId = value.UserId,
                    AccessToken = value.AccessToken,
                    RefreshToken = value.RefreshToken,
                    AccessTokenExpiry = value.AccessTokenExpiry,
                    RefreshTokenExpiry = value.RefreshTokenExpiry,
                    IpAddress = value.IpAddress,
                    UserAgent = value.UserAgent
                };

                _dBContext.UserAuthenticationModels.Add(newPostImage);
                await _dBContext.SaveChangesAsync();
            }
            catch(Exception ex) 
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        private bool UserNotFoundDisabledDeactivated(string userID)
        {
            var tmpUserModel = _dBContext.UserModels.FirstOrDefault(e => e.UserId == userID);
            if (tmpUserModel.IsNull())
                return true;
            else
                if (tmpUserModel.Isdeleted == true || tmpUserModel.Isdisabled == true)
                return true;

            return false;
        }

        private string GetUserSession(string userID)
        {
            var tmpUserModel = _dBContext.UserModels.FirstOrDefault(e => e.UserId == userID);
            if (!tmpUserModel.IsNull())
                if (tmpUserModel.Isdeleted == false || tmpUserModel.Isdisabled == false)
                    return tmpUserModel.LastLoginSession ?? "";

            return "";
        }

        private DateTime? GetUserLastLoginDate(string userID)
        {
            var tmpUserModel = _dBContext.UserModels.FirstOrDefault(e => e.UserId == userID);
            if (!tmpUserModel.IsNull())
                if (tmpUserModel.Isdeleted == false || tmpUserModel.Isdisabled == false)
                    return tmpUserModel.LastLoginDate;

            return null;
        }
        private async Task SetLastLoginDateAsync(string userID)
        {
            var userModel = GetUserModel(userID);
            userModel.LastLoginDate = DateTime.Now;

            _dBContext.Update(userModel);
            await _dBContext.SaveChangesAsync();
        }

        private UserModel GetUserModel(string userID)
        {
            return _dBContext.UserModels.Find(GetUserModelID(userID));
        }

        private int GetUserModelID(string userID)
        {
            var tmpUserModel = _dBContext.UserModels.FirstOrDefault(e => e.UserId == userID);
            if (!tmpUserModel.IsNull())
                if (tmpUserModel.Isdeleted == false || tmpUserModel.Isdisabled == false)
                    return tmpUserModel.Id;

            return 0;
        }

        private async Task SetLastLoginSessionAsync(string userID, string session = "")
        {
            var userModel = GetUserModel(userID);
            userModel.LastLoginSession = session;

            _dBContext.Update(userModel);
            await _dBContext.SaveChangesAsync();
        }

        private void FillLoginModel(ReturnLoginCredentials loginModel)
        {
            var userModel = GetUserModel(loginModel.UserId);
            DisplayUserDetails(userModel);

            loginModel.DisplayName = userModel.UserDisplayName;
            loginModel.GroupingDesc = userModel.GroupingDesc;
            loginModel.UserTypeDesc = userModel.UserTypeDesc;
            loginModel.BranchOfAssignmentCode = userModel.BranchOfAssignment;
            loginModel.BranchOfAssignmentDesc = userModel.BranchOfAssignmentDesc;
            loginModel.MenuDesc = _commonClass.GetMenus(_commonClass.GetGroupMenu(userModel.GroupCode));
            loginModel.LastLoginDate = userModel.LastLoginDate;
            loginModel.BCPBranch = GetBCPBranch(userModel.BranchOfAssignment.GetValue());
        }

        private BCPBranch GetBCPBranch(string branchOfAssignment)
        {
            string buddyBranch = _commonClass.GetBuddyBranch(branchOfAssignment);
            return new BCPBranch
            {
                BranchBuddyCode = buddyBranch,
                BranchBuddyDesc = _commonClass.GetBranchOfAssignmentDesc(buddyBranch)
            };
        }

        private void DisplayUserDetails(UserModel userModel)
        {
            userModel.UserDisplayName = userModel.UserDisplayName.GetValue();
            userModel.GroupCode = _commonClass.GetGroupIDbyUserType(userModel.UserType.GetValue());
            userModel.GroupingDesc = _commonClass.GetGroupDesc(userModel.GroupCode.GetValue());
            userModel.UserTypeDesc = _commonClass.GetUserTypeDesc(userModel.UserType.GetValue());
            userModel.BranchOfAssignmentDesc = _commonClass.GetBranchOfAssignmentDesc(userModel.BranchOfAssignment.GetValue());
            userModel.LastLoginDate = userModel.LastLoginDate;
        }

    }
}
