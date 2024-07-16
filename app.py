from flask import Flask, request, jsonify
import spacy
from spacy.util import minibatch, compounding
import random
from spacy.training import Example
from Scripts.analytics import RejectionReasons, monthly_rejection_rates, correlation_analysis, feature_importance, monthly_rejection_reasons
from Scripts.trainintentmodel import train_model
from Scripts.Gemini import call_gemini_api
from Scripts.chatbot import predict_intents


app = Flask(__name__)


# Rejection reasons for the Pipe Production Reports
@app.route('/rejection_reasons', methods=['GET'])
def rejection_reasons():
    rejection_reasons = RejectionReasons()
    response = jsonify(rejection_reasons.to_dict())
    response.headers.add("Content-Type", "application/json")
    print(response.data)
    return response


# Monthly rejection rates for the Pipe Production Reports
@app.route('/monthly_rejection_rates', methods=['GET'])
def Monthly_rejection_rates():
    rejection_rates = monthly_rejection_rates()
    # Convert Period objects to strings
    rejection_rates_dict = {str(k): v for k, v in rejection_rates.to_dict().items()}
    response = jsonify(rejection_rates_dict)
    response.headers.add("Content-Type", "application/json")
    return response


# Correlation analysis between chemical composition and rejection
@app.route('/correlation_analysis', methods=['GET'])
def Correlation_analysis():
    correlation = correlation_analysis()
    response = jsonify(correlation.to_dict())
    response.headers.add("Content-Type", "application/json")
    return response


# Feature importance analysis for pipe rejection
@app.route('/feature_importance', methods=['GET'])
def Feature_importance():
    importance = feature_importance()
    response = jsonify(importance.to_dict())
    response.headers.add("Content-Type", "application/json")
    return response


@app.route('/monthly_rejection_reasons/<months>', methods=['GET'])
def Monthly_rejection_reasons(months):
    rejection_reasons = monthly_rejection_reasons(months)
    labels = []
    data = []
    details = {}
    composition = {}

    for key, count in rejection_reasons['monthly_rejection_reasons'].items():
        month, faultType = key
        # Change Month to Word Format such as Jan, Feb, Mar
        month = month.strftime('%b')

        if month in labels:
            data[labels.index(month)] += count
        else:
            labels.append(month)
            data.append(count)

        if month not in details:
            details[month] = []
        details[month].append({"faultType": faultType, "count": count})
    
    # print("Monthly Composition Analysis: ", rejection_reasons['monthly_composition_analysis'])
    

    for key, values in rejection_reasons['monthly_composition_analysis'].items():
        element = key

        for month, value in values.items():
            month = month.strftime('%b')
            if value == 0:
                continue
            print(month, element, value)

            if month not in composition:
                composition[month] = {}

            if element in composition[month]:
                composition[month][element].append(value)
            else:
                composition[month][element] = [value]

    response_dict = {"labels": labels, "data": data, "details": details, "composition": composition}
    print(response_dict)
    response = jsonify(response_dict)
    response.headers.add("Content-Type", "application/json")

    return response


# Predict the intent of a user input
@app.route('/predict_intent', methods=['POST'])
def predict_intent():
    data = request.json
    user_input = data.get('text')

    # Handle empty input
    if not user_input:
        return jsonify({"error": "No text provided"}), 400

    intent, similarity = predict_intents(user_input)
    
    response = jsonify({"intent": intent, "similarity": similarity})
    response.headers.add("Content-Type", "application/json")
    return response

# Generate text using the Gemini API
@app.route('/generate_text', methods=['POST'])
def generate_text():
    data = request.json
    input_text = data.get('input_text')
    ChatHistory = data.get('chatHistory')
    output_text = call_gemini_api(input_text, ChatHistory)
    response = jsonify({"output_text": output_text})
    response.headers.add("Content-Type", "application/json")
    return response

if __name__ == '__main__':
    train_model()
    with app.app_context():
        Monthly_rejection_reasons(12)
    app.run(port=5000)