using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Linq;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IUserBusiness userBusiness;

        public UserController(IUserBusiness userBusiness)
        {
            this.userBusiness = userBusiness;
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Registration(UserModel model)
        {
            var isExists = userBusiness.IsRegisteredAlready(model.Email);
            if (isExists)
            {
                return Ok(new ResponseModel<string> { Status = true, Message = "Email exists already..." });
            }
            else
            {
                var result = userBusiness.UserRegistration(model);
                if (result != null)
                {
                    return Ok(new ResponseModel<UserModel> { Status = true, Message = "Registration Successful..........", Data = result });
                }
                else
                {
                    return BadRequest(new ResponseModel<UserModel> { Status = false, Message = "Registration Failed..............." });
                }
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UserLoginModel model)
        {
            var result = userBusiness.UserLogin(model);
            if (result != null)
            {
                return Ok(new ResponseModel<string> { Status = true, Message = "login successfull", Data = result });

            }
            else
            {
                return BadRequest(new ResponseModel<UserModel> { Status = false, Message = "login failed" });
            }
        }

        [HttpPut]
        [Route("UpdateUser")]
        public IActionResult UpdateUser(UserModel model)
        {
            var result = userBusiness.UpdateUser(model);

            if (result != null)
            {
                return Ok(new ResponseModel<string> { Status = true, Message = "user Updated successfully." });
            }
            else
            {
                return BadRequest(new ResponseModel<string> { Status = false, Message = "user Updation failed." });
            }
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public IActionResult DeleteUser(int id)
        {
            var result = userBusiness.DeleteUser(id);
            if (result != null)
            {
                return Ok(new ResponseModel<string> { Status = true, Message = "user deleted successfully." });
            }
            else
            {
                return BadRequest(new ResponseModel<string> { Status = false, Message = "user deletion failed." });
            }
        }

        [HttpPost]
        [Route("ForgotPwd")]
        public IActionResult ForgotPwd(string Email)
        {
            var isExists = userBusiness.IsRegisteredAlready(Email);
            if (isExists)
            {
                var result = userBusiness.ForgetPassword(Email);
                if (result != null)
                {
                    return Ok(new ResponseModel<string> { Status = true, Message = "Mail Sent Successfully..........", Data = result });
                }
                else
                {
                    return BadRequest(new ResponseModel<string> { Status = false, Message = "Mail Sending Failed.............." });
                }
            }
            else
            {
                return BadRequest(new ResponseModel<string> { Status = true, Message = "Email doesn't exists ..........." });
            }
        }


        [Authorize]
        [HttpPut]
        [Route("ResetNewPassword")]
        public IActionResult ResetNewPassword(ResetPwdModel model)
        {
            string email = User.Claims.FirstOrDefault(x => x.Type == "Email").Value;
            var result = userBusiness.ResetNewPassword(email, model.password, model.confirmPassword);
            if (result != null)
            {
                return Ok(new ResponseModel<string> { Status = true, Message = "Password resetted successfully." });
            }
            else
            {
                return BadRequest(new ResponseModel<string> { Status = false, Message = "Resetting password failed." });
            }
        }
    }
}
