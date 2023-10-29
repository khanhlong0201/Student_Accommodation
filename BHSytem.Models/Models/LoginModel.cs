using System;
using System.ComponentModel.DataAnnotations;

namespace BHSytem.Models.Models
{
    public class LoginModel
    {

        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string UserCode { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
        public int UserId { get; set; }
        public int? UserIdSAP { get; set; }
        public string UserCodeSAP { get; set; }
        public string PasswordSAP { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string ReCaptcha { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public DateTime Birthday { get; set; }
        public string MobilPhone { get; set; }
        public string HomePhone { get; set; }
        public string Email { get; set; }
        public int Position { get; set; }
        //public DateTime DateJoin { get; set; }
        //public DateTime DateLeft { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        //public DateTime CreateDateTime { get; set; }
        //public int CreateEmpID { get; set; }
        //public DateTime AlterDateTime { get; set; }
        //public string AlterEmpID { get; set; }
        //public int AlterCount { get; set; }
        public int Branch { get; set; }

        public DateTime LastDateLogon { get; set; }
        public int CountLogon { get; set; }

        public bool IsAdmin { get; set; }

        public int Task { get; set; }
        public string Type { get; set; }

        public bool RevAlert { get; set; }

        public string FontName { get; set; }
        public double FontSize { get; set; }
        public string superUser { get; set; }
        public string otp { get; set; }
        public string DflWhs { get; set; }
        public string cardCode { get; set; }

    }

    /// <summary>
    /// tìm trên KH con
    /// </summary>
    public class DocFilterModel
    {
        public string code_Phase { get; set; } = "";
        public string code_Cluster { get; set; } = "";
        public string code_Machine { get; set; } = "";
        public string code_Machine_Struc { get; set; } = "";
        public string kind { get; set; } = "";
        public string code_Job { get; set; } = "";

        public string name_Phase { get; set; } = "";
        public string name_Cluster { get; set; } = "";
        public string name_Machine { get; set; } = "";
        public string name_Machine_Struc { get; set; } = "";
        public string name_Job { get; set; } = "";

        public string json { get; set; } = ""; //LongTran 20230326 biến dùng để fillter theo phòng ban ( xem nhiêu phòng ban)
    }

    public class LoginRequestModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string newpassword { get; set; }
        public string branch { get; set; }
        public string database { get; set; }
        public string imei { get; set; }
        public string version { get; set; }
        public string fbToken { get; set; }
        public string token { get; set; }
        public string email { get; set; }
    }

    public class LoginResponseDetailModel
    {
        public int status { get; set; }
        public string message { get; set; }
        public int userID { get; set; }
        public string token { get; set; }
        public string typeLogin { get; set; }
        public string connectTime { get; set; }
        public string versionNo { get; set; }
        public string iMEI { get; set; }
        public string expire { get; set; }
        public string branch { get; set; }
        public string superuser { get; set; }//SUPERUSER
        public string otp { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string cardCode { get; set; }
    }

    public class AuthorizationModel
    {
        public string? menuId { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public bool isCheckAll { get; set; }
        public string statusID { get; set; }
        public string codeVtype { get; set; }
        public string nameVtype { get; set; }
        public string parent { get; set; } //longtran 2030616 ( để group by thoe nhóm menu trên phân quyền)

        public bool isShowCheck { get; set; }
    }


}
