using Core.CommonModels.Enums;
using Core.Db;
using Core.Db.Ef;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Posts.Api.Dtos;
using Posts.App.Queries;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(GameAutomapper));
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEntityRepository<Post>, EntityRepository<Post>>();
builder.Services.AddScoped<IEntityRepository<Tag>, EntityRepository<Tag>>();
builder.Services.AddScoped<IEntityRepository<TagNavigation>, EntityRepository<TagNavigation>>();
builder.Services.AddScoped<PostsQueries>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"Bearer token\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8 
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false, 
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents()
        {
            OnTokenValidated = async ctx =>
            {
                var usermgr = ctx.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                var signmgr = ctx.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                var username = ctx.Principal?.FindFirst(ClaimTypes.Name)?.Value;
                var user = await usermgr.FindByNameAsync(username);
                ctx.Principal = await signmgr.CreateUserPrincipalAsync(user);
            }
        };
    });


builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequireDigit = true;
    opts.Password.RequiredLength = 8;
    opts.Password.RequireUppercase = true;
    opts.Password.RequireLowercase = true;
    opts.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    }));

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NgOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

DoMigration(app);

app.Run();

void DoMigration(IApplicationBuilder builder)
{
    var context = builder.ApplicationServices.CreateScope().ServiceProvider
        .GetRequiredService<ApplicationContext>();
    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}