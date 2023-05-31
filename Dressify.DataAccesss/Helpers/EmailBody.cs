using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Helpers
{
    public class EmailBody
    {
       public static string ResetPasswordEmail(string token, string email)
        {
            return $@"<html>
<head>
</head>
<body style=""margin: 0;padding: 0;font-family: Arial, Helvetica, sans-serif;"">
  <div style=""height: auto;background: linear-gradient(to top,#abe1de 50%,#368F8B 90%) no-repeat;width: 400px;padding: 30px;"">
    <div>
        <div>
          <h1>Reset Your Password</h1>
          <hr>
          <p>This Email for reset password request for your Dressify account.</p>
          <p>please tap password below to change your password</p>
          <a href=""https://localhost:7115/resetPassword?email={email}&code={token}"" target=""_blank"" style=""background: #032725;padding: 10px;border: none;color: white;border-radius:4px ;display: block;margin: 0 auto;width:50% ;text-align: center;text-decoration: none;"">
             Reset Password 
          </a>
          <p>Kind Regards<br><br>
          Dressify
          </p>
        </div>
    </div>
  </div>

</body>
</html>";
        }
    }
}   