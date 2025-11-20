using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Tools.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string email, string subject, string body)
    {
        // 1. Get info from appsettings.json
        var apiKey = _configuration["SendGrid:ApiKey"];
        var fromEmail = _configuration["SendGrid:FromEmail"];
        var fromName = _configuration["SendGrid:FromName"];
        Console.WriteLine("API key: " + apiKey);
        Console.WriteLine("FromEmail: " + fromEmail);
        Console.WriteLine("FromName: " + fromName);
        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrEmpty(fromEmail))
        {
            throw new Exception("Mail server configuration is missing");
        }
        
        // 2. Create client and content
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(email);
        
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", body);
        
        // 3. send Email
        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            //TODO: Log error for further investigation
        }
    }
}