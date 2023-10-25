using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using WishApp.Core.ServicesWithInterface;
using WishApp.Core.Services;
using WishApp.Data;
using WishApp.Data.Models.Auth;
using WishApp.Core.Models;

var builder = WebApplication.CreateBuilder(args);

#region Configuration
var appSetting = new AppSetting {
    JwtIssuerSigningKey = builder.Configuration.GetValue<string>("JwtIssuerSigningKey")!,
    GoogleSsoClientSecret = builder.Configuration.GetValue<string>("GoogleSsoClientSecret")!,
    GoogleSsoClientId = builder.Configuration.GetValue<string>("GoogleSsoClientId")!,
};

builder.Services.AddSingleton(appSetting);
#endregion

#region Db & Authentication
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
    //options.UseSqlite(builder.Configuration.GetConnectionString("SQLitePath"));
});

builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

//builder.Services.AddAuthorization(options => {
//    options.AddPolicy("Policy", policy => policy.Requirements.Add(new PolicyRequirement()));
//});

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters() {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSetting.JwtIssuerSigningKey))
    };
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => {
    options.TokenLifespan = TimeSpan.FromDays(30);
});
#endregion

#region Logging
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Wish.Api_.Log"), rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.Services.AddLogging(builder => {
    builder.AddSerilog();
});
#endregion

#region Endpoint
builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("AllowAllOrigin", builder => {
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
#endregion

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SP.Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter `Bearer` [space] and then your valid token in the text input below."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, Array.Empty<string>()
                    }
                });
});

#endregion

#region Services
builder.Services.AddTransient<IHttpClientService, HttpClientService>();
builder.Services.AddTransient<IHttpContextService, HttpContextService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<IStaticProvider, StaticProvider>();

builder.Services.AddTransient<UtilityService>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<MasterDataService>();
builder.Services.AddTransient<TransactionDataService>();
#endregion

var app = builder.Build();

#region Minimal API
app.MapGet("/", () => {
    var dtStr = new Func<string>(() => {
        try {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            return fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        catch(Exception ex) {
            return $"[{ex.Message}]";
        }
    })();

    return $"""
        WishApp.API 
        
        Last modified time: {dtStr}
        """;
});
#endregion

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(a => {
        a.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}

app.UseCors("AllowAllOrigin");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
