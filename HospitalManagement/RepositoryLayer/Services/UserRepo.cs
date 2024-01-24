using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Services
{
    public class UserRepo : IUserRepo
    {
        private readonly IConfiguration configuration;

        public UserRepo(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public UserModel UserRegistration(UserModel user)
        {
            string encryptPwd = EncryptPassword(user.Pwd);
            if (user != null)
            {
                using (SqlConnection con = new SqlConnection(this.configuration["ConnectionStrings:Hospital"]))
                {
                    SqlCommand cmd = new SqlCommand("uspAddUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Pwd", encryptPwd);
                    cmd.Parameters.AddWithValue("@MobNo", user.MobNo);
                    cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(user.DOB));
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return user;
            }
            else
            {
                return null;
            }
        }

        public string UpdateUser(UserModel user)
        {
            string encryptPwd = EncryptPassword(user.Pwd);
            if (user != null)
            {
                using (SqlConnection con = new SqlConnection(this.configuration["ConnectionStrings:Hospital"]))
                {
                    SqlCommand cmd = new SqlCommand("uspUpdateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", user.UserId);
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Pwd", encryptPwd);
                    cmd.Parameters.AddWithValue("@MobNo", user.MobNo);
                    cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(user.DOB));
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return "User is Updated Successfully..........";
            }
            else
            {
                return null;
            }
        }


        public string DeleteUser(int id)
        {
            if (id >= 1)
            {
                using (SqlConnection con = new SqlConnection(this.configuration["ConnectionStrings:Hospital"]))
                {
                    SqlCommand cmd = new SqlCommand("uspDeleteUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return "User Is Deleted Successfully......................";
            }
            else
            {
                return null;
            }
        }

        public string UserLogin(UserLoginModel login)
        {
            string encryptPwd = EncryptPassword(login.Pwd);
            if (login != null)
            {
                UserModel user = new UserModel();

                using (SqlConnection con = new SqlConnection(this.configuration["ConnectionStrings:Hospital"]))
                {
                    SqlCommand cmd = new SqlCommand("uspUserLogin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", login.Email);
                    cmd.Parameters.AddWithValue("@Pwd", encryptPwd);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows == true)
                    {
                        while (rdr.Read())
                        {
                            user.UserId = rdr.GetInt32("UserId");
                            user.UserName = rdr["UserName"].ToString();
                            user.Email = rdr["Email"].ToString();
                            user.Pwd = rdr["Pwd"].ToString();
                            user.MobNo = rdr["MobNo"].ToString();
                            user.DOB = rdr["DOB"].ToString();
                            user.Address = rdr["Address"].ToString();
                            user.CreatedAt = Convert.ToDateTime(rdr["CreatedAt"]);
                            user.UpdatedAt = Convert.ToDateTime(rdr["UpdatedAt"]);
                        }
                        var token = GenerateToken(user.Email, user.UserId);
                        return token;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }


        public string ForgetPassword(string EmailId)
        {
            if (EmailId != null)
            {
                UserModel user = new UserModel();

                using (SqlConnection con = new SqlConnection(this.configuration["ConnectionStrings:Hospital"]))
                {
                    SqlCommand cmd = new SqlCommand("uspForgotPwd", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", EmailId);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows == true)
                    {
                        while (rdr.Read())
                        {
                            user.UserId = rdr.GetInt32("UserId");
                            user.UserName = rdr["UserName"].ToString();
                            user.Email = rdr["Email"].ToString();
                            user.Pwd = rdr["Pwd"].ToString();
                            user.MobNo = rdr["MobNo"].ToString();
                            user.DOB = rdr["DOB"].ToString();
                            user.Address = rdr["Address"].ToString();
                            user.CreatedAt = Convert.ToDateTime(rdr["CreatedAt"]);
                            user.UpdatedAt = Convert.ToDateTime(rdr["UpdatedAt"]);
                        }
                        var token = GenerateToken(user.Email, user.UserId);
                        MSMQModel mSMQModel = new MSMQModel();
                        mSMQModel.SendMessage(token, user.Email, user.UserName);
                        return token.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public string ResetNewPassword(string Email, string password, string confirmPassword)
        {
            var pwd = EncryptPassword(password);
            var confpwd = EncryptPassword(confirmPassword);

            if (Email != null && pwd != null && confpwd != null)
            {
                if (pwd.Equals(confpwd))
                {
                    using (SqlConnection con = new SqlConnection(this.configuration["ConnectionStrings:Hospital"]))
                    {
                        SqlCommand cmd = new SqlCommand("uspResetNewPwd", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Parameters.AddWithValue("@Pwd", pwd);
                        cmd.Parameters.AddWithValue("@ConfPwd", confpwd);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return "Password resetted Successfully..........";
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }



        public bool IsRegisteredAlready(string email)
        {
            int emailCount = 0;
            if (email != null)
            {
                using (SqlConnection con = new SqlConnection(this.configuration["ConnectionStrings:Hospital"]))
                {
                    SqlCommand cmd = new SqlCommand("uspIsEmailExists", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int emailExistsCount))
                    {
                        emailCount = emailExistsCount;
                    }
                    con.Close();
                    if (emailCount > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            else
            {
                return false;
            }

        }

        public static string EncryptPassword(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }


        private string GenerateToken(string Email, int UserId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Email", Email),
                new Claim("UserId", UserId.ToString())
            };
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
