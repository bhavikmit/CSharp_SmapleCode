using Colten.BAL.Interface;
using Colten.Common;
using Colten.Common.EncryptionHelper;
using Colten.Logging.Contracts;
using Colten.Model;
using Colten.Model.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Colten.API.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        #region Fields

        private readonly IUserRepository _userRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly AppSettings _appSettings;
        private readonly ILoggerManager _logger;
        private readonly ILocalResourceRepository _localResourceRepository;
        private readonly IAttoryneyRepository _attoryneyRepository;

        #endregion

        #region Constructor

        public UserController(IUserRepository userService,
            ILoggerManager logger,
            IOptions<AppSettings> appSettings,
            IEmailRepository emailRepository,
            ILocalResourceRepository localResourceRepository,
            IAttoryneyRepository attoryneyRepository)
        {
            _userRepository = userService;
            _emailRepository = emailRepository;
            _logger = logger;
            _appSettings = appSettings.Value;
            _localResourceRepository = localResourceRepository;
            _attoryneyRepository = attoryneyRepository;
        }

        #endregion

        #region Authenticate

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel loginModel)
        {
            APIResponse apiResponse = new APIResponse();
            TokenModel user = await _userRepository.Authenticate(loginModel.Username, loginModel.Password, loginModel.UserType);
            if (user == null)
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = Constant.UsernamePasswordIncorrectMessage;
                return Ok(apiResponse);
            }
            else if (user.Status != 1)
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = _localResourceRepository.GetValue(user.Status);
                return Ok(apiResponse);
            }
            apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
            apiResponse.ResponseData = user;
            return Ok(apiResponse);
        }

        #endregion

        [AllowAnonymous]
        [HttpPost("AttorneyRegister")]
        public async Task<IActionResult> AttorneyRegister([FromBody] RegisterModel registerModel)
        {
            APIResponse apiResponse = new APIResponse();
            KeyValuePair<long, long> attorneyMobile = new KeyValuePair<long, long>(0, 0);

            if (ModelState.IsValid)
            {
                registerModel.Password = (!string.IsNullOrEmpty(registerModel.Password) ? EncryptionHelper.ComputeMD5Hash(registerModel.Password) : string.Empty);

                attorneyMobile = await _userRepository.CreateAttorneyMobile(registerModel);

                if (attorneyMobile.Value <= 0)
                {
                    apiResponse.ResponseStatus = ResponseStatus.INVALID;
                    apiResponse.ResponseMessage = _localResourceRepository.GetValue(attorneyMobile.Value);
                    return Ok(apiResponse);
                }

                var encryptedId = (!string.IsNullOrEmpty(attorneyMobile.Key.AsString())) ? EncryptionHelper.EncryptString128Bit(attorneyMobile.Key.AsString()) : string.Empty;
                var encryptedEmail = EncryptionHelper.EncryptString128Bit(registerModel.Username);

                var apiVersion = HttpContext.GetRequestedApiVersion();
                string callbackUrl = StringHelper.ConfirmationLink(_appSettings.WebBaseUrl, $"{ apiVersion.MajorVersion.AsString()}.{apiVersion.MinorVersion.AsString()}", encryptedId, encryptedEmail);

                await _emailRepository.SendVerificationLinkEmail(registerModel.Username, callbackUrl);

                apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
                apiResponse.ResponseData = attorneyMobile.Key;
            }
            else
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = Constant.EnterAllFieldMessage;
            }

            return Ok(apiResponse);
        }

        //User Registration
        [AllowAnonymous]
        [HttpPost("UserRegister")]
        public async Task<IActionResult> UserRegister([FromBody] UserRegisterModel userRegisterModel)
        {
            APIResponse apiResponse = new APIResponse();
            KeyValuePair<long, long> user = new KeyValuePair<long, long>(0, 0);

            if (ModelState.IsValid)
            {
                userRegisterModel.Password = (!string.IsNullOrEmpty(userRegisterModel.Password) ? EncryptionHelper.ComputeMD5Hash(userRegisterModel.Password) : string.Empty);

                user = await _userRepository.CreateUser(userRegisterModel);
                if (user.Value <= 0)
                {
                    apiResponse.ResponseStatus = ResponseStatus.INVALID;
                    apiResponse.ResponseMessage = _localResourceRepository.GetValue(user.Value);
                    return Ok(apiResponse);
                }

                var encryptedId = (!string.IsNullOrEmpty(user.Key.AsString())) ? EncryptionHelper.EncryptString128Bit(user.Key.AsString()) : string.Empty;
                var encryptedEmail = EncryptionHelper.EncryptString128Bit(userRegisterModel.UserName);

                var apiVersion = HttpContext.GetRequestedApiVersion();
                string callbackUrl = StringHelper.ConfirmationLink(_appSettings.WebBaseUrl, $"{ apiVersion.MajorVersion.AsString()}.{apiVersion.MinorVersion.AsString()}", encryptedId, encryptedEmail);

                await _emailRepository.SendVerificationLinkEmail(userRegisterModel.UserName, callbackUrl);

                apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
                apiResponse.ResponseMessage = _localResourceRepository.GetValue(user.Value);
                apiResponse.ResponseData = user.Key;
            }
            else
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = Constant.EnterAllFieldMessage;
            }
            return Ok(apiResponse);

        }

        //Email verification
        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailModel confirmEmailModel)
        {
            APIResponse apiResponse = new APIResponse();
            if (string.IsNullOrWhiteSpace(confirmEmailModel.EncryptedUserId))
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = Constant.UserIdRequiredMessage;
                return Ok(apiResponse);
            }
            try
            {
                var encryptedId = EncryptionHelper.DecryptString128Bit(confirmEmailModel.EncryptedUserId).AsInt64();
                var encryptedEmail = EncryptionHelper.DecryptString128Bit(confirmEmailModel.Email);

                int emailVerify = await _userRepository.EmailVerify(encryptedId);
                if (emailVerify > 0)
                {
                    apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
                    apiResponse.ResponseMessage = Constant.EmailVerificationAndLoginMessage;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.ResponseStatus = ResponseStatus.INVALID;
                    apiResponse.ResponseMessage = Constant.NoUserFoundOfEmailMessage;
                    return Ok(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                apiResponse.ResponseStatus = ResponseStatus.FAILURE;
                apiResponse.ResponseMessage = Constant.ErrorMessage;
                return Ok(apiResponse);
            }
        }


        #region ForgotPassword

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            APIResponse apiResponse = new APIResponse();

            ResetPasswordDataModel resetPasswordData = await _userRepository.ForgotPassword(forgotPasswordModel);

            if (resetPasswordData != null)
            {
                //need to pass roleId while request from admin panel else it should be 0.                
                if (ValidateUserType.ValidateUserTypeToRole(resetPasswordData.RoleId, forgotPasswordModel.RoleId))
                {
                    apiResponse.ResponseStatus = ResponseStatus.EMAILNOTEXISTS;
                    apiResponse.ResponseMessage = Constant.EmailNotExists;
                    return Ok(apiResponse);
                }

                if (resetPasswordData.STATUS == (int)ResponseStatus.SUCCESS)
                {
                    resetPasswordData.EncryptedUserId = (!string.IsNullOrEmpty(resetPasswordData.Id.AsString()) ? EncryptionHelper.EncryptString128Bit(resetPasswordData.Id.AsString()) : string.Empty);

                    var apiVersion = HttpContext.GetRequestedApiVersion();
                    var baseUrl = string.Empty;
                    if (resetPasswordData.RoleId == (int)UserType.Admin)
                    {
                        baseUrl = _appSettings.WebAdminBaseUrl;
                    }
                    else
                    {
                        baseUrl = _appSettings.WebBaseUrl;
                    }
                    string callbackUrl = StringHelper.ForgotPasswordLink(baseUrl, $"{ apiVersion.MajorVersion.AsString()}.{apiVersion.MinorVersion.AsString()}", resetPasswordData.EncryptedUserId);

                    await _emailRepository.SendForgotPassword(resetPasswordData.Email, callbackUrl);

                    apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
                    apiResponse.ResponseMessage = Constant.ForgotPasswordSuccessMessage;
                }
                else if (resetPasswordData.STATUS == (int)ResponseStatus.EMAILNOTEXISTS)
                {
                    apiResponse.ResponseStatus = ResponseStatus.EMAILNOTEXISTS;
                    apiResponse.ResponseMessage = Constant.EmailNotExists;
                }
                else
                {
                    apiResponse.ResponseStatus = ResponseStatus.INVALID;
                    apiResponse.ResponseMessage = Constant.ErrorMessage;
                    return Ok(apiResponse);
                }
            }
            else
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = Constant.ErrorMessage;
                return Ok(apiResponse);
            }
            return Ok(apiResponse);
        }

        #endregion

        #region ResetPassword

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDataModel resetPasswordDataModel)
        {
            APIResponse apiResponse = new APIResponse();

            resetPasswordDataModel.NewPassword = (!string.IsNullOrEmpty(resetPasswordDataModel.NewPassword) ? EncryptionHelper.ComputeMD5Hash(resetPasswordDataModel.NewPassword) : string.Empty);

            if (resetPasswordDataModel.IsAdmin == true)
            {
                resetPasswordDataModel.EncryptedUserId = resetPasswordDataModel.UserId.AsString();
            }
            else
            {
                resetPasswordDataModel.EncryptedUserId = (!string.IsNullOrEmpty(resetPasswordDataModel.EncryptedUserId) ? EncryptionHelper.DecryptString128Bit(resetPasswordDataModel.EncryptedUserId) : string.Empty);
            }

            long resetPassword = await _userRepository.ResetPassword(resetPasswordDataModel.EncryptedUserId.AsInt64(), resetPasswordDataModel.NewPassword);
            if (resetPassword == 0)
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = Constant.ErrorMessage;
                return Ok(apiResponse);
            }
            if (resetPassword > 0)
            {
                apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
                apiResponse.ResponseMessage = Constant.PasswordResetSuccess;
                apiResponse.ResponseData = resetPassword;
            }

            return Ok(apiResponse);
        }

        #endregion

        #region UserProfile

        [HttpGet("{id}")]
        public async Task<IActionResult> UserProfileDetail(long id)
        {
            APIResponse apiResponse = new APIResponse();

            UserDetailModel userDetail = await _userRepository.GetUserProfileByUserId(id);

            if (userDetail == null)
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = Constant.NoDataFound;
                return Ok(apiResponse);
            }
            apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
            apiResponse.ResponseData = userDetail;
            return Ok(apiResponse);

        }

        [AllowAnonymous]
        [HttpPost("UserProfileInsertUpdate")]
        public async Task<IActionResult> UserProfile([FromBody] UserDetailModel userDetailModel)
        {
            APIResponse apiResponse = new APIResponse();

            long updateProfile = await _attoryneyRepository.UpdateProfile(userDetailModel);

            if (updateProfile <= 0)
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = _localResourceRepository.GetValue(updateProfile);
                return Ok(apiResponse);
            }

            apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
            apiResponse.ResponseMessage = _localResourceRepository.GetValue(updateProfile);
            return Ok(apiResponse);
        }

        #endregion
       
        #region User List

        [Route("UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList([FromQuery] UserListModel userListModel, [FromQuery] UserSearchModel userSearch)
        {
            APIResponse apiResponse = new APIResponse();

            UserListDataModel userListData = await _userRepository.GetUserList(userListModel, userSearch);

            if (userListData.TotalRecord <= 0)
            {
                apiResponse.ResponseStatus = ResponseStatus.INVALID;
                apiResponse.ResponseMessage = Constant.NoDataFound;
                return Ok(apiResponse);
            }
            apiResponse.ResponseStatus = ResponseStatus.SUCCESS;
            apiResponse.ResponseData = userListData;
            return Ok(apiResponse);

        }

        #endregion
       
    }
}
