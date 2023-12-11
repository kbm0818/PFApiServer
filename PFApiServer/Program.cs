using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using PFApiServer;
using PFApiServer.Models.Global;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// ����� ���� �� controllers/actions �� ���۵ǵ��� ���񽺿� �߰��Ѵ�
builder.Services.AddControllers();

// �⺻������ ASP.NET���� JSON ����ȭ�� System.Text.Json�� ����Ѵ�
// ��� NewtonsoftJson�� ����ϱ� ���� ȣ���̴�
builder.Services.AddControllers().AddNewtonsoftJson();

#region DBContext �߰�
builder.Services.AddEntityFrameworkMySQL()
                .AddDbContextPool<GlobalContext>(options =>
                {
#if DEBUG
                    options.UseMySQL(builder.Configuration.GetConnectionString("DatabaseConnection_Global_Dev")!);
#elif RELEASE
                    options.UseMySQL(builder.Configuration.GetConnectionString("DatabaseConnection_Global")!);
#endif
                });
#endregion DBContext �߰�

#region Redis �߰�
#if DEBUG
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("RedisConnection_Dev"); });
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection_Dev")!);
#elif RELEASE 
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetConnectionString("RedisConnection"); });
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!);
#endif
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
#endregion Redis �߰�

var app = builder.Build();

// Forward headers in order to be able to operate behind a reverse proxy
// ������ ���Ͻÿ����� �����Ҽ� �ֵ��� �߰�
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    // Enable exception handling middleware
    app.UseExceptionHandler("/Error");
    // HTTPS�� ��� ����
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    // ���� �����ͺ��̽��� ���� ���� ���̱׷��̼��� ������ �����Ѵ�
    //app.UseMigrationsEndPoint();
}

// ������ ó���ϴ� ��ü �̵��� ����
app.UseAuthentication();

#region �̵���� �߰�
#endregion �̵���� �߰�

app.MapControllers();
app.Run();