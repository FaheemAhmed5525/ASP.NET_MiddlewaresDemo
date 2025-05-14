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

using System.Globalization;
using System.Threading;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using MiddlewaresDemo;

var builder = WebApplication.CreateBuilder();

// ------------------- Services Configuration -------------------

// Enable fixed rate limiting with a named policy
//builder.Services.AddRateLimiter(options =>
//{
//    options.AddFixedWindowLimiter("controlledResources", option =>
//    {
//        option.PermitLimit = 8;
//        option.Window = TimeSpan.FromSeconds(8);
//        option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        option.QueueLimit = 2;
//    });

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
//    */
//});


/////Slider rate limit with a nmed policy
//var rateOptions = new RateLimiterOptions();

//builder.Configuration.GetSection("SliderRateLimiter").Bind(rateOptions);

//builder.Services.AddRateLimiter(options =>
//options.AddSlidingWindowLimiter(policyName: "sliding", option =>
//{
//    option.PermitLimit = 64;
//    option.Window =TimeSpan.FromSeconds(8);
//    option.SegmentsPerWindow = 8;
//    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//    option.QueueLimit = 4;
//}
//));


////Token bucket limit with named policy
//var rateOptions = new RateLimiterOptions();

//builder.Configuration.GetSection("TokenBucketLimiter").Bind(rateOptions);

//builder.Services.AddRateLimiter(options =>
//options.AddTokenBucketLimiter(policyName: "tokenBucket", option =>
//{
//    option.TokenLimit = 10;
//    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//    option.QueueLimit = 4;
//    option.ReplenishmentPeriod = TimeSpan.FromSeconds(2);
//    option.TokensPerPeriod = 4;
//    option.AutoReplenishment = false;
//}));


////Conurrency limiter with named policy
//var rateOptions = new RateLimiterOptions();

//builder.Configuration.GetSection("ConcurrencyLimiter").Bind(rateOptions);

//builder.Services.AddRateLimiter(options =>
//options.AddConcurrencyLimiter(policyName: "concurrency", option =>
//{
//    option.PermitLimit = 64;
//    option.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//    option.QueueLimit = 4;
//}));


var rateOptions = new RateLimiterOptions();

//// --------------- Rate Limiting Partitions -----------------

////Rate limiter by ip address
//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//    RateLimitPartition.GetFixedWindowLimiter(
//        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
//        factory: _ => new FixedWindowRateLimiterOptions
//        {
//            PermitLimit = 10,
//            Window = TimeSpan.FromMinutes(1)
//        }
//    ));

//});


////Rate limter by user identity 
//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//    RateLimitPartition.GetFixedWindowLimiter(
//        partitionKey: httpContext.User.Identity?.ToString() ?? "anonomys",
//        factory: _ => new FixedWindowRateLimiterOptions
//        {
//            PermitLimit = 2,
//            Window = TimeSpan.FromMinutes(1)
//        }
//    ));
//});

////Rate Limiter by Api key
//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//    {
//        string apiKey = httpContext.Request.Headers["X-API-Key"].ToString() ?? "no-key";

//        return apiKey switch
//        {
//            "premium-key" => RateLimitPartition.GetFixedWindowLimiter(
//                partitionKey: apiKey,
//                factory: _ => new FixedWindowRateLimiterOptions
//                {
//                    PermitLimit = 4,
//                    Window = TimeSpan.FromMinutes(1)
//                }
//            ),
//            _ => RateLimitPartition.GetFixedWindowLimiter(
//                partitionKey: apiKey,
//                factory: _ => new FixedWindowRateLimiterOptions
//                {
//                    PermitLimit = 2,
//                    Window = TimeSpan.FromMinutes(1)
//                })
//        };
//    });
//});


////Rate limiter by Endpoint
//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//    {
//        string path = httpContext.Request.Path.ToString();

//        if (path.StartsWith("/api/public")) {
//            return RateLimitPartition.GetFixedWindowLimiter(
//                partitionKey: $"{httpContext.Connection.RemoteIpAddress}-public",
//                factory: _ => new FixedWindowRateLimiterOptions
//                {
//                    PermitLimit = 2,
//                    Window = TimeSpan.FromMinutes(1)
//                }
//            );
//        }
//        return RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
//            factory: _ => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 5,
//                Window = TimeSpan.FromMinutes(1)
//            }
//        );
//    });
//});



////Chained linmt 
//builder.Services.AddRateLimiter(_ =>
//{
//    _.OnRejected = async (context, cancellationToken) =>
//    {
//        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
//        {
//            context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
//        }
//        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//        await context.HttpContext.Response.WriteAsync("Too many request. Please try laterl", cancellationToken);
//    };

//    _.GlobalLimiter = PartitionedRateLimiter.CreateChained(
//        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//        {
//            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

//            return RateLimitPartition.GetFixedWindowLimiter
//            (userAgent, _ => new FixedWindowRateLimiterOptions
//            {
//                AutoReplenishment = true,
//                PermitLimit = 4,
//                Window = TimeSpan.FromSeconds(2)
//            });
//        }),
//        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//        {
//            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

//            return RateLimitPartition.GetFixedWindowLimiter
//            (userAgent, _ => new FixedWindowRateLimiterOptions
//            {
//                AutoReplenishment = true,
//                PermitLimit = 30,
//                Window = TimeSpan.FromSeconds(20)
//            });
//        }));
//});



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
//app.MapGet("/resources/controlled", () => Results.Ok("This endpoint is rate limited"))              //https://localhost:7093/resources/controlled
//    .RequireRateLimiting("concurrency");

//// ------------------- Fallback -------------------

app.MapFallback(async context =>
{
    await context.Response.WriteAsync("Welcome to ASP.NET Middleware demo Project");
});

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
