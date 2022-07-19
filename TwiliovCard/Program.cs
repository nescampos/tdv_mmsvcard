using vCardLib.Models;
using vCardLib.Enums;
using vCardLib.Serializers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using Twilio.Types;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string twilioAccountSID = configuration["TwilioAccountSID"];
string twilioAuthToken = configuration["TwilioAuthToken"];
string twilioMessagingServiceSid = configuration["TwilioMessagingServiceSid"];
string serverUrl = "<ngrok URL>"; // has to end with one '/'
string vCardFolder = "<full folder path>";

vCard vCardData = new vCard();
vCardData.FamilyName = "Campos";
vCardData.GivenName = "Néstor";
vCardData.PhoneNumbers = new List<TelephoneNumber>();
vCardData.PhoneNumbers.Add(new TelephoneNumber { Type = TelephoneNumberType.Work, Value = "<phone number>", CustomTypeName = "Main contact" });

string serializedvCardData = Serializer.Serialize(vCardData);

string vCardFileName = "ncampos.vcf";
string SavePath = Path.Combine(vCardFolder, vCardFileName);
File.WriteAllText(SavePath, serializedvCardData);


TwilioClient.Init(twilioAccountSID, twilioAuthToken);

var messageOptions = new CreateMessageOptions(new PhoneNumber("<to number>"));
messageOptions.Body = "This is the main contact for Néstor.";
messageOptions.MediaUrl = new List<Uri>() { new Uri(serverUrl + vCardFileName) };
messageOptions.SendAsMms = true;
messageOptions.MessagingServiceSid = twilioMessagingServiceSid;

//messageOptions.SendAt = DateTime.UtcNow.AddMinutes(20);
//messageOptions.ScheduleType = MessageResource.ScheduleTypeEnum.Fixed;


var message = MessageResource.Create(messageOptions);

Console.WriteLine($"Message SID: {message.Sid}");
Console.WriteLine($"Message Status: {message.Status}");

File.Delete(SavePath);