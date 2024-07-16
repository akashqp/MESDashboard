import spacy
from Scripts.Gemini import predict_intent_using_gemini_api


# class ChatbotService():
    # def __init__(self):
    #     self.responses = {
    #         "greeting": ("Hi there! How can I help you today?", ["View Reports", "Tickets", "Talk to Engineer"]),
    #         "help": ("Hi there! How can I help you today?", ["View Reports", "Tickets", "Talk to Engineer"]),
    #         "view_reports": ("Which report would you like to view?", ["View downtime reports", "View production reports", "Production Analysis"]),
    #         "view_downtime_reports": ("To view Downtime Reports, you can access them by clicking on 'Reports' in the nav-bar and selecting 'Downtime Report' from the drop-down menu. Alternatively, you can click on the link below.", ["/Reports/Downtime"]),
    #         "view_production_reports": ("To view Production Reports, you can access them by clicking on 'Reports' in the nav-bar and selecting 'Production Report' from the drop-down menu. Alternatively, you can click on the link below.", ["/Reports/Production"]),
    #         "production_analysis": ("How many \"Months\" of Productin Data would you like to analyze?", ["3 Months", "6 Months", "12 Months"]),
    #         "tickets": ("What would you like to do?", ["Submit a ticket", "View submitted tickets", "View Ticket Status"]),
    #         "submit_ticket": ("Which department does this ticket belong to?", ["IT", "HR", "Production"]),
    #         "talk_to_engineer": ("You can contact an engineer by going to the 'Contact Engineer' section.", []),
    #         "view_submitted_tickets": ("To view submitted tickets, click on the link below.", ["/Tickets/SubmittedTickets"]),
    #         "view_ticket_status": ("Enter your ticket ID to view the status.", []),
    #         "unknown": ("I'm sorry, I didn't understand that. Can you please rephrase?", [])
    #     }

def predict_intents(user_input):
    # Use the trained model to predict the intent
    nlp = spacy.load("Scripts\intent_model")
    doc = nlp(user_input)
    intent = max(doc.cats, key=doc.cats.get)
    similarity = doc.cats[intent]
    print(intent, similarity)
    if similarity >= 0.9:
        response = intent
    else:
        response = "unknown"

    if response == "unknown":
        response = predict_intent_using_gemini_api(user_input)

    # return response and similarity
    return response, similarity