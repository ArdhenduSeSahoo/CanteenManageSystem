using CanteenManage.Models;
using CanteenManage.Services;
using CanteenManage.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using System.Net.Http;

namespace CanteenManage.Controllers
{
    [Authorize(Roles = "Employee")]
    public class DashboardController : Controller
    {
        private readonly FoodListingService foodListingService;
        private readonly UtilityServices utilityServices;
        private readonly ILogger<DashboardController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppConfigProvider _appConfigProvider;


        public DashboardController(FoodListingService foodListing, UtilityServices utilityServices, IHttpClientFactory httpClientFactory, ILogger<DashboardController> logger, AppConfigProvider appConfigProvider, IHttpClientFactory httpClientFactory1)
        {
            foodListingService = foodListing;
            this.utilityServices = utilityServices;
            _httpClientFactory = httpClientFactory1;
            _appConfigProvider = appConfigProvider;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            EmployeeDashboardViewDataModel employeeDashboardViewDataModel = new EmployeeDashboardViewDataModel();

            try
            {
                SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
                var breakfastFoods = await foodListingService.GetTodayFoodNames((int)FoodTypeEnum.Breakfast, cancellationToken);
                var lunchFoods = await foodListingService.GetTodayFoodNames((int)FoodTypeEnum.Lunch, cancellationToken);
                var snacksFoods = await foodListingService.GetTodayFoodNames((int)FoodTypeEnum.Snacks, cancellationToken);
                var dinnerFoods = await foodListingService.GetTodayFoodNames((int)FoodTypeEnum.Dinner, cancellationToken);
                var myorderBreakfast = await foodListingService.GetMyOrderTodayFoodNames((int)FoodTypeEnum.Breakfast, sessionDataModel.UserIdOrZero, cancellationToken);
                var myorderLunch = await foodListingService.GetMyOrderTodayFoodNames((int)FoodTypeEnum.Lunch, sessionDataModel.UserIdOrZero, cancellationToken);
                var myorderSnacks = await foodListingService.GetMyOrderTodayFoodNames((int)FoodTypeEnum.Snacks, sessionDataModel.UserIdOrZero, cancellationToken);
                var myorderDinner = await foodListingService.GetMyOrderTodayFoodNames((int)FoodTypeEnum.Dinner, sessionDataModel.UserIdOrZero, cancellationToken);


                employeeDashboardViewDataModel.UserName = sessionDataModel.UserName;
                employeeDashboardViewDataModel.UserId = sessionDataModel.UserId;
                employeeDashboardViewDataModel.CartItemCount = await foodListingService.GetCartItemCount(sessionDataModel.UserIdOrZero, cancellationToken);

                string responsbody = "";
                string? EConnect_token = HttpContext.Session.GetString(SessionConstants.EconnectToken);
                //EConnect_token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJ6d28zLWgzV1pySms1cG5Ga1ZlWGtmSnhnQlRaZkNMelVXT2l3bXBDX1hjIn0.eyJleHAiOjE3NDk3NDIzMDQsImlhdCI6MTc0OTczMTUwNCwianRpIjoiYWU1NDMxYTEtNWY2NS00ZjQ3LWEzYTEtYjg5MThhNDM0ZmU4IiwiaXNzIjoiaHR0cDovLzE5Mi4xNjguNi42OjgwODAvYXV0aC9yZWFsbXMvSGlyZUh1YiIsInN1YiI6IjM1MzJjZDk2LTNhMDEtNGM0Mi05ZTVjLWVlZWRiMTllNmI4YiIsInR5cCI6IkJlYXJlciIsImF6cCI6ImFkbWluLWNsaSIsInNlc3Npb25fc3RhdGUiOiJkNzI0YjgzZi0wMDEwLTQyYTgtOTQ2Yi03NTA0ZmU4ZWYzYzIiLCJhY3IiOiIxIiwic2NvcGUiOiJvcGVuaWQgZW1haWwgZGVzY3JpcHRpb24gcHJvZmlsZSIsInNpZCI6ImQ3MjRiODNmLTAwMTAtNDJhOC05NDZiLTc1MDRmZThlZjNjMiIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwibmFtZSI6IkFyZGhlbmR1IFNla2hhciBTYWhvbyIsImRlc2NyaXB0aW9uIjoiU0QxMjY1IiwicHJlZmVycmVkX3VzZXJuYW1lIjoiYXJkaGVuZHUgc2VraGFyIHNhaG9vIiwiZ2l2ZW5fbmFtZSI6IkFyZGhlbmR1IiwiZmFtaWx5X25hbWUiOiJTZWtoYXIgU2Fob28iLCJlbWFpbCI6ImFyZGhlbmR1QGVzc3BsLmNvbSJ9.L7Dzw3cJhnwuh22aljrEK9PYtq-K29wJkCHehClmC18EvajCgVqxFHnoj5GTFQG8hwyfngVMVHP0yugyb_uHSTMp8DEdabsfcaFoZ_tLnqsPJpQ93NbFzW09HbzS462D3k1Quiv4Ej-o_xEpoAzf7zIizpuXR_ip6AjShz_qYPrgApVSLUfUPg58l-4V7lkdeb_aZPxdytXHLsmjiBiSPOFPLgWQTMNNWnZNlzWnkoiEWuaNsvlz-olMM6V4u7R12RHyAqFOfOc86e7HcaH5x3HbL9l-X3xtiVae9iE-W6h1FIhqy6R0XjLe44ydlk7QUg1MsadJgeD3rAsNzM8C6w";
                //var httpClient = _httpClientFactory.CreateClient(CustomDataConstants.PortalAuthValidater);
                try
                {

                    //httpClient.Timeout = new TimeSpan(0, 0, 50);
                    //httpClient.DefaultRequestHeaders.Clear();
                    //httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, "Bearer " + EConnect_token);
                    //var responsdata = await httpClient.PatchAsync(_appConfigProvider.GetPortalAuthValidaTorEndpoint(), null);
                    //_logger.LogError($"Calling e connect auth path--{_appConfigProvider.GetPortalAuthValidaTorEndpoint()} token-{EConnect_token}");
                    //responsdata.EnsureSuccessStatusCode();
                    //responsbody = await responsdata.Content.ReadAsStringAsync();
                    //_logger.LogError(responsbody);
                    //responsbody = "AuthURL--" + httpClient.BaseAddress + _appConfigProvider.GetPortalAuthValidaTorEndpoint() + "----- Response-----" + responsbody;
                    //context.Request.Headers["Authorization"] = "Bearer " + App_token;
                    //await _next(context);
                    //Console.WriteLine(responsdata.ToString());
                }
                catch (Exception ex)
                {
                    //_logger.LogError($"Token validation failed: {ex.Message}---- for user id--{sessionDataModel.UserEmpIdOrNull}-----E connect token-----{EConnect_token}");
                    //context.Response.Redirect("/Error");
                    //return;
                    //responsbody = "AuthURL--" + httpClient.BaseAddress + _appConfigProvider.GetPortalAuthValidaTorEndpoint() + "----- Response-----" + ex.Message + "--" + ex.StackTrace;
                }
                responsbody += "--Userid--" + sessionDataModel.UserEmpIdOrNull;


                employeeDashboardViewDataModel.BreakfastFoods = string.Join(", ", breakfastFoods);//
                employeeDashboardViewDataModel.LunchFoods = string.Join(", ", lunchFoods);
                employeeDashboardViewDataModel.SnacksFoods = string.Join(", ", snacksFoods);//snacksFoods
                employeeDashboardViewDataModel.DinnerFoods = string.Join(", ", dinnerFoods);

                employeeDashboardViewDataModel.MyOrderBreakfastFoods = string.Join(", ", myorderBreakfast);//
                employeeDashboardViewDataModel.MyOrderLunchFoods = string.Join(", ", myorderLunch);
                employeeDashboardViewDataModel.MyOrderSnacksFoods = string.Join(", ", myorderSnacks);
                employeeDashboardViewDataModel.MyOrderDinnerFoods = string.Join(", ", myorderDinner);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DashboardControler Index " + ex.Message);
            }
            return View(employeeDashboardViewDataModel);
        }
        public async Task<IActionResult> QuickFood(CancellationToken cancellationToken)
        {
            SessionDataModel sessionDataModel = utilityServices.GetSessionDataModel(HttpContext.Session);
            var data = await foodListingService.GetquickfoodsAsync(cancellationToken);
            EmployeeDashboardViewDataModel employeeDashboardViewDataModel = new EmployeeDashboardViewDataModel();
            employeeDashboardViewDataModel.Foods = data;
            employeeDashboardViewDataModel.CartItemCount = await foodListingService.GetCartItemCount(sessionDataModel.UserIdOrZero, cancellationToken);
            return View(employeeDashboardViewDataModel);
        }
    }
}
