﻿using SmartElectronicsApi.Application.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Auth
{
    public class UserGetDto
    {
        public string FullName { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Image {  get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? CreatedTime { get; set; }
        public int loyalPoints { get; set; }
        public int loyaltyTier { get; set; }
        public ICollection<OrderListItemDto>? orders { get; set; }
        public int OrdersCount { get; set; }
        public int WishListedItemsCount { get; set; }
        public int TotalAmountSum { get; set; }
        public List<string> FavoriteCategories { get; set; }
    }
}
