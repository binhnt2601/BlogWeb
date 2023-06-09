using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App.Models;
using App.Security.Requirement;
using App.Security.RequirementHandler;
using App.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"));
});

// builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MyBlogContext>();
builder.Services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3); // Khóa 3 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddOptions();                                        // Kích hoạt Options
var mailsettings = builder.Configuration.GetSection("MailSettings");  // đọc config
builder.Services.Configure<MailSettings>(mailsettings);               // đăng ký để Inject
builder.Services.AddSingleton<IEmailSender, SendMailService>();
// builder.Services.AddTransient<IAuthorizationRequirement, AgeRequirement>();
builder.Services.AddTransient<IAuthorizationHandler, RequirementHandler>();
// builder.Services.AddTransient<ILogger, Logger<RequirementHandler>>();

//AuthenticationService
builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    // Đọc thông tin Authentication:Google từ appsettings.json
                    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

                    // Thiết lập ClientID và ClientSecret để truy cập API google
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
                    options.CallbackPath = "/login-from-google";
                })
                .AddFacebook(facebookOptions =>
                {
                    // Đọc cấu hình
                    IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");
                    facebookOptions.AppId = facebookAuthNSection["AppId"];
                    facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
                    // Thiết lập đường dẫn Facebook chuyển hướng đến
                    facebookOptions.CallbackPath = "/login-from-facebook";
                });
builder.Services.AddAuthorization(options => {
    options.AddPolicy("AlowEditorRole", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();
        // policyBuilder.RequireRole("Admin");
        // policyBuilder.RequireRole("Editor");
        policyBuilder.RequireClaim("ManageRole", "Delete");
    });
    options.AddPolicy("TrinhDoDaiHoc", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();
        // policyBuilder.RequireRole("Admin");
        // policyBuilder.RequireRole("Editor");
        policyBuilder.RequireClaim("HocVan", "DaiHoc");
    });
    options.AddPolicy("18YearsOld", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new AgeRequirement());

        // IAuthorizationRequirement dc xu ly bang AuthorizationHandler
    });
    options.AddPolicy("UpdateBlog", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new BlogUpdateRequirement());

        // IAuthorizationRequirement dc xu ly bang AuthorizationHandler
    });
    options.AddPolicy("ShowAdminMenu", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole("Admin");

        // IAuthorizationRequirement dc xu ly bang AuthorizationHandler
    });
});
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // Trên 30 giây truy cập lại sẽ nạp lại thông tin User (Role)
    // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
    options.ValidationInterval = TimeSpan.FromSeconds(20);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
