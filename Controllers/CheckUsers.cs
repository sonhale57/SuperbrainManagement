using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;

namespace SuperbrainManagement.Controllers
{
    public class CheckUsers
    {
        public static bool checkcookielogin()
        {
            try
            {
                string cookie = System.Web.HttpContext.Current.Request.Cookies["check"]["login"].ToString();
                string iduser = System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString();
                string ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                MD5Hash md5h = new MD5Hash();
                String check = md5h.Decrypt(cookie.ToString());
                iduser = md5h.Decrypt(iduser.ToString());
                if (check != (("Yvaphatco" + iduser)))
                {
                    return false;
                }
                else
                { return true; }
            }
            catch { return false; }
        }
        /// <summary>
        /// Lấy IdUser đăng nhập
        /// </summary>
        /// <returns></returns>
        public static string iduser()
        {
            try
            {
                MD5Hash md5 = new MD5Hash();
                string idlogin = md5.Decrypt(System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString());
                return idlogin;
            }
            catch { return ""; }
        }
        /// <summary>
        /// Code check phân quyền của user
        /// </summary>
        
    }
}