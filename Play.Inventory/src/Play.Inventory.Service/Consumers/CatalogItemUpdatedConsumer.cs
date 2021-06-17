using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Service.Repositories;
using Play.Inventory.Service.Entities;
using System.Threading.Tasks;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {
            var message = context.Message;

            var item = await repository.GetByIdAsync(message.ItemId);

            if (item == null)
            {
                item = new CatalogItem
                {
                    Id = message.ItemId,
                    Name = message.Name,
                    Description = message.Description
                };
            }
            else
            {
                item.Name = message.Name;
                item.Description = message.Description;
            }

            await repository.UpdateAsync(item);
        }
    }
}
