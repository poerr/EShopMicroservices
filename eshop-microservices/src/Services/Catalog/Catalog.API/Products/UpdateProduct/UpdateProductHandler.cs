namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(
        Guid Id, 
        string Name, 
        List<string> Categories, 
        string Description, 
        string ImageFile, 
        decimal Price
    ) : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);

    public class UpdateProductHandler
        (IDocumentSession session, ILogger<UpdateProductHandler> logger)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("UpdateProductHandler.Handle called with {@Command}", command);

            var existingProduct = await session.LoadAsync<Product>(command.Id, cancellationToken);

            if (existingProduct == null)
            {
                throw new ProductNotFoundException();
            }

            existingProduct.Name = command.Name;
            existingProduct.Category = command.Categories;
            existingProduct.Description = command.Description;
            existingProduct.ImageFile = command.ImageFile;
            existingProduct.Price = command.Price;

            session.Update(existingProduct);
            await session.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(true);   
        }
    }
}
