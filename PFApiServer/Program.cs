using PFApiServer;
using PFApiServer.RDB;
using PFApiServer.Redis;
using System;
using System.Text.Json;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = builder.Configuration;
builder.Services.Configure<DBConfig>(configuration.GetSection(nameof(DBConfig)));
builder.Services.AddTransient<IGlobalDB, GlobalDB>();
builder.Services.AddSingleton<IUserDB, UserDB>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

SettingLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();

ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
LogManager.SetLoggerFactory(loggerFactory, "Global");

app.UseMiddleware<PFApiServer.Middleware.CheckUserAuthAndLoadUserData>();

app.UseRouting();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { _ = endpoints.MapControllers(); });
#pragma warning restore ASP0014

var userDB = app.Services.GetRequiredService<IUserDB>();
userDB.Init(configuration.GetSection("DBConfig")["Redis"] ?? throw new ArgumentNullException());

app.Run();
//app.Run(configuration["ServerAddress"]);

void SettingLogger()
{
    ILoggingBuilder logging = builder.Logging;
    _ = logging.ClearProviders();

    string fileDir = configuration["logdir"]!;

    bool exists = Directory.Exists(fileDir);

    if (!exists)
    {
        _ = Directory.CreateDirectory(fileDir);
    }

    _ = logging.AddZLoggerRollingFile(
        (dt, x) => $"{fileDir}{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log",
        x => x.ToLocalTime().Date, 1024,
        options =>
        {
            options.EnableStructuredLogging = true;
            JsonEncodedText time = JsonEncodedText.Encode("Timestamp");
            //DateTime.Now는 UTC+0 이고 한국은 UTC+9이므로 9시간을 더한 값을 출력한다.
            JsonEncodedText timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));

            options.StructuredLoggingFormatter = (writer, info) =>
            {
                writer.WriteString(time, timeValue);
                info.WriteToJsonWriter(writer);
            };
        }); // 1024KB

    _ = logging.AddZLoggerConsole(options =>
    {
        options.EnableStructuredLogging = true;
        JsonEncodedText time = JsonEncodedText.Encode("EventTime");
        JsonEncodedText timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));

        options.StructuredLoggingFormatter = (writer, info) =>
        {
            writer.WriteString(time, timeValue);
            info.WriteToJsonWriter(writer);
        };
    });

}