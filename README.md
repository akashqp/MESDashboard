**AI Features Integration in Manufacturing Execution System (MES)**


**Project Overview**

This project focuses on integrating AI capabilities into a Manufacturing Execution System (MES) to enhance its functionality for the steel pipe-making industry. The integration includes AI-driven features like predictive analytics, a chatbot for user interaction, and a comprehensive ticketing system for issue management.

**Key Features**

AI Integration: Enhanced quality control through machine learning models for predicting pipe rejection rates and equipment failures.
Chatbot: Implemented using Natural Language Processing (NLP) and Gemini for real-time user support.
Data Visualization: Graphical representation of production data to improve decision-making processes.
Ticketing System: Allows users to submit, view, and track tickets for issues during the production process.

**Objectives**

The primary objectives of this project include:

AI Feature Development: Adding AI-driven analytics and predictive maintenance capabilities to the MES.
Chatbot Development: Creating an intuitive chatbot for MES operators, enabling real-time issue reporting and support.
Predictive Analytics: Using machine learning to predict reasons for pipe rejection and optimize the manufacturing process.
Data Visualization: Providing a user-friendly interface for visualizing and analyzing production data.
Ticketing System: Developing a system for managing and tracking issues within the MES.

**Technology Stack**

Backend: .NET (C#)
Machine Learning & AI: Flask (Python), spaCy, Gemini
Frontend: .NET-based user interface
Database: SQL Server
APIs: RESTful APIs for communication between .NET and Flask
Libraries: spaCy for NLP, Matplotlib for visualization, Flask for APIs

**Architecture**

This project involves two core servers:

.NET Server: Handles the core business logic, user interface, and interactions with the MES.
Flask Server: Hosts machine learning models and processes API requests for predictive analytics and intent classification.

**Communication Flow**
User Input: Users interact with the MES via a chatbot or data entry forms.
API Requests: Requests that involve machine learning models are sent from the .NET server to the Flask server.
ML Processing: The Flask server processes requests using AI models and sends back predictions to the .NET server.
Output: Results are displayed to the user via the MES interface.

**Core Functionalities**
1. AI Feature Integration
Predictive Analytics: Models predict reasons for pipe rejection and optimize maintenance schedules.
Quality Control: AI helps identify defect patterns to reduce rejection rates.
2. Chatbot
Real-time Support: Helps MES operators by classifying intents and providing useful responses.
Intent Classification: Classifies user inputs and guides the interaction using NLP techniques.
3. Ticketing System
Submit Tickets: Users can submit tickets detailing production issues.
Track Issues: Users can view and track the status of submitted tickets.
4. Data Visualization
Interactive Dashboards: Present production data in user-friendly formats, enabling better decision-making.
Installation

**To set up the project locally:**

**Clone the repository:**
git clone https://github.com/akashqp/MESDashboard.git

**Install dependencies:**

**For the .NET server:**
dotnet restore

**For the Flask server:**
pip install -r requirements.txt
Run the .NET and Flask servers:

**Start the .NET server:**
dotnet run

**Start the Flask server:**
python app.py

**Usage:**
Navigate to the MES dashboard to interact with the chatbot or view production reports.
Submit tickets via the chatbot for any production issues.
Use the visualization dashboard to monitor production trends and quality control metrics.
