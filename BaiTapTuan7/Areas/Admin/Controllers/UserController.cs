﻿using BaiTapTuan7.Areas.Admin.Model;
using BaiTapTuan7.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class UserController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: User
        public ActionResult Index()
        {

            List<tb_Users> lst = db.tb_Users.ToList();
            ViewBag.UserLists = lst;
            return View();
        }
        public ActionResult AddUser()
        {
            ViewBag.ListOfType = TypeList.GetTypeList();
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(AddUserModel us1)
        {

            if (ModelState.IsValid)
            {
                var check = db.tb_Users.FirstOrDefault(m => m.Username == us1.Username);
                if (check == null)
                {
                    tb_Users us = new tb_Users();
                    us.Username = us1.Username;
                    us.Usertype = us1.Usertype;
                    us.Email = us1.Email;
                    us.Password = us1.Password;
                    us.Block = true;
                    us.RegisterDate = DateTime.Now;
                    db.tb_Users.Add(us);
                    db.SaveChanges();
                    tb_Users uz = db.tb_Users.FirstOrDefault(m => m.Username == us.Username);
                    if (uz.Usertype == "teacher")
                    {
                        tb_Teacher tc = new tb_Teacher();
                        tc.TeacherFirstName = us1.FirstName;
                        tc.TeacherLastName = us1.LastName;
                        tc.Gmail = us1.Email;
                        tc.UserId = uz.Id;
                        db.tb_Teacher.Add(tc);
                    }
                    else if (uz.Usertype == "student")
                    {
                        tb_Student stu = new tb_Student();
                        stu.FirstName = us1.FirstName;
                        stu.LastName = us1.LastName;
                        stu.Gmail = us1.Email;
                        stu.UserId = uz.Id;
                        db.tb_Student.Add(stu);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    us1.AddUserError = "Username has exists";
                    return View("AddUser", us1);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult ActiveUser(int? Id)
        {

            if (ModelState.IsValid)
            {
                tb_Users us = db.tb_Users.Find(Id);
                if (us != null)
                {
                    us.Block = false;
                    db.SaveChanges();
                    return RedirectToAction("UserList", us);
                }
                else
                {
                    return RedirectToAction("UserList", us);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult UnactiveUser(int? Id)
        {

            if (ModelState.IsValid)
            {
                tb_Users us = db.tb_Users.Find(Id);
                if (us != null)
                {
                    us.Block = true;
                    db.SaveChanges();
                    return RedirectToAction("UserList");
                }
                else
                {
                    return RedirectToAction("UserList");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult RemoveUser(int? Id)
        {

            if (ModelState.IsValid)
            {
                tb_Users us = db.tb_Users.Find(Id);
                if (us == null)
                {
                    return RedirectToAction("UserList");
                }
                else
                {
                    if (us.Usertype == "teacher")
                    {
                        return RedirectToAction("RemoveTeacherByUserId", us);
                    }
                    else
                    {
                        return RedirectToAction("RemoveStudentByUserId", us);
                    }
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult RemoveTeacherByUserId(tb_Users us)
        {
            int userid = us.Id;

            if (ModelState.IsValid)
            {
                tb_Teacher tc = db.tb_Teacher.FirstOrDefault(m => m.UserId == userid);
                db.Entry(tc).State = EntityState.Deleted;
                db.Entry(us).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("UserList", "User");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult RemoveStudentByUserId(tb_Users us)
        {
            int userid = us.Id;

            if (ModelState.IsValid)
            {
                tb_Student tc = db.tb_Student.FirstOrDefault(m => m.UserId == userid);
                db.Entry(tc).State = EntityState.Deleted;
                db.Entry(us).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("UserList", "User");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

    }
}