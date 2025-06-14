﻿using CanteenManage.CanteenRepository.Models;
using CanteenManage.Utility;

namespace CanteenManage.Models
{
    public class MyOrderListPanelViewModel
    {
        public string PanelTitle { get; set; }
        public List<FoodOrder> FoodOrders { get; set; }
        //public List<EmployeeOrderDetail> FoodOrders { get; set; }
        public FoodTypeEnum FoodType { get; set; }
        public bool ShowAllOrder { get; set; }
    }
}
