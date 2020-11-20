﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class StudentController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Student
        public ActionResult Index()
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                List<tb_Student> listStudent = db.tb_Student.ToList();
                ViewBag.StudentList = listStudent;
            }
            return View();
        }
        public ActionResult EditStudent(int id)
        {
            tb_Student stu = db.tb_Student.FirstOrDefault(m => m.UserId == id);
            return View("EditStudent", stu);
        }
        [HttpPost]
        public ActionResult EditStudent(tb_Student stu)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    var check = db.tb_Student.FirstOrDefault(m => m.UserId == stu.UserId);
                    if (check != null)
                    {
                        tb_Student oldstu = db.tb_Student.FirstOrDefault(m => m.StudentId == check.StudentId);
                        db.Entry(oldstu).State = EntityState.Deleted;
                        db.Entry(stu).State = EntityState.Added;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Student");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}