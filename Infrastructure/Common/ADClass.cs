using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using Microsoft.EntityFrameworkCore;
using WebAPI;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Common
{
    public class ADClass
    {
        private readonly IConfiguration _config;
        public ADClass(IConfiguration config)
        {
            _config = config;
        }

        public Dictionary<EnumValidateCredDetails, object> ValidateADCredInfo(string user, string password)
        {
            Dictionary<EnumValidateCredDetails, object> _bIsValid = new Dictionary<EnumValidateCredDetails, object>() { };//= false;

            _bIsValid.Add(EnumValidateCredDetails.isValid, false);
            _bIsValid.Add(EnumValidateCredDetails.DisplayName, "");
            _bIsValid.Add(EnumValidateCredDetails.ErrorMessage, "");

            bool _bIsCredValid = ValidateADCred(user, password);
            //bool _bIsCredValid = true;
            UserPrincipal _oUserPrincipal = GetADCredInfo(user);

            try
            {
                if (!_oUserPrincipal.IsNull())
                {
                    if (_bIsCredValid)
                    {
                        _bIsValid[EnumValidateCredDetails.isValid] = _bIsCredValid;
                        _bIsValid[EnumValidateCredDetails.DisplayName] = _oUserPrincipal.DisplayName;
                    }
                    else
                    {
                        _bIsValid[EnumValidateCredDetails.ErrorMessage] = "Invalid Credentials.";
                        //_bIsValid[ValidateCredDetails.ErrorMessage] = (_oUserPrincipal.LastBadPasswordAttempt.ToString() != "") ? "Incorrect Password " :
                        //                                                      (_oUserPrincipal.IsAccountLockedOut()) ? "Account is Locked." :
                        //                                                      (_oUserPrincipal.Enabled.ToString() != "") ? "Account Disabled" : "";
                    }
                }
                else
                {
                    _bIsValid[EnumValidateCredDetails.ErrorMessage] = "Invalid Credentials.";//_oUserPrincipal.LastBadPasswordAttempt.ToString();
                }
            }
            catch (Exception ex)
            {
                _bIsValid[EnumValidateCredDetails.ErrorMessage] = ex.Message;
            }
            return _bIsValid;
        }

        public bool ValidateADCred(string user, string password)
        {
            bool _bIsCredValid = false;
            //password.Decrypt()
            using (PrincipalContext _oPrincipalContext = new PrincipalContext(ContextType.Domain, GetDomainName()))
            {
                _bIsCredValid = _oPrincipalContext.ValidateCredentials(user, password, ContextOptions.Negotiate);
            }
            return _bIsCredValid;
        }

        public UserPrincipal GetADCredInfo(string user)
        {
            UserPrincipal _oUserPrincipal;
            using (PrincipalContext _oPrincipalContext = new PrincipalContext(ContextType.Domain, GetDomainName()))
            {
                _oUserPrincipal = UserPrincipal.FindByIdentity(_oPrincipalContext, IdentityType.SamAccountName, user);
            }
            return _oUserPrincipal;
        }

        public string GetDomainName()
        {
            return _config["DomainName"];
        }
    }
}
