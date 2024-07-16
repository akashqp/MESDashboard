import spacy
import random
from spacy.training import Example
from spacy.util import minibatch, compounding
import json

def train_model():
    # Load spaCy model
    nlp = spacy.load('en_core_web_sm')

    # Load intents from intents.json
    with open("Scripts\intents.json") as file:
        intents = json.load(file)

    # Load last_intents from last_intents.json
    with open("Scripts\last_intents.json") as file:
        last_intents = json.load(file)

    # Check if intents.json is the same as last_intents.json
    if intents == last_intents:
        return

    # Create training data
    training_data = []
    for intent, phrases in intents.items():
        for phrase in phrases:
            training_data.append((phrase.lower(), {"cats": {intent: 1.0}}))

    # Add the text categorizer to the pipeline if it doesn't exist
    if "textcat" not in nlp.pipe_names:
        textcat = nlp.add_pipe("textcat", last=True)
    else:
        textcat = nlp.get_pipe("textcat")

    # Add labels to text classifier
    for intent in intents.keys():
        textcat.add_label(intent)

    # Train the model
    def train_spacy(training_data, n_iter=50):
        random.shuffle(training_data)
        optimizer = nlp.begin_training()
        for epoch in range(n_iter):
            losses = {}
            random.shuffle(training_data)
            batches = minibatch(training_data, size=compounding(4.0, 32.0, 1.001))
            for batch in batches:
                texts, annotations = zip(*batch)
                examples = [Example.from_dict(nlp.make_doc(text), annotation) for text, annotation in zip(texts, annotations)]
                nlp.update(examples, sgd=optimizer, drop=0.5, losses=losses)
            print(f"Losses at iteration {epoch}: {losses}")
        return nlp

    # Train the model
    nlp = train_spacy(training_data)

    # Save the trained model
    nlp.to_disk("Scripts\intent_model")

    # Save intents to last_intents.json
    with open("Scripts\last_intents.json", "w") as file:
        json.dump(intents, file) 


# # Load the trained model
# nlp = spacy.load("intent_model")