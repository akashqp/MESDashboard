using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.AspNetCore.Http;
using MESDashboard.Data;
using MESDashboard.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Azure;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MESDashboard.Services
{
    public class ChatbotService
    {
        private Dictionary<string, (string Response, List<string> Options)> _responses;
        // private PredictionEngine<IntentData, IntentPrediction> _predEngine;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ChatbotService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, HttpClient httpClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _httpClient = httpClient;

            _responses = new Dictionary<string, (string, List<string>)>
            {
                { "greeting", ("Hi there! How can I help you today?", new List<string> { "View Reports", "Tickets", "Talk to Engineer" }) },
                { "help", ("Hi there! How can I help you today?", new List<string> { "View Reports", "Tickets", "Talk to Engineer" }) },
                { "view_reports", ("Which report would you like to view?", new List<string> { "View downtime reports", "View production reports", "Production Analysis" }) },
                { "view_downtime_reports", ("To view Downtime Reports, you can access them by clicking on 'Reports' in the nav-bar and selecting 'Downtime Report' from the drop-down menu. Alternatively, you can click on the link below.", new List<string> {"/Reports/Downtime"}) },
                { "view_production_reports", ("To view Production Reports, you can access them by clicking on 'Reports' in the nav-bar and selecting 'Production Report' from the drop-down menu. Alternatively, you can click on the link below.", new List<string> {"/Reports/Production"}) },
                { "production_analysis", ("How many \"Months\" of Productin Data would you like to analyze?", new List<string> {"3 Months", "6 Months", "12 Months"}) },
                { "tickets", ("What would you like to do?", new List<string> { "Submit a ticket", "View submitted tickets", "View Ticket Status" }) },
                { "submit_ticket", ("Which department does this ticket belong to?", new List<string> {"IT", "HR", "Production"}) },
                { "talk_to_engineer", ("You can contact an engineer by going to the 'Contact Engineer' section.", new List<string>()) },
                { "view_submitted_tickets", ("To view submitted tickets, click on the link below.", new List<string> { "/Tickets/SubmittedTickets" }) },
                { "view_ticket_status", ("Enter your ticket ID to view the status.", new List<string>()) },
                { "unknown", ("I'm sorry, I didn't understand that. Can you please rephrase?", new List<string>())}
            };
        }

        private void SaveChatHistory(string userMessage, string botResponse, List<string> options)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var chatHistory = session.GetString("ChatHistory");

            var history = chatHistory != null ? JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(chatHistory) : new List<Dictionary<string, string>>();

            history.Add(new Dictionary<string, string> { { "user", userMessage }, { "bot", botResponse }, { "options", JsonConvert.SerializeObject(options) } });
            Console.WriteLine($"Chat History: {JsonConvert.SerializeObject(history)}");
            session.SetString("ChatHistory", JsonConvert.SerializeObject(history));
        }


        public async Task<object> GetChatHistory()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var chatHistory = session.GetString("ChatHistory");

            if (chatHistory == null)
            {
                return new List<(string User, string Bot, List<string> Options)>();
            }

            return chatHistory;
        }

        public void ClearChatHistory()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Remove("ChatHistory");
            session.Remove("ConversationState");

            AddInitialMessage();
        }

        public async Task<object> GetResponseAsync(string userInput)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var conversationState = session.GetString("ConversationState");

            if (conversationState == null)
            {   
                var Prediction = await PredictIntent(userInput);
                // Convert Prediction from String to Json

                var prediction = JsonConvert.DeserializeObject<dynamic>(Prediction).Intent.ToString();
                var Similarity = JsonConvert.DeserializeObject<dynamic>(Prediction).Similarity.ToString();
                // convert similarity to float
                var similarity = float.Parse(Similarity);
                Console.WriteLine($"Predicted intent:///{prediction}///");

                // Initialise response variable of type: System.Collections.Generic.List<string> options>
                (string Response, List<string> Options) response = ("", new List<string>());
                if (_responses.ContainsKey(prediction))
                {
                    if (prediction == "submit_ticket")
                    {
                        session.SetString("ConversationState", "awaiting_department");
                    }
                    else if (prediction == "view_ticket_status")
                    {
                        session.SetString("ConversationState", "awaiting_ticket_id");
                    }
                    else if (prediction == "view_reports")
                    {
                        session.SetString("ConversationState", "view_reports");
                    }
                    else if (prediction == "tickets")
                    {
                        session.SetString("ConversationState", "tickets");
                        response = _responses[prediction];

                        SaveChatHistory(userInput, response.Response, response.Options);
                        
                        return new { message = response.Response, options = response.Options };
                    }
                    else if (prediction == "production_analysis")
                    {
                        session.SetString("ConversationState", "awaiting_months");
                    }

                    response = _responses[prediction];
                    if (prediction == "greeting" || prediction == "help" || similarity <= 0.9) {
                        var generatedResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\n\nUser Input: " + userInput + "\nPredefined Response: " + response.Response);
                        response.Response = generatedResponse;
                    }

                    var responseMessage = new
                    {
                        message = response.Response,
                        options = response.Options
                    };

                    SaveChatHistory(userInput, response.Response, response.Options);

                    return responseMessage;
                }

                var unknownResponse = new
                {
                    message = "I'm sorry, I didn't understand that. Can you please rephrase?",
                    options = new List<string>()
                };

                SaveChatHistory(userInput, unknownResponse.message, response.Options);

                return unknownResponse;
            }
            else
            {   
                string botResponse = string.Empty;
                switch (conversationState)
                {   
                    case "view_reports":
                        var Prediction = await PredictIntent(userInput);
                        var prediction = JsonConvert.DeserializeObject<dynamic>(Prediction).Intent.ToString();
                        var Similarity = JsonConvert.DeserializeObject<dynamic>(Prediction).Similarity.ToString();
                        var similarity = float.Parse(Similarity);
                        Console.WriteLine($"Predicted intent: {prediction}");
                        if (prediction == "view_downtime_reports")
                        {
                            session.Remove("ConversationState");
                            var response = _responses["view_downtime_reports"];
                            botResponse = response.Response;
                            if (similarity <= 0.9) {
                                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            }
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = response.Response, link = response.Options };
                        }
                        else if (prediction == "view_production_reports")
                        {
                            session.Remove("ConversationState");
                            var response = _responses["view_production_reports"];
                            botResponse = response.Response;
                            if (similarity <= 0.9) {
                                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            }
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = response.Response, link = response.Options };
                        }
                        else if (prediction == "production_analysis")
                        {
                            session.Remove("ConversationState");
                            session.SetString("ConversationState", "awaiting_months");
                            var response = _responses["production_analysis"];
                            botResponse = response.Response;
                            if (similarity <= 0.9) {
                                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            }
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = response.Response, options = response.Options };
                        }
                        else
                        {   var response = _responses["view_reports"];
                            botResponse = "Please choose a valid option:\n1. View downtime reports\n2. View production reports\n3. Production Analysis";
                            if (similarity <= 0.9) {
                                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            }
                            botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = botResponse, options = response.Options };
                        }
                    
                    case "tickets":

                        Prediction = await PredictIntent(userInput);
                        prediction = JsonConvert.DeserializeObject<dynamic>(Prediction).Intent.ToString();
                        Similarity = JsonConvert.DeserializeObject<dynamic>(Prediction).Similarity.ToString();
                        similarity = Convert.ToInt32(Similarity);

                        if (prediction == "submit_ticket")
                        {
                            session.Remove("ConversationState");
                            session.SetString("ConversationState", "awaiting_department");
                            var response = _responses["submit_ticket"];
                            botResponse = response.Response;
                            if (similarity <= 0.9) {
                                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            }
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = response.Response, options = response.Options };
                        }
                        else if (prediction == "view_ticket_status")
                        {
                            session.Remove("ConversationState");
                            session.SetString("ConversationState", "awaiting_ticket_id");
                            var response = _responses["view_ticket_status"];
                            botResponse = response.Response;
                            if (similarity <= 0.9) {
                                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            }
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = response.Response, options = response.Options };
                        }
                        else if (prediction == "view_submitted_tickets")
                        {
                            session.Remove("ConversationState");
                            var response = _responses["view_submitted_tickets"];
                            botResponse = response.Response;
                            if (similarity <= 0.9) {
                                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            }
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = response.Response, link = response.Options };
                        }
                        else
                        {   
                            var response = _responses["tickets"];
                            botResponse = "Please choose a valid option:\n1. Submit a ticket\n2. View submitted tickets\n3. View Ticket Status";
                            if (similarity <= 0.9) {
                                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            }
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = botResponse, options = response.Options};
                        }

                    case "awaiting_department":
                        var Options = new List<string> { "P1", "P2", "P3" };
                        session.SetString("TicketDepartment", userInput);
                        session.SetString("ConversationState", "awaiting_priority");
                        botResponse = "Please specify the priority of the ticket (P1, P2, P3):";
                        SaveChatHistory(userInput, botResponse, Options);
                        return new { message = botResponse, options = Options };

                    case "awaiting_priority":
                        Options = new List<string>();
                        session.SetString("TicketPriority", userInput);
                        session.SetString("ConversationState", "awaiting_issue");
                        botResponse = "Please describe the issue:";
                        botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                        SaveChatHistory(userInput, botResponse, Options);
                        return new { message = botResponse, options = Options };

                    case "awaiting_issue":
                        Options = new List<string>();
                        var department = session.GetString("TicketDepartment");
                        var priority = session.GetString("TicketPriority");
                        var issue = userInput;

                        var ticket = new Ticket
                        {
                            Department = department,
                            Priority = priority,
                            Issue = issue,
                            Status = "Open",
                            SubmittedAt = DateTime.Now
                        };

                        _context.Tickets.Add(ticket);
                        _context.SaveChanges();

                        botResponse = $"Thank you! Your ticket has been submitted with ID: {ticket.Id}";
                        SaveChatHistory(userInput, botResponse, Options);

                        session.Remove("ConversationState");
                        session.Remove("TicketDepartment");
                        session.Remove("TicketPriority");
                        return new { message = botResponse, options = Options };

                    case "awaiting_ticket_id":
                        var ticketId = userInput;
                        var Ticket = await _context.Tickets.FindAsync(int.Parse(ticketId));
                        if (Ticket == null)
                        {   
                            Options = new List<string>();
                            botResponse = $"Ticket {ticketId} not found. Please enter a valid ticket ID.";
                            botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            SaveChatHistory(userInput, botResponse, Options);
                            return new { message = botResponse, options = Options};
                        }
                        else
                        {
                            Options = new List<string>();
                            session.Remove("ConversationState");
                            botResponse = $"The status of ticket {ticketId} is: {Ticket.Status}";
                            SaveChatHistory(userInput, botResponse, Options);
                            return new { message = botResponse, options = Options };
                        }

                    case "awaiting_months":
                        Console.WriteLine(userInput);
                        if (userInput == "3 Months" || userInput == "6 Months" || userInput == "12 Months")
                        {
                            var months = 0;
                            if (userInput == "3 Months")
                            {
                                months = 3;
                            }
                            else if (userInput == "6 Months")
                            {
                                months = 6;
                            }
                            else
                            {
                                months = 12;
                            }
                            Options = new List<string> { "Monthly Rejection Reasons" };
                            session.SetInt32("Months", months);
                            session.SetString("ConversationState", "awaiting_analysis");
                            botResponse = "What kind of analysis would you like to perform?";
                            SaveChatHistory(userInput, botResponse, Options);
                            return new { message = botResponse, options = new List<string> { "Monthly Rejection Reasons" } };
                        }
                        else
                        {      
                            var response = _responses["production_analysis"];
                            botResponse = "Please choose a valid option:\n1. 3 Months\n2. 6 Months\n3. 12 Months";
                            botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                            SaveChatHistory(userInput, botResponse, response.Options);
                            return new { message = botResponse, options = response.Options };
                        }

                    case "awaiting_analysis":
                        Console.WriteLine(userInput.ToLower());
                        if (userInput.ToLower() == "monthly rejection reasons")
                        {   
                            session.SetString("Analysis", userInput);
                            session.Remove("ConversationState");
                            var response = await _httpClient.GetAsync("http://127.0.0.1:5000/monthly_rejection_reasons/" + session.GetInt32("Months"));
                            return new { message = "The Analysis is being opened....", options = new List<string> { "Open in Same Page" },
                                        response = response.Content.ReadAsStringAsync().Result, title = "Monthly Rejection Reasons"};
                        }
                        else
                        {
                            return new { message = "Please choose a valid option:\n1. Monthly Rejection Reasons", options = new List<string> { "Monthly Rejection Reasons" } };
                        }
                }
                var options = new List<string>();
                botResponse = "I'm sorry, something went wrong. Please try again.";
                botResponse = await CallGeminiAPI("\nBased on User Input and Predefined Response, generate an appropriate response for the user. \nKeep in mind that I am asking for a response for a chatbot, so only return direct response, no need of explanation or such." + "\nUser Input: " + userInput + "\nPredefined Response: " + botResponse);
                SaveChatHistory(userInput, botResponse, options);
                return new { message = botResponse, options = options };
            }
        }

        public async Task<string> PredictIntent(string userInput)
        {
            var jsonContent = JsonConvert.SerializeObject(new { text = userInput });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://127.0.0.1:5000/predict_intent", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var predictionResult = JsonConvert.DeserializeObject<PredictionResponse>(jsonResponse);
                var similarityResult = JsonConvert.DeserializeObject<SimilarityResponse>(jsonResponse);

                return System.Text.Json.JsonSerializer.Serialize(new { Intent = predictionResult.Intent, Similarity = similarityResult.Similarity });
            }
            else
            {
                throw new Exception("Failed to get intent prediction from the Flask server.");
            }
        }

        public async Task<string> CallGeminiAPI(string prompt)
        {   
            var session = _httpContextAccessor.HttpContext.Session;
            var chatHistory = session.GetString("ChatHistory");
            // Parse the chatHistory to Json
            if (chatHistory == null)
            {
                chatHistory = "[]";
            }
            var chatHistoryJson = JArray.Parse(chatHistory);
            var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:5000/generate_text", new { input_text = prompt, ChatHistory = chatHistory });
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var generatedResponse = JsonConvert.DeserializeObject<GeneratedResponse>(jsonResponse);
                return generatedResponse.GeneratedText;
            }
            else
            {
                throw new Exception("Failed to get response from the Gemini API.");
            }
        }
    
        public void AddInitialMessage()
        {   
            // Add initial message to the chat history according to the time of day
            string userMessage = "";
            string botResponse = "";
            if (DateTime.Now.Hour < 12)
            {   
                botResponse = "Good Morning, May I know your name?";
            }
            else if (DateTime.Now.Hour < 18)
            {
                // Make a greeting and ask for user's name
                botResponse = "Good Afternoon, May I know your name?";
            }
            else
            {
                // Make a greeting and ask for user's name
                botResponse = "Good Evening, May I know your name?";
            }
            List<string> options = new List<string>();
            SaveChatHistory(userMessage, botResponse, options);
        }

    }


    public class PredictionResponse
    {
        [JsonProperty("intent")]
        public string Intent { get; set; }
    }

    public class SimilarityResponse
    {
        [JsonProperty("similarity")]
        public float Similarity { get; set; }
    }

    public class GeneratedResponse
    {
        [JsonProperty("output_text")]
        public string GeneratedText { get; set; }
    }

    public class IntentData
    {
        public string Text { get; set; }
        public string Intent { get; set; }
    }
}
