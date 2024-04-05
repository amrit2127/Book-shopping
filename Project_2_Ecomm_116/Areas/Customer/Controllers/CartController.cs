using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using Project_2_Ecomm_116.DataAccess.Repository.IRepository;
using Project_2_Ecomm_116.Models;
using Project_2_Ecomm_116.Models.ViewModels;
using Project_2_Ecomm_116.Utility;
using Stripe;
using System.Security.Claims;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Net.Mail;
using System.Net;
using Stripe.FinancialConnections;

namespace Project_2_Ecomm_116.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool IsEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                    //AddressList = _unitOfWork.StreetAddress.GetAll().Select(cl => new SelectListItem()
                    //{
                    //    Text = cl.Address,
                    //    Value = cl.Id.ToString()
                    //})
                };
                return View(ShoppingCartVM);
            }
            //***
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.
                FirstOrDefault(au => au.Id == claim.Value);

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            } 

            //Email
            if (!IsEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been sent Kindly verify your email!!";
                ViewBag.EmailCSS = "text-success";
                IsEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email must be confirm for Authorize customer!!";
                ViewBag.EmailCSS = "text-danger";
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claim.Value);
            if (user == null)
                ModelState.AddModelError(string.Empty, "Email is Empty!!");
            else
            {
                //Email
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                //***

                IsEmailConfirm = true;
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Plus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.Id == id);
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.Id == id);
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count -= 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefault(sc => sc.Id == id);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            //Session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            //***
            return RedirectToAction(nameof(Index)); 
        }

        public IActionResult Summary(string ids)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //ShoppingCartVM = new ShoppingCartVM()
            //{
            //    ListCart=_unitOfWork.ShoppingCart.GetAll(sc=>sc.ApplicationUserId==claim.Value,includeProperties:"Product"),
            //    OrderHeader= new OrderHeader()
            //};

            if (ids == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = _unitOfWork.ShoppingCart.GetAll(Sc => Sc.ApplicationUserId == claim.Value, includeProperties: "Product"),
                    OrderHeader = new OrderHeader()
                };
            }

            else
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = _unitOfWork.ShoppingCart.GetAll(Sc => ids.Contains(Sc.Id.ToString()), includeProperties: "Product"),
                    OrderHeader = new OrderHeader()
                };
            }

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claim.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken, string ids)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault
                (au => au.Id == claim.Value);
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.
                GetAll(sc => sc.ApplicationUserId == claim.Value,
                includeProperties: "Product").Where(x => ids.Contains(x.Id.ToString()));

            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    ProductId = list.ProductId,
                    Price = list.Price,
                    Count = list.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();
            //Session
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);


            #region Stripe

            if (stripeToken == null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Today.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            {
                //Payment Process
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "OrderId: " + ShoppingCartVM.OrderHeader.Id,
                    Source = stripeToken
                };

                //Payment
                var service = new ChargeService();
                Charge charge = service.Create(options);

                //***intent id
                //var options1 = new PaymentIntentCreateOptions
                //{
                //    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                //    Currency = "USD",
                //    PaymentMethodTypes = new List<string> { "card" },
                //};

                //var serviceintent = new PaymentIntentService();
                //var paymentIntent = service.Create(options);





                if (charge.BalanceTransactionId == null)
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
                else
                {
                    ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
                }
                _unitOfWork.Save();
            }
            #endregion

            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            var accountSid = "ACf114e87e0628eb69865bb734b933d9de";
            var authToken = "e2de8ba24e37c5ec5790ca3ef53c4f0f";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber("+917717649716"));
            messageOptions.From = new PhoneNumber("+14699957439");
            messageOptions.Body = "Your order has been successfully Placed.";

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);

            //email verification and message
            try
            {
                string smtpServer = "smtp-mail.outlook.com";
                int smtpPort = 587;
                string smtpUsername = "amrit2702@outlook.com";
                string smtpPassword = "amrit@2702";

                //create message on email
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(smtpUsername);
                mailMessage.To.Add("amritjyot2001@gmail.com");
                mailMessage.Subject = "Order Confirmation";

                mailMessage.Body = "Your order has been successfully Placed";

                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort);              
                smtp.Credentials = new NetworkCredential(smtpUsername,smtpPassword);
                smtp.EnableSsl = true;

                smtp.Send(mailMessage);

                ViewBag.Message = "Email sent successfully!";
            }

            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);

            return View(id);
        }

    }
}
