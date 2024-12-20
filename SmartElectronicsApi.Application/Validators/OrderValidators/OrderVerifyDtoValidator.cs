using FluentValidation;
using SmartElectronicsApi.Application.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Validators.OrderValidators
{
    public class OrderVerifyDtoValidator:AbstractValidator<OrderVerifyDto>
    {
        public OrderVerifyDtoValidator()
        {
            RuleFor(s => s.UserName).NotEmpty();
            RuleFor(s => s.ShippedToken).NotEmpty().MaximumLength(6);
            RuleFor(s => s.Id).NotNull();
        }
    }
}
