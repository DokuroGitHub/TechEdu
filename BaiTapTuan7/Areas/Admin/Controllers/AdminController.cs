﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BaiTapTuan7.Models;
using BaiTapTuan7.Areas.Admin.Model;
namespace BaiTapTuan7.Areas.Admin.Controllers
{
    //[AuthorizeController]
    public class AdminController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Admin
        public ActionResult Index()
        {
            var tc = (tb_Teacher)Session["admin"];
            ViewBag.newsList = MyNews();
            ViewBag.inCome = CreateListIncomeUsdOfYear(2020);
            string a = "";
            a = a + ViewBag.inCome[0];
            for(int i =1; i< 12; i++)
            {
                a = a + "," +ViewBag.inCome[i];
            }
            ViewBag.inn = a;
            return View(tc);
        }
        public JsonResult AreaChartData()
        {
            Chart _chart = new Chart();
            _chart.labels = new string[] { "Jan", "Feb", "Mar" };
            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "This Year",
                data = CreateListIncomeUsdOfYear(2020),
                backgroundColor = new string[] { "800000", "#E9967C", "#FF0000" },
                borderColor = new string[] { "800000", "#E9967C", "#FF0000" },
                borderWidth = "1"
            });
            _chart.datasets = _dataSet;
            return Json(_chart, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChartRender()
        {
            ViewBag.inCome = CreateListIncomeUsdOfYear(2020);
            string a = "";
            a = a + ViewBag.inCome[0];
            for (int i = 1; i < 12; i++)
            {
                a = a + "," + ViewBag.inCome[i];
            }
            ViewBag.inn = a;
            return View();
        }
        // Phần thông báo của admin
        public ActionResult NewsDetails(int newsid)
        {
            ViewBag.newsList = MyNews();
            var news = db.tb_News.Find(newsid);
            return View("NewsDetails", news);
        }
        public ActionResult AddNews()
        {
            ViewBag.RoleList = GetRoleNews();
            tb_News news = new tb_News();
            return View("AddNews", news);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddNews(tb_News news)
        {
            if (ModelState.IsValid)
            {
                tb_News ne = new tb_News();
                ne.Title = news.Title;
                ne.Details = news.Details;
                ne.CreatedDate = DateTime.Now;
                ne.From = ((tb_Teacher)Session["admin"]).TeacherFirstName + " " + ((tb_Teacher)Session["admin"]).TeacherLastName;
                ne.To = news.To;
                db.Entry(ne).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult EditNews(int newsid)
        {
            ViewBag.RoleList = GetRoleNews();
            tb_News ne = db.tb_News.Find(newsid);
            return View("EditNews", ne);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditNews(tb_News news)
        {
            if (ModelState.IsValid)
            {
                tb_News ne = db.tb_News.Find(news.News_Id);
                ne.Title = news.Title;
                ne.Details = news.Details;
                ne.CreatedDate = DateTime.Now;
                ne.From = ((tb_Teacher)Session["admin"]).TeacherFirstName + " " + ((tb_Teacher)Session["admin"]).TeacherLastName;
                ne.To = news.To;
                db.Entry(ne).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult AccountSettings()
        {
            var us = (tb_Users)Session["user"];
            return PartialView("AccountSettings", us);
        }
        [HttpPost]
        public ActionResult AccountSettings(tb_Users us)
        {
            if (ModelState.IsValid)
            {
                var uss = db.tb_Users.Find(us.Id);
                uss.Password = MD5(us.Password);
                db.Entry(uss).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult DeleteNews(int newsid)
        {
            if (ModelState.IsValid)
            {
                var news = db.tb_News.Find(newsid);
                db.Entry(news).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        // Thông tin cá nhân của admin (admin ví dụ là một teacher)
        public ActionResult Logout()
        {
            ClearCache();
            return RedirectToAction("Index", "Home", new { @area = "" });
        }
        public ActionResult AdminProfile()
        {
            tb_Teacher tc = (tb_Teacher)Session["admin"];
            var tcc = db.tb_Teacher.Find(tc.TeacherId);
            return View("AdminProfile", tcc);
        }
        [HttpPost]
        public ActionResult AdminProfile(tb_Teacher tc)
        {
            if (ModelState.IsValid)
            {
                tb_Teacher oldtc = db.tb_Teacher.FirstOrDefault(m => m.TeacherId == tc.TeacherId);
                if (oldtc != null)
                {
                    oldtc.TeacherFirstName = tc.TeacherFirstName;
                    oldtc.TeacherLastName = tc.TeacherLastName;
                    oldtc.DateOfBirth = tc.DateOfBirth;
                    oldtc.PhoneNumber = tc.PhoneNumber;
                    oldtc.Gmail = tc.Gmail;
                    oldtc.PlaceOfBirth = tc.PlaceOfBirth;
                    HttpPostedFileBase upload = Request.Files["image"];
                    if (upload.FileName != "")
                    {
                        using (var binaryReader = new BinaryReader(upload.InputStream))
                            oldtc.Images = binaryReader.ReadBytes(upload.ContentLength);
                    }
                    db.Entry(oldtc).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("AdminProfile", oldtc);
                }
                else
                {
                    ModelState.AddModelError("", "Edit teacher failed 1");
                    return View("AdminProfile", tc);
                }
            }
            else
            {
                ModelState.AddModelError("", "Edit Student failed 2 ");
                return View("AdminProfile", tc);
            }
        }
        public ActionResult AccountSetting()
        {
            ViewBag.teacher = (tb_Teacher)Session["teacher"];
            tb_Users us = (tb_Users)Session["user"];
            var uss = db.tb_Users.Find(us.Id);
            return View("AccountSetting", uss);
        }
        [HttpPost]
        public ActionResult AccountSetting(tb_Users us)
        {
            if (ModelState.IsValid)
            {
                var uss = db.tb_Users.Find(us.Id);
                if (uss != null)
                {
                    uss.Password = MD5(us.Password);
                    db.Entry(uss).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AccountSetting");
                }
                else
                {
                    ModelState.AddModelError("", "Edit User failed");
                    return RedirectToAction("AccountSetting", us);
                }
            }
            else
            {
                ModelState.AddModelError("", "Edit User failed");
                return RedirectToAction("AccountSetting", us);
            }

        }

        //Charts
        public ActionResult StatisticalOfYear()
        {
            return View();
        }
        public ActionResult StatisticalOfMonth()
        {
            return View();
        }
        public ActionResult StatisticalOfWeek()
        {
            return View();
        }
        public void ClearCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        public int[] CreateListIncomeUsdOfYear(int year, int month = 0)
        {
            int[] incomeMonthList = new int[12];
            if (month == 0)
            {
                for (int i = 1; i < 13; i++)
                {
                    decimal income = IncomeUsdForAllCourseIn1Month(year, i);
                    incomeMonthList[i - 1] = (int)income;
                }
            }
            else
            {
                incomeMonthList = new int[1];
                incomeMonthList[0] = (int)IncomeUsdForAllCourseIn1Month(year, month);
            }
            return incomeMonthList;
        }
        public decimal IncomeUsdForAllCourseIn1Month(int year,int month =0)
        {
            decimal sum = 0;
            var allOfCourse = db.tb_Course.Where(m => m.Course_Price != null || m.Course_Price != 0).ToList();
            foreach (var item in allOfCourse)
            {
                int count = IncomeStudentFor1CourseIn1Month(item.Course_Id, year, month);
                sum = sum + (decimal)(count * item.Course_Price);
            }
            return sum;
        }
        public int IncomeStudentFor1CourseIn1Month(int couid, int year, int month = 0)
        {
            var listOfIncomeStudent = listOfIncomeStudentIn1Month(couid, year, month);
            return listOfIncomeStudent.Count;
        }
        public List<tb_StudentCourse> listOfIncomeStudentIn1Month(int couid, int year, int month = 0)
        {
            if (month != 0)
            {
                return db.tb_StudentCourse.Where(m => m.CourseId == couid && ((DateTime)m.EnrollDate).Month == month && ((DateTime)m.EnrollDate).Year == year).ToList();
            }
            else
                return db.tb_StudentCourse.Where(m => m.CourseId == couid && ((DateTime)m.EnrollDate).Year == year).ToList();

        }
        public List<SelectListItem> GetRoleNews()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "1", Text = "All" });
            list.Add(new SelectListItem { Value = "2", Text = "Admin" });
            list.Add(new SelectListItem { Value = "3", Text = "Teacher" });
            list.Add(new SelectListItem { Value = "4", Text = "Student" });
            return list;
        }

        public List<tb_News> MyNews()
        {
            var newsLists = db.tb_News.ToList();
            return newsLists;
        }
        private static string MD5(string Metin)
        {
            MD5CryptoServiceProvider MD5Code = new MD5CryptoServiceProvider();
            byte[] byteDizisi = Encoding.UTF8.GetBytes(Metin);
            byteDizisi = MD5Code.ComputeHash(byteDizisi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in byteDizisi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }
    }
}