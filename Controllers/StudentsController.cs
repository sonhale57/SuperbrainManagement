using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Google.Protobuf.WellKnownTypes;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class StudentsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Students
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.Branch).Include(s => s.MKTCampaign);
            return View(students.ToList());
        }
       
        public ActionResult AddCourseProgramOfStudents(int IdStudent)
        {
            Student student = Connect.SelectSingle<Student>("select * from Student");
            Session["infoUser"] = student;
            return View(student);  
        }
        [HttpPost] // Use POST for actions that modify data
        public ActionResult Deletes(int IdCourse, int IdRegistration)
        {
            try
            {
                var deleteItem = db.RegistrationCourses.FirstOrDefault(x => x.IdCourse == IdCourse && x.IdRegistration == IdRegistration);
                if (deleteItem != null)
                {
                    db.RegistrationCourses.Remove(deleteItem);
                    db.SaveChanges();
                    return Json(deleteItem.IdRegistration);

                }
                else
                {
                    return Json(new { success = false, message = "Item not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Item not found" });
            }
        }
        public string GenerateCode()
        {
            string prefix = "DK_HQ";

            // Generate a random number
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999); // Adjust range as needed

            // Generate the new code
            string newCode = $"{prefix}_{randomNumber}";

            return newCode;
        }
        public class cartitem
        {
            public int idProgram { get; set; }
            public int idCourse { get; set; }
            public int Idpromotion { get; set; }
            public int price { get; set; }
            public string nameprogram { get; set; }
            public string IdRegistrations { get; set; }

            public string DateTime { get; set; }
            public int Id { get; set; }
            public int total { get; set; }

        }
        public ActionResult getData(string IdRegistration)
        {
            DataTable dataTable = Connect.SelectAll("select cour.Name as NameCourse,rescourse.IdCourse,res.Id,rescourse.Price,pro.Name as NameProgram,res.Amount,rescourse.TotalAmount,res.Code,res.DateCreate,res.Discount  from Registration res \r\ninner join RegistrationCourse rescourse on rescourse.IdRegistration = res.Id\r\ninner join Course cour on cour.Id = rescourse.IdCourse\r\ninner join Program pro on pro.Id = cour.IdProgram where res.Id = '" + IdRegistration + "'");
            Registration registration = Connect.SelectSingle<Registration>("select * from Registration where Id = '" + IdRegistration + "'");
            // Khởi tạo danh sách HTML string để lưu dữ liệu
            var data = new StringBuilder();
            var totalamount = 0;
            var i = 0;
            var datacreate = "";
            var idRegistration = 0;
            var code = "";
            foreach (DataRow row in dataTable.Rows)
            {
                i++;
                string amountString = string.Format("{0:N0} VND", row["TotalAmount"]);
                string discount = string.Format("{0:N0} VND", row["Discount"]);
                string nameProgramCourse = row["NameProgram"].ToString() + "<hr>" + row["NameCourse"].ToString();
                // Tạo dòng HTML cho mỗi dòng dữ liệu
                var newRow = "<tr>" +
                    "<td style='text-align:center;'>" + i + "</td>" +
                    "<td style='text-align:left;'>" + nameProgramCourse + "</td>" +
                    "<td style='text-align:center;'>1</td>" +
                    "<td style='text-align:right;'>" + amountString + "</td>" +
                    "<td style='text-align:center;'>"+discount+"</td>" +
                    "<td style='text-align:right;'>" + amountString + "</td>" +
                    "<td style='text-align:center;'>" +
                    "<a href='#' class='btn btn-sm btn-danger ti-trash' onclick=\"deleteitem('" + row["IdCourse"] + "','" + row["Id"] + "')\" data-courseid='" + row["IdCourse"] + "' data-registrationid='" + row["Id"] + "'>" +
                    "<i class='bx bx-trash-alt font-size-18'></i>" +
                    "</a>" +
                    "</td>" +
                    "</tr>";

                // Thêm dòng vào danh sách HTML
                data.Append(newRow);

                // Lấy ngày tạo

                // Tính tổng số tiền

                totalamount += Convert.ToInt32(row["TotalAmount"]);
            }

            // Tạo đối tượng kết quả
            var result = new
            {
                datalist = data.ToString(), // Chuyển đổi danh sách HTML thành chuỗi
                TotalAmount = totalamount,
                DateCreate = Convert.ToDateTime(registration.DateCreate).ToString("dd/MM/yyyy"),
                idRegistrations = registration.Id,
                Code = registration.Code
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveRegistration(int idProgram, int IdCourse, int Idpromotion, int price, string nameprogram, string code,string description,int discount)
        {
            MD5Hash md5 = new MD5Hash();
            string iduser = System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString();
            iduser = md5.Decrypt(iduser.ToString());
            Student student = Session["infoUser"] as Student;
            Registration registrations = Connect.SelectSingle<Registration>("select * from Registration where Id='" + code + "'");
            List<cartitem> cartitems = new List<cartitem>();
            if (registrations != null)
            {
                RegistrationCourse registrationCourse = Connect.SelectSingle<RegistrationCourse>("select * from RegistrationCourse where IdRegistration = '" + registrations.Id + "' and IdCourse = '" + IdCourse + "'");
                if (registrationCourse != null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    RegistrationCourse courseres = new RegistrationCourse();
                    courseres.IdCourse = IdCourse;
                    courseres.IdRegistration = registrations.Id;
                    courseres.Price = price;
                    courseres.TotalAmount = price;
                    courseres.Amount = price;
                    courseres.Status = true;
                    db.RegistrationCourses.Add(courseres);
                    db.SaveChanges();
                    Course course = db.Courses.Where(x => x.Id == IdCourse).FirstOrDefault();
                    Program program = db.Programs.Where(x => x.Id == course.IdProgram).FirstOrDefault();
                    cartitem cartitem = new cartitem();
                    cartitem.idProgram = idProgram;
                    cartitem.idCourse = IdCourse;
                    cartitem.price = Convert.ToInt32(courseres.Price);
                    cartitem.Id = registrations.Id;
                    cartitem.nameprogram = program.Name;
                    cartitem.IdRegistrations = registrations.Code;
                    cartitem.DateTime = DateTime.Now.ToString("dd/MM/yyyy");
                    List<RegistrationCourse> registrationtotal = Connect.Select<RegistrationCourse>("select * from RegistrationCourse where IdRegistration = '" + registrations.Id + "'");
                    cartitem.total = Convert.ToInt32(registrationtotal.Sum(x => x.Amount));

                    cartitems.Add(cartitem);

                    int total = cartitems.Sum(item => item.price);
                    Console.WriteLine(cartitems);
                    return Json(registrations.Id, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                Registration registration = new Registration();
                registration.IdBranch = student.IdBranch;
                registration.IdStudent = student.Id;
                registration.IdUser = Convert.ToInt32(iduser);
                registration.Amount = price;
                registration.TotalAmount = price;
                registration.Discount = discount;
                registration.Code = GenerateCode();
                registration.DateCreate = DateTime.Now;
                registration.Description = description;
                db.Registrations.Add(registration);
                db.SaveChanges();
                if (registration.Id != null)
                {
                    RegistrationCourse registrationCourse = new RegistrationCourse();
                    registrationCourse.IdCourse = IdCourse;
                    registrationCourse.IdRegistration = registration.Id;
                    registrationCourse.Amount = price;
                    registrationCourse.TotalAmount = price;
                    registrationCourse.Discount = price;
                    registrationCourse.DateExtend = DateTime.Now;
                    db.RegistrationCourses.Add(registrationCourse);
                    db.SaveChanges();
                }
                Course course = db.Courses.Where(x => x.Id == IdCourse).FirstOrDefault();
                Program program = db.Programs.Where(x => x.Id == course.IdProgram).FirstOrDefault();
                cartitem cartitem = new cartitem();
                cartitem.idProgram = idProgram;
                cartitem.idCourse = IdCourse;
                cartitem.price = price;

                cartitem.nameprogram = program.Name;
                cartitem.IdRegistrations = GenerateCode();
                cartitem.DateTime = DateTime.Now.ToString("dd/MM/yyyy");
                cartitem.Id = registration.Id;
                List<RegistrationCourse> registrationtotal = Connect.Select<RegistrationCourse>("select * from RegistrationCourse where IdRegistration = '" + registration.Id + "'");
                cartitem.total = Convert.ToInt32(registrationtotal.Sum(x => x.Amount));
                cartitems.Add(cartitem);

                int total = cartitems.Sum(item => item.price);
                Console.WriteLine(cartitems);
                return Json(registration.Id, JsonRequestBehavior.AllowGet);
            }
          
        }
        public ActionResult GetDataCombobox(int? IdProgram,int? IdCourse,int? type)
        {
            var item = new
            {
                courses = new List<Course>(),
                programs = new List<Program>()
            };

            // Lấy danh sách tất cả các chương trình
            List<Program> programs = Connect.Select<Program>("SELECT * FROM Program");
            List<Promotion> promotions = Connect.Select<Promotion>("Select * from Promotion");
            var priceCourse = 0;
            // Nếu không có IdProgram được chọn, lấy danh sách khóa học của chương trình đầu tiên
            if (IdProgram == 0 && type == 1)
            {
                // Lấy danh sách khóa học của chương trình đầu tiên
               
             
                    if (programs.Count > 0)
                    {
                        List<Course> courses = Connect.Select<Course>("SELECT * FROM Course WHERE IdProgram = '" + programs[0].Id + "'");

                        priceCourse = Convert.ToInt32(courses[0].Price);
                        item.programs.AddRange(programs);
                        item.courses.AddRange(courses);
                    }
                
            }
            else if(IdProgram != 0 && type == 1)
            {
           
                    // Nếu có IdProgram được chọn, lấy danh sách khóa học của chương trình tương ứng
                    List<Course> courses = Connect.Select<Course>("SELECT * FROM Course WHERE IdProgram = '" + IdProgram + "'");
                    priceCourse = Convert.ToInt32(courses[0].Price);
                    item.courses.AddRange(courses);
                
            
            }else if (type == 2) 
            {
                if (IdCourse != 0)
                {
                    Course course = Connect.SelectSingle<Course>("select * from Course where Id = '" + IdCourse + "'");
                    priceCourse = Convert.ToInt32(course.Price);
                }
            }
          
            // Tạo chuỗi HTML cho các dropdown chương trình
            var strpro = "";
            if (item.programs.Count > 0)
            {
                foreach (var pro in item.programs)
                {
                    strpro += "<option value='" + pro.Id + "' data-name='" + pro.Name + "'>" + pro.Name + "</option>";
                }
            }
            var strcour = "";
            foreach (var course in item.courses)
            {
                strcour += "<option value='" + course.Id + "' data-name='" + course.Name + "'>" + course.Name + "</option>";
            }
            var strpromotion = "";
            foreach(var promotion in promotions)
            {
                strpromotion += "<option value='" + promotion.Id + "' data-name='" + promotion.Name + "'>" + promotion.Name + "</option>";
            }
            // Tạo đối tượng để chứa chuỗi HTML của dropdown chương trình và khóa học
            var strcombobox = new
            {
                strpro,
                strcour,
                strpromotion,
                priceCourse =  priceCourse,
                type = type
                // Đây là để giữ chỗ, bạn có thể thêm logic để tạo chuỗi HTML cho dropdown khóa học tương ứng
            };

            return Json(strcombobox, JsonRequestBehavior.AllowGet);
        }



        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo");
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
