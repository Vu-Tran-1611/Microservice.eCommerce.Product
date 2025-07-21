using ProductsMicroService.DataAccessLayer;
using ProductsMicroService.BusinessLogicLayer;
using ProductsMicroService.API.Middleware;
using FluentValidation.AspNetCore;
using ProductsMicroService.API.APIEnpoints;


var builder = WebApplication.CreateBuilder(args);


// Add DAL and BLL services 
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Fluent Validation
builder.Services.AddFluentValidationAutoValidation();

//Add model binder to read values from json to enum 
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});


//Add Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Cors 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

//Use CORS
app.UseCors();

//Swagger
app.UseSwagger();
app.UseSwaggerUI();


//Authentication and Authorization
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
// Map product endpoints
app.MapProductEndpoints();
app.Run();
