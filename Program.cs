using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Data;
//using Reserva_Restaurantes.Data.DbInitializerDev;
using System.Text.Json.Serialization;
using Reserva_Restaurantes.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ENTITY

var connectionString = builder.Configuration.GetConnectionString("AzureConnection") ??
                       throw new InvalidOperationException("Connection string 'AzureConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// IDENTITY
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // -> adicionar autorização - controlo de roles
    .AddEntityFrameworkStores<ApplicationDbContext>();



// JWTAdd commentMore actions
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

// configurar autenticação que valida token JWT
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// o que é um singleton? o que é um transient? o que é um scoped?
builder.Services.AddSingleton<TokenService>();


builder.Services.AddControllersWithViews();

// uso de variáveis de sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();

// Eliminar a proteção de 'ciclos' qd se faz uma pesquisa que envolva um relacionamento 1-N em Linq
// https://code-maze.com/aspnetcore-handling-circular-references-when-working-with-json/
// https://marcionizzola.medium.com/como-resolver-jsonexception-a-possible-object-cycle-was-detected-27e830ea78e5
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);

// add swagger
// https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",new OpenApiInfo {
        Title="Minha API de gestão de Reseveras de Restaurantes",
        Version="v1",
        Description="API para gestão de Reservas, Restaurantes, Utilizadores, Mesas e Pagamentos"
    });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });

    // Caminho para o XML gerado
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory,xmlFile);
    options.IncludeXmlComments(xmlPath);

});

// Eliminar a proteção de 'ciclos' qd se faz uma pesquisa que envolva um relacionamento 1-N em LinqAdd commentMore actions
// https://code-maze.com/aspnetcore-handling-circular-references-when-working-with-json/
// https://marcionizzola.medium.com/como-resolver-jsonexception-a-possible-object-cycle-was-detected-27e830ea78e5
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    //app.UseItToSeedSqlServer();
    
    // cria o swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// garante que a sessão é usada na aplicação
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();