# .net10 webapi utils

## packages used

### C# Utils nuget package

[net.Utils.perfect2645](https://github.com/perfect2645/Laboratory/tree/main/Utils)

### Api Versioning

[ApiVersioning](https://github.com/dotnet/aspnet-api-versioning)

### Swashbuckle.AspNetCore.SwaggerGen

## Internal functions

### NetCore configurations

- IOC / DI configuration
- Api Versioning configuration
- Swagger configuration
- CORS configuration

### Filters

#### GlobalExceptionFilter

The `GlobalExceptionFilter` is an ASP.NET Core filter that handles exceptions globally across the application.
It captures unhandled exceptions thrown during the execution of controller actions and processes them in a centralized manner.

## Entity Framework Core

### Sql Server Database support

- AddSqlServerContext

```CSharp
var builder = WebApplication.CreateBuilder(args);

// your configuration
builder.AddSqlServerContext<ShirtsDbContext>("Net10DemoDb");
// your configuration
```

- Use repository pattern

```CSharp
    public interface IShirtRepository : IRepository<Shirt, int>
    {
        Task<Shirt?> GetByPropertiesAsync(string brand, string gender, string color, int size);
    }

    [Register(ServiceType = typeof(IShirtRepository), Lifetime = Lifetime.Scoped)]
    public class ShirtRepository : RepositoryBase<Shirt, int>, IShirtRepository
    {
        private readonly ShirtsDbContext _dbContext;
        public ShirtRepository(ShirtsDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Shirt?> GetByPropertiesAsync(string brand, string gender, string color, int size)
        {
            var targetShirt = DbSet.FirstOrDefaultAsync(s =>
                s.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase)
                && s.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase)
                && s.Color.Equals(color, StringComparison.OrdinalIgnoreCase)
                && s.Size == size);

            return targetShirt;
        }
    }
```
