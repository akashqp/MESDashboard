import requests
import json

import google.generativeai as genai

def call_gemini_api(input_text, ChatHistory):

    genai.configure(api_key="AIzaSyClVaX8736twd_u2FaSxqmusD0sqpd6I9w")

    # Create the model
    generation_config = {
    "temperature": 1,
    "top_p": 0.95,
    "top_k": 64,
    "max_output_tokens": 8192,
    "response_mime_type": "text/plain",
    }

    model = genai.GenerativeModel(
    model_name="gemini-1.5-flash",
    generation_config=generation_config,

    )
    # chat_history = [
    #     {
    #         "parts": [
    #             {
    #                 "text": "Hello, I need some help with navigating the MES"
    #             }
    #         ],
    #         "role": "user"
    #     },
    #     {
    #         "parts": [
    #             {
    #                 "text": "Hi there! How can I help you with navigating the MES? Is there a specific area you're having trouble with, or are you just looking for some general guidance?"
    #             }
    #         ],
    #         "role": "model"
    #     }
    # ]

    if ChatHistory is None or ChatHistory == "[]":
        chat_history = []
    else:
        # Convert ChatHistory to json
        ChatHistory = json.loads(ChatHistory)

        chat_history = []
        for item in ChatHistory:
            chat_history.append({
                "parts": [
                    {
                        "text": item["user"]
                    }
                ],
                "role": "user"
            })
            chat_history.append({
                "parts": [
                    {
                        "text": item["bot"]
                    }
                ],
                "role": "model"
            })
        print(chat_history)

    chat_session = model.start_chat(
        history=chat_history
    )

    response = chat_session.send_message(input_text)
    print(input_text)
    print(response.text)

    return response.text

def predict_intent_using_gemini_api(user_input):
    genai.configure(api_key="AIzaSyClVaX8736twd_u2FaSxqmusD0sqpd6I9w")

    # Create the model
    generation_config = {
    "temperature": 1,
    "top_p": 0.95,
    "top_k": 64,
    "max_output_tokens": 8192,
    "response_mime_type": "text/plain",
    }

    model = genai.GenerativeModel(
    model_name="gemini-1.5-flash",
    generation_config=generation_config,
    )

    chat_session = model.start_chat()

    response = chat_session.send_message(user_input)

    # Load intents from intents.json
    with open("Scripts\intents.json") as file:
        intents = json.load(file)

    # Predict the intent of the user input
    response = chat_session.send_message("Find the intent of the user input(Only return the intent string, no other word should be there in the response):\nAlso previously in chathistory if the bot has asked for name from user and current response is a name, then return intent as 'greeting' otherwise return appropriate intent from below keeping in mind user_input" +  "\nUser Input: " + user_input +  "\nIntents: " + str(intents))
    intent = response.text
    # Remove white space from the intent
    intent = intent.strip()
    print("Intent: ", intent.strip())

    return intent