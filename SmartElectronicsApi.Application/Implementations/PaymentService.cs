using Microsoft.Extensions.Configuration;
using SmartElectronicsApi.Application.Interfaces;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class PaymentService:IPaymentService
    {
        private readonly string _secretKey;

        public PaymentService(IConfiguration configuration)
        {
            _secretKey = configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<Charge> CreateCharge(decimal amount, string currency, string source)
        {
            var options = new ChargeCreateOptions
            {
                Amount = (long)(amount * 100), // Amount in cents
                Currency = currency,
                Source = source,
                Description = "Order Payment",
            };

            var service = new ChargeService();
            Charge charge = await service.CreateAsync(options);
            return charge;
        }
    }
}
