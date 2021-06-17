using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common.Service.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> inventoryItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemRepository;

        public ItemsController(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            this.inventoryItemsRepository = itemsRepository;
            this.catalogItemRepository = catalogItemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var inventoryItemsEntities = await inventoryItemsRepository.GetAllAsync(i => i.UserId == userId);
            var itemIds = inventoryItemsEntities.Select(i => i.CatalogItemId);
            var catalogItemEntities = await catalogItemRepository.GetAllAsync(item => itemIds.Contains(item.Id));


            var inventoryItemDtos = inventoryItemsEntities.Select(i =>
            {
                var catalogItem = catalogItemEntities.Single(ci => ci.Id == i.CatalogItemId);
                return i.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> AddItem(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryItemsRepository.GetByIdAsync(
                i => i.UserId == grantItemsDto.UserId && i.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow,
                };

                await inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}