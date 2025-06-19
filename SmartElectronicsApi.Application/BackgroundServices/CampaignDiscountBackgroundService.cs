using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.DataAccess.Data.Implementations;

namespace SmartElectronicsApi.Application.BackgroundServices
{
    public class CampaignDiscountBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public CampaignDiscountBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var campaignService = scope.ServiceProvider.GetRequiredService<ICampaignService>();
                        var nonApliedCampaigns = unitOfWork.CampaignRepository.GetNonAppliedCampaigns();
                        foreach (var campaign in nonApliedCampaigns)
                        {
                            if (campaign.DiscountPercentageValue.HasValue)
                            {
                                await campaignService.ApplyDiscountPriceToProduct(campaign.Id);
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(105), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    
                }
                catch (Exception ex)
                {
                    
                }
            }

        }
    }
}
