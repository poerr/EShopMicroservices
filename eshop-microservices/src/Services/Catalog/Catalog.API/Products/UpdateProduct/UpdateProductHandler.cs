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

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty().WithMessage("Product ID is required");

            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 150).WithMessage("Name must be between 2 and 150 characters");

            RuleFor(command => command.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }

    public class UpdateProductHandler
        (IDocumentSession session)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var existingProduct = await session.LoadAsync<Product>(command.Id, cancellationToken);

            if (existingProduct == null)
            {
                throw new ProductNotFoundException(command.Id);
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
