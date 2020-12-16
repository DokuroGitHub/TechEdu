﻿using BaiTapTuan7.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace BaiTapTuan7.Controllers
{
    public class HomeController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DangNhap()
        {
            ViewBag.Message = "Login";
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            string taikhoan = f["username"].ToString();
            string matkhau = f["password"].ToString();
            //string matkhaumd5 = GetMD5(matkhau);
            tb_Users us = db.tb_Users.SingleOrDefault(n => n.Username == taikhoan && n.Password == matkhau);
            //nếu user nhập đúng mật khẩu
            if (us != null)
            {
                if (us.Block == true)
                {
                    return Content("er_block");
                }
                Session["user"] = us;
                Session["userName"] = us.Username;

                if (us.Usertype == "admin")
                {
                    Session["admin"] = db.tb_Teacher.FirstOrDefault(m => m.UserId == us.Id);
                    return Content("/Admin");
                }
                else if (us.Usertype == "student")
                {
                    Session["student"] = db.tb_Student.FirstOrDefault(m => m.UserId == us.Id);
                    return Content("/Student");
                }
                else if (us.Usertype == "teacher")
                {
                    Session["teacher"] = db.tb_Teacher.FirstOrDefault(m => m.UserId == us.Id);
                    return Content("/Teacher");
                }
                else
                {
                    return Content("false");
                }
            }
            else
            {
                return Content("false");
            }
        }

    }
}