//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using MiddlewaresDemo;

var builder = WebApplication.CreateBuilder();

// ------------------- Services Configuration -------------------

// Enable rate limiting with a named policy
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("controlledResources", option =>
    {
        option.PermitLimit = 8;
        option.Window = TimeSpan.FromSeconds(8);
        option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        option.QueueLimit = 2;
    });

    // Optional: Global rate limiting (disabled by default)
    /*
    options.RejectionStatusCode = 429;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            })
    );
    */
});

var app = builder.Build();

//// ------------------- Custom Middleware -------------------

//// Inline middleware with short-circuit control
//app.Use(async (context, next) =>
//{
//    await context.Response.WriteAsync("Hello from middleware!\n");
//    await next(); // Remove this line to short-circuit
//});

//// UseWhen conditionally applies middleware based on query string
//app.UseWhen(
//    context => context.Request.Query.ContainsKey("message"),
//    ShowMessageMiddleware
//);

//// Register custom middleware class
//app.UseMiddleware<MiddlewareTest>();

// Apply rate limiter middleware
app.UseRateLimiter();

//// ------------------- Endpoint Routing -------------------

//// Conditional routing using MapWhen
//app.MapWhen(
//    context => context.Request.Query["time"] == "morning",
//    SayGoodMorning
//);

//app.MapWhen(
//    context => context.Request.Query.ContainsKey("evening"),
//    SayGoodEvening
//);

//// Nested mapping
//app.Map("/good", good =>
//{
//    good.Map("/evening", SayGoodEvening);
//});

//app.Map("/goodmorning", SayGoodMorning);

// Rate limited endpoint
app.MapGet("/resources/controlled", () => Results.Ok("This endpoint is rate limited"))
    .RequireRateLimiting("controlledResources");

//// ------------------- Fallback -------------------

//app.MapFallback(async context =>
//{
//    await context.Response.WriteAsync("Welcome to ASP.NET Middleware demo Project");
//});

app.Run();

//// ------------------- Middleware Handlers -------------------

//static void SayGoodMorning(IApplicationBuilder app)
//{
//    app.Run(async context =>
//    {
//        await context.Response.WriteAsync("Good Morning!\n");
//    });
//}

//static void SayGoodEvening(IApplicationBuilder app)
//{
//    app.Run(async context =>
//    {
//        await context.Response.WriteAsync("Hello and Good Evening!\n");
//    });
//}

//static void ShowMessageMiddleware(IApplicationBuilder app)
//{
//    var logger = app.ApplicationServices.GetRequiredService<ILogger<Program>>();

//    app.Use(async (context, next) =>
//    {
//        var message = context.Request.Query["message"];
//        logger.LogInformation("Message: {message}", message);
//        await context.Response.WriteAsync($"Message received: {message}\n");
//        await next();
//    });
//}
