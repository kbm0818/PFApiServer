using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using PFApiServer;
using PFApiServer.Models.Global;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// 라우팅 로직 즉 controllers/actions 이 동작되도록 서비스에 추가한다
builder.Services.AddControllers();

// 기본적으로 ASP.NET에선 JSON 직렬화를 System.Text.Json을 사용한다
// 대신 NewtonsoftJson을 사용하기 위한 호출이다
builder.Services.AddControllers().AddNewtonsoftJson();

#region DBContext 추가
builder.Services.AddEntityFrameworkMySQL()
                .AddDbContextPool<GlobalContext>(options =>
                {
#if DEBUG
                    options.UseMySQL(builder.Configuration.GetConnectionString("DatabaseConnection_Global_Dev")!);
#elif RELEASE
                    options.UseMySQL(builder.Configuration.GetConnectionString("DatabaseConnection_Global")!);
#endif
                });
#endregion DBContext 추가

#region Redis 추가
#if DEBUG
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("RedisConnection_Dev"); });
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection_Dev")!);
#elif RELEASE 
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("RedisConnection"); });
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!);
#endif
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
#endregion Redis 추가

var app = builder.Build();

// Forward headers in order to be able to operate behind a reverse proxy
// 역방향 프록시에서도 동작할수 있도록 추가
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    // Enable exception handling middleware
    app.UseExceptionHandler("/Error");
    // HTTPS만 사용 강제
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    // 사용된 데이터베이스에 보류 중인 마이그레이션이 있으면 적용한다
    //app.UseMigrationsEndPoint();
}

// 인증을 처리하는 자체 미들웨어를 구현
app.UseAuthentication();

#region 미들웨어 추가
#endregion 미들웨어 추가

app.MapControllers();
app.Run();