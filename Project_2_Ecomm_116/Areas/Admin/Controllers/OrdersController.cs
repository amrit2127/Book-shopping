using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_2_Ecomm_116.DataAccess.Data;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.DataAccess.Repository;
using System.Security.Claims;
using Stripe.Issuing;
using Project_2_Ecomm_116.Utility;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Project_2_Ecomm_116.Models;
using Stripe;
using System.Runtime.Intrinsics.X86;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public OrdersController(IUnitOfWork unitOfWork, ApplicationDbContext context,
            IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIs

        [HttpGet]
        public IActionResult GetAll()
        {
            var orderList = _unitOfWork.OrderHeader.GetAll();
            return Json(new { data = orderList });          
        }

        [HttpDelete]
        public IActionResult DeleteOrder(int id)
        {
            var order = _unitOfWork.OrderHeader.Get(id);
            if (order == null)
                return Json(new { success = false, message = "Something went wrong while Deleting Order!!!" });
            _unitOfWork.OrderHeader.Remove(order);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Order deleted Successfully !!!" });
        }

        public IActionResult ViewDetail(int id, int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.FirstOrDefault(e => e.Id == id);
            //var orderToDuplicate = _unitOfWork.OrderHeader.FirstOrDefault(o => orderId == o.Id);

            return View(orderHeader);
        }

        public async Task<IActionResult> CancelOrder(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim!=null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }

            var orders = _unitOfWork.OrderHeader.Get(id);
            if (orders == null) return NotFound();
            //order cancel
            orders.OrderStatus = SD.OrderStatusCancelled;





            orders.PaymentStatus = SD.PaymentStatusRefunded;
           _unitOfWork.Save();


            //To remove the cancelled order from table

            //_unitOfWork.OrderHeader.Remove(orders);

            //twilio message
            var accountSid = "ACf114e87e0628eb69865bb734b933d9de";
            var authToken = "e2de8ba24e37c5ec5790ca3ef53c4f0f";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber("+917717649716"));
            messageOptions.From = new PhoneNumber("+14699957439");
            messageOptions.Body = "Your order has been cancelled successfully.";

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);


            //email verification and message of cancel order
            try
            {
                string smtpServer = "smtp-mail.outlook.com";
                int smtpPort = 587;
                string smtpUsername = "amrit2702@outlook.com";
                string smtpPassword = "amrit@2702";

                //create message on email
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(smtpUsername);
                mailMessage.To.Add("amritjyotkaur2702@gmail.com");
                mailMessage.Subject = "Order Confirmation";

                mailMessage.Body = "Your order has been successfully Cancelled";

                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);
                smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtp.EnableSsl = true;

                smtp.Send(mailMessage);

                ViewBag.Message = "Email sent successfully!";
            }

            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
            }         

            return View(id);
        }       

        #endregion
    }

    }

