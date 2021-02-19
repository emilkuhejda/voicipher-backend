using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/purchases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class BillingPurchaseController : ControllerBase
    {
        private readonly Lazy<IBillingPurchaseRepository> _billingPurchaseRepository;

        public BillingPurchaseController(Lazy<IBillingPurchaseRepository> billingPurchaseRepository)
        {
            _billingPurchaseRepository = billingPurchaseRepository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll(Guid userId, CancellationToken cancellationToken)
        {
            var billingPurchases = await _billingPurchaseRepository.Value.GetAllAsync(userId, cancellationToken);
            var outputModels = billingPurchases.Select(MapBillingPurchase).ToArray();

            return Ok(outputModels);
        }

        [HttpGet("detail/{purchaseId}")]
        public async Task<IActionResult> Get(Guid purchaseId, CancellationToken cancellationToken)
        {
            var billingPurchase = await _billingPurchaseRepository.Value.GetAsync(purchaseId, cancellationToken);
            var outputModel = MapBillingPurchase(billingPurchase);

            return Ok(outputModel);
        }

        private object MapBillingPurchase(BillingPurchase billingPurchase)
        {
            return new
            {
                Id = billingPurchase.Id,
                UserId = billingPurchase.UserId,
                PurchaseId = billingPurchase.PurchaseId,
                ProductId = billingPurchase.ProductId,
                AutoRenewing = billingPurchase.AutoRenewing,
                PurchaseToken = billingPurchase.PurchaseToken,
                PurchaseState = billingPurchase.PurchaseState,
                ConsumptionState = billingPurchase.ConsumptionState,
                Platform = billingPurchase.Platform,
                TransactionDateUtc = billingPurchase.TransactionDateUtc
            };
        }
    }
}
