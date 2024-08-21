using System.Text;
using Azure.Messaging.ServiceBus;
using Core.Entities;
using Core.Repositories;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("ServiceBusConnectionString") ?? string.Empty;
var topicName = builder.Configuration["ServiceBusTopicName"] ?? string.Empty;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/bookingrequests", async Task<IResult> (BookingRequest bookingRequest, IOwnerRepository ownerRepository, ITenantRepository tenantRepository) =>
    {
        try
        {
            var sender = new ServiceBusClient(connectionString).CreateSender(topicName);
            var owner = ownerRepository.GetByPropertyId(bookingRequest.PropertyId);
            var tenant = tenantRepository.GetById(bookingRequest.TenantId);
            var booking = new Booking
            {
                Request = bookingRequest,
                OwnerName = owner.FullName,
                TenantName = tenant.FullName
            };

            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(booking)))
            {
                ApplicationProperties =
                {
                    { "tenantId", booking.Request.TenantId },
                    { "propertyId", booking.Request.PropertyId },
                    { "rentStart", booking.Request.RentStart },
                    { "rentEnd", booking.Request.RentEnd },
                    { "ownerName", booking.OwnerName },
                    { "tenantName", booking.TenantName }
                }
            };

            await sender.SendMessageAsync(message);
            Console.WriteLine("Message posted successfully.");

            return Results.Created();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    })
    .WithName("Bookings")
    .WithOpenApi();

app.Run();
