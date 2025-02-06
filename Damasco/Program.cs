
using API.GraphQL;
using API.GraphQL.Mutations;
using Common.Helpers.FileUpload;
using Common.Model.FilterTypes;
using Common.Token;
using Damasco.GraphQL;
using Damasco.GraphQL.Mutations;
using Damasco.GraphQL.Queries;
using Datalayer;
using HotChocolate.Types.Pagination;
using LoggerServices.Application;
using LoggerServices.Implementations;
using LoggerServices.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserServices;
using UserServices.Application;
using UserServices.Implementations;
using UserServices.Interfaces;

var AllowedOrigins = "AllowedOrigins";
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var jwtKey = configuration.GetValue<string>("JwtRequirements:Key");
// Add services to the container.
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// mediatr injections
builder.Services.AddUserApplication();
builder.Services.AddLoggerApplication();


//datalayer
builder.Services.AddScoped<IRepositoryService, RepositoryService>();
builder.Services.AddScoped<IFileUploadHelper, FileUploadHelper>();

// services
builder.Services.AddScoped<ValidateModuleHeader>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IJwtAuthentication, JwtAuthentication>();
builder.Services.AddScoped<ILoggerService, LoggerService>();


// gql injection
builder.Services.AddGQLDependencyInjection();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigins,
        builder =>
        {
            builder.AllowAnyOrigin() //WithOrigins(GlobalSettings.CORSAllowedOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// graphql config

builder.Services.AddGraphQLServer()
    .AddQueryType(q => q.Name("Query"))
   
    .AddType<UserQueryType>()

    .AddMutationType(q => q.Name("Mutation"))
  
    .AddType<UserMutation>()
    .AddType<AccountMutation>()
    .AddUploadType()

    .AddType<CategoryFilterType>()

    .AddFiltering<CustomFilteringConvention>()
    .AddSorting()
    .SetPagingOptions(new PagingOptions
    {
        MaxPageSize = int.MaxValue - 1,
        DefaultPageSize = int.MaxValue - 1,
        IncludeTotalCount = true
    })

    //.AddHttpRequestInterceptor<HeaderChecker>()
    .AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(AllowedOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseRouting();
app.MapGraphQL();
app.UseStaticFiles();
app.Run();
