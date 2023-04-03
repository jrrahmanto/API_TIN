using API_TIN_KBI.Models;
using API_TIN_KBI.Models.request;
using API_TIN_KBI.Models.response;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static API_TIN_KBI.Models.response.Auth;

namespace API_TIN_KBI.Controllers
{
    [Route("[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : Controller
    {
        private readonly dbcontext _context;
        public AuthController(dbcontext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult GetAllTodos([FromBody] Models.request.Auth listexref, [FromHeader] string apikey, [FromHeader] string userid, [FromHeader] string timestamp, [FromHeader] string signature)
        {
            try
            {
                var useractive = _context.api_acess.Where(x => x.userid == userid && x.apikey == apikey && x.actived == 1).FirstOrDefault();
                if (useractive != null)
                {
                    List<Models.response.Auth.success> success = new List<Models.response.Auth.success>();
                    var sign = GenerateSHA256("POST:" + userid + ":" + apikey + ":" + timestamp + "");
                    if (sign == signature)
                    {
                        foreach (var item in listexref.listexref)
                        {
                            var rawtradefeed = _context.RawTradeFeed.Where(x => x.ExchangeRef == item).ToList();
                            if (rawtradefeed.Count != 0)
                            {
                                Double qty = 0;
                                string seller = "";
                                string buyer = "";
                                List<detaillogams> detaillogam = new List<detaillogams>();
                                foreach (var rt in rawtradefeed)
                                {
                                    seller = _context.ClearingMember.Where(x => x.code == rt.SellerInvCode.Substring(0, 4)).FirstOrDefault().Name;
                                    buyer = _context.ClearingMember.Where(x => x.code == rt.BuyerInvCode.Substring(0, 4)).FirstOrDefault().Name;
                                    var nobst = rt.SellerRef.Split("_");
                                    var coa = _context.StagingSellerAllocation.Where(x => x.nobst == nobst[0] && x.businessdate == rt.BusinessDate.Date).FirstOrDefault();
                                    qty += Convert.ToDouble(rt.Qty) * (Convert.ToDouble(coa.volume) / 1000);
                                    detaillogam.Add(new detaillogams
                                    {
                                        noBst = nobst[0],
                                        port = coa.location,
                                        product = coa.productid,
                                        brand = coa.brand,
                                        tonase = (Convert.ToDouble(coa.volume) / 1000).ToString(),
                                        price = rt.Price.ToString(),
                                        total = ((Convert.ToDouble(coa.volume) / 1000) * Convert.ToDouble(rt.Price)).ToString()
                                    });
                                }
                                success.Add(new Models.response.Auth.success
                                {
                                    ExchangeRef = item,
                                    BusinessDate = rawtradefeed[0].BusinessDate.ToShortDateString(),
                                    TradeTime = rawtradefeed[0].TradeTime.ToString("HH:mm:ss"),
                                    TotalTonase = (qty).ToString(),
                                    TotalTransaction = (qty * Convert.ToDouble(rawtradefeed[0].Price)).ToString(),
                                    SellerName = seller,
                                    BuyerName = buyer,
                                    detaillogam = detaillogam,
                                    message = "Success retreive data"
                                });
                            }
                        }
                    }
                    else
                    {
                        success.Add(new Models.response.Auth.success
                        {
                            message = "Invalid signature !"
                        });
                    }
                    var log = new api_log();
                    log.userid = userid;
                    log.signature = signature;
                    log.timestamp = timestamp;
                    log.createddate = DateTime.Now;
                    log.request = string.Join(" ", listexref.listexref);
                    log.respons = string.Join(" ", success[0].message);
                    _context.Add(log);
                    _context.SaveChangesAsync();
                    return Json(success);
                }
                else
                {
                    List<Models.response.Auth.error> error = new List<Models.response.Auth.error>();
                    var log = new api_log();
                    log.userid = userid;
                    log.signature = signature;
                    log.timestamp = timestamp;
                    log.createddate = DateTime.Now;
                    log.request = string.Join(" ", listexref.listexref);
                    log.respons = "Error ! Please check userid or apikey !";
                    _context.Add(log);
                    _context.SaveChangesAsync();
                    error.Add(new Models.response.Auth.error
                    {
                        message = "Error ! Please check userid or apikey !"
                    });
                    return Json(error);
                }

            }
            catch (Exception x)
            {
                List<Models.response.Auth.error> error = new List<Models.response.Auth.error>();
                var log = new api_log();
                log.userid = userid;
                log.signature = signature;
                log.timestamp = timestamp;
                log.createddate = DateTime.Now;
                log.request = string.Join(" ", listexref.listexref);
                log.respons = x.Message;
                _context.Add(log);
                _context.SaveChangesAsync();
                error.Add(new Models.response.Auth.error
                {
                    message = x.Message
                });
                return Json(error);
            }
        }

        static string GenerateSHA256(string plaintext)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(plaintext));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
    }
}
