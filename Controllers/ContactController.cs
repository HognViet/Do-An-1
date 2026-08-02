using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Mvc;

namespace San_Pham_Do_An1.Controllers
{
    public class ContactController : Controller
    {
        
            private readonly WedQuanAoDbContext _context;

            public ContactController(WedQuanAoDbContext context)
            {
                _context = context;
            }
            public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Send(string name, string phone, string email, string message)
        {
            if (ModelState.IsValid)
            {
             
                TbContact contact = new TbContact();

               
                contact.Name = name;
                contact.Phone = phone; 
                contact.Email = email;
                contact.Message = message;

               
                contact.IsRead = false;          
                contact.CreatedDate = DateTime.Now; 
                contact.CreatedBy = "Khách hàng";   
                contact.ModifiedDate = DateTime.Now;
                contact.ModifiedBy = "System";

           
                _context.Add(contact);
                _context.SaveChanges();

              
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}
