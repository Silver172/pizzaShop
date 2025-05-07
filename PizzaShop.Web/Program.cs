using System.Text;
using Microsoft.EntityFrameworkCore;
using PizzaShop.DataAccess.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using PizzaShop.DataAccess.Interfaces;
using PizzaShop.DataAccess.Implementation;
using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Implementation;
using PizzaShop.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using PizzaShop.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connection = builder.Configuration.GetConnectionString("PizzaShopDbConnection");
builder.Services.AddDbContext<PizzashopContext>(options => options.UseNpgsql(connection));

DependencyInjection.RegisterServices(builder.Services, builder.Configuration.GetConnectionString("PizzaShopDbConnection"));

builder.Services.AddSession(options =>  
{  
    options.IdleTimeout = TimeSpan.FromDays(1);  
    options.Cookie.HttpOnly = true;  
    options.Cookie.IsEssential = true;  
});  

builder.Services.AddScoped<IAuthenticationServices, PizzaShop.Service.Implementation.AuthenticationService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ITableAndSectionService, TableAndSectionService>();
builder.Services.AddScoped<ITaxesAndFeesService, TaxesAndFeesService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITableOrderAppService, TableOrderAppService>();
builder.Services.AddScoped<IKOTOrderAppService, KOTOrderAppService>();
builder.Services.AddScoped<IWaitingListOrderAppService, WaitingListOrderAppService>();
builder.Services.AddScoped<IMenuOrderAppService, MenuOrderAppService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IModifierGroupRepository, ModifierGroupRepository>();
builder.Services.AddScoped<IMappingMenuItemWithModifierRepository, MappingMenuItemWithModifierRepository>();
builder.Services.AddScoped<IModifierReposotory, ModifierRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<ITaxesAndFeesRepository, TaxesAndFeesRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IWaitingTokenRepository, WaitingTokenRepository>();
builder.Services.AddScoped<ITableOrderMappingRepository, TableOrderMappingRepository>();
builder.Services.AddScoped<IOrderTaxMappingRepository, OrderTaxMappingRepository>();
builder.Services.AddScoped<IOrderItemModifierMappingRepository, OrderItemModifierMappingRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

// add for session http accessor
builder.Services.AddHttpContextAccessor();
// JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers["Authorization"] = "Bearer " + token;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Home/Error404");
                }
                context.HandleResponse();
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                if (context.Response.StatusCode == 400)
                {
                    context.Response.WriteAsync("You are not authorized to access this resource.");
                    return Task.CompletedTask;
                }
                context.Response.Redirect("/Home/Error403");
                return Task.CompletedTask;
            },
        };
    });


builder.Services.AddAuthorization(options =>
{
    var permissions = new[]
    {
        "Users.View", "Users.AddEdit", "Users.Delete",
        "Role.View", "Role.AddEdit", "Role.Delete",
        "Menu.View", "Menu.AddEdit", "Menu.Delete",
        "TableSection.View", "TableSection.AddEdit", "TableSection.Delete",
        "TaxFees.View", "TaxFees.AddEdit", "TaxFees.Delete",
        "Orders.View", "Orders.AddEdit", "Orders.Delete",
        "Customers.View", "Customers.AddEdit", "Customers.Delete"
    };

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}");

app.Run();
