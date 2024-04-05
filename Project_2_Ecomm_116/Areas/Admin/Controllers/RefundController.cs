using Microsoft.AspNetCore.Mvc;
using static NuGet.Packaging.PackagingConstants;
using Stripe;

namespace Project_2_Ecomm_116.Areas.Admin.Controllers
{
    public class RefundController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
       // [AutoValidateAntiforgeryToken]
        //public async Task<IActionResult> Refund(int id)
        //{

        //    var order = _unitOfWork.OrderHeader.FirstOrDefault(o => o.Id == id);

        //    if (order != null && !string.IsNullOrEmpty(order.TransactionId))
        //    {
        //        try
        //        {

        //            var refundService = new RefundService();
        //            var refundOptions = new RefundCreateOptions
        //            {
        //                Charge = order.TransactionId,
        //            };
        //            var refund = await refundService.CreateAsync(refundOptions);


        //            if (refund.Status == "succeeded")
        //            {
        //                order.PaymentStatus = SD.PaymentStatusRefunded;
        //                order.OrderStatus = SD.OrderStatusRefunded;
        //                _unitOfWork.save();
        //            }


        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (StripeException ex)
        //        {

        //            ModelState.AddModelError(string.Empty, ex.Message);
        //        }
        //    }


        //    return RedirectToPage("/RefundError");
        //}


        //payment refund
//            try
//            {

//                var refundService = new RefundService();
//        var refundOptions = new RefundCreateOptions
//        {
//            Charge = orders.TransactionId,
//        };
//        var refund = await refundService.CreateAsync(refundOptions);


//                if (refund.Status == "succeeded")
//                {
//                    orders.PaymentStatus = SD.PaymentStatusRefunded;
//                    orders.OrderStatus = SD.OrderStatusRefunded;
//                    _unitOfWork.Save();
//                }

//                return RedirectToAction(nameof(Index));
//}
//            catch (StripeException ex)
//            {
//                // Handle Stripe-related errors
//                return BadRequest(new { Message = $"Error refunding payment: {ex.Message}" });
//            }







    }
}
