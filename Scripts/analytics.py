import pyodbc
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler, MinMaxScaler
from sklearn.metrics import classification_report, confusion_matrix

def load_data():
    # Configure the database connection
    conn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER=AKASHPC\\SQLEXPRESS;DATABASE=MES;Trusted_Connection=yes;')

    # Query data from the PipeProductionReports table
    pipe_production_df = pd.read_sql("SELECT * FROM PipeProductionReports", conn)

    # Query data from the LadleCompositionData table
    ladle_composition_df = pd.read_sql("SELECT * FROM LadleCompositionData", conn)

    conn.close()

    # Merge DataFrames based on LadleNo
    merged_df = pd.merge(pipe_production_df, ladle_composition_df, on='LadleNo')

    return merged_df


# Rejection Reasons: Analyze the reasons for pipe rejections to identify common issues
def RejectionReasons():
    # Load the data
    merged_df = load_data()

    # # Merge DataFrames based on LadleNo
    # merged_df = pd.merge(pipe_production_df, ladle_composition_df, on='LadleNo')

    # Count the number of rejections for each reason
    rejection_reasons = merged_df[merged_df['State'] == 'Reject']['Reason'].value_counts()
    return rejection_reasons


# Trend Analysis: Analyze the trend of rejections over time to identify any patterns or seasonal variations
def monthly_rejection_rates():
    # Load the data
    merged_df = load_data()

    # Convert ReportDate to datetime and extract month and year
    merged_df['ReportDate'] = pd.to_datetime(merged_df['ReportDate'])
    merged_df['YearMonth'] = merged_df['ReportDate'].dt.to_period('M')

    # Group by YearMonth and calculate rejection rate
    monthly_rejection_rate = merged_df.groupby('YearMonth')['State'].apply(lambda x: (x == 'Reject').mean())

    return monthly_rejection_rate


# Correlation Analysis: Correlation between Chemical Composition and Rejectioon
def correlation_analysis():
    # Load the data
    merged_df = load_data()

    # Encode the 'State' column as binary (1 for Reject, 0 for Ok)
    merged_df['RejectFlag'] = merged_df['State'].apply(lambda x: 1 if x == 'Reject' else 0)

    # Drop unnecessary columns
    merged_df = merged_df.drop(['Id', 'ReportDate', 'State', 'LadleNo', 'PipeNo', 'Shift', 'MachineId', 'OperatorId', 'Reason',
                                 'SubReason', 'LadleCompositionDataLadleNo'], axis=1)

    # Calculate the correlation matrix
    correlation_matrix = merged_df.corr()

    # Extract correlation of chemical elements with rejection
    rejection_correlation = correlation_matrix['RejectFlag'].drop(['RejectFlag'])
    print("Rejection Correlations:", rejection_correlation)

    return rejection_correlation


# Feature Impotance Analysis: Identify the most important features(Chemical Compositions) for pipe rejection
def feature_importance():
    # Load the data
    merged_df = load_data()

    # Encode the 'State' column as binary (1 for Reject, 0 for Ok)
    merged_df['RejectFlag'] = merged_df['State'].apply(lambda x: 1 if x == 'Reject' else 0)

    # Prepare the data
    features = merged_df.drop(columns=['Id', 'PipeNo', 'LadleNo', 'State', 'ReportDate', 'Shift', 'MachineId', 
                                       'OperatorId', 'Reason', 'SubReason', 'RejectFlag', 'LadleCompositionDataLadleNo'])
    target = merged_df['RejectFlag']

    # Split into training and testing sets
    X_train, X_test, y_train, y_test = train_test_split(features, target, test_size=0.3, random_state=42)


    # Standardize the features
    scaler = StandardScaler()
    X_train_scaled = scaler.fit_transform(X_train)
    X_test_scaled = scaler.transform(X_test)

    print("Model Training Started")

    # Train a Random Forest Classifier
    model = RandomForestClassifier(n_estimators=100, random_state=42)
    model.fit(X_train_scaled, y_train)

    print("Model Training Completed")
    print("Model Score: ", model.score(X_test_scaled, y_test))

    # Make predictions on the test set
    y_pred = model.predict(X_test_scaled)

    # Evaluate the model
    print("Confusion Matrix:")
    print(confusion_matrix(y_test, y_pred))
    print("\nClassification Report:")
    print(classification_report(y_test, y_pred))


    # Get feature importances
    feature_importances = pd.Series(model.feature_importances_, index=features.columns)
    # print("Feature Importances:", feature_importances)

    return feature_importances

    # Plot the feature importances
    plt.figure(figsize=(12, 6))
    feature_importances.sort_values(ascending=False).plot(kind='bar')
    plt.title('Feature Importance for Pipe Rejection Prediction')
    plt.ylabel('Importance')
    plt.grid(True)
    plt.show()


# Monthly Rejection Reasons: Analyze the reasons for pipe rejections on a monthly basis
# Returns the count of rejections for each reason for each months within the specified period and analyze the reason for rejection based on the chemical composition of each month
def monthly_rejection_reasons(months):
    # Convert months to integer
    months = int(months)

    # Load the data
    merged_df = load_data()

    # Convert ReportDate to datetime and extract month and year
    merged_df['ReportDate'] = pd.to_datetime(merged_df['ReportDate'])
    merged_df['YearMonth'] = merged_df['ReportDate'].dt.to_period('M')

    # Filter data for the specified number of months
    start_month = merged_df['YearMonth'].max() - months
    filtered_df = merged_df[merged_df['YearMonth'] >= start_month]

    # Count the number of rejections for each reason for each month
    monthly_rejection_reasons = filtered_df[filtered_df['State'] == 'Reject'].groupby(['YearMonth', 'Reason'])['State'].count()

    # Analyze the reason for rejection based on the chemical composition of each month
    # For each month, calculate the average chemical composition
    chemical_columns = ['C', 'Si', 'Mn', 'P', 'S', 'Ti', 'Mg', 'V', 'Cr', 'Cu', 'Sn', 'Pb', 'Mo', 'Al', 'Ni', 'Co', 'Nb', 'W', 'As', 'Bi', 'Ca', 'Ce', 'Sb', 'B', 'N', 'Zn', 'Fe', 'FMg']

    monthly_composition_analysis = filtered_df[filtered_df['State'] == 'Reject'].groupby(['YearMonth'])[chemical_columns].mean()

    print("Monthly Composition Analysis:" , monthly_composition_analysis)

    # # Do a Feature Importance Analysis for each month and remove the least important features
    # remaining_features = set(chemical_columns)
    # for month in monthly_composition_analysis.index:
    #     month_data = filtered_df[filtered_df['YearMonth'] == month]
    #     if len(month_data) > 1:  # Check if there is more than one sample
    #         features = month_data[chemical_columns]
    #         target = month_data['State']
        
    #         # Split into training and testing sets
    #         X_train, _, y_train, _ = train_test_split(features, target, test_size=0.25, random_state=42)
        
    #         # Standardize the features
    #         scaler = StandardScaler()
    #         X_train_scaled = scaler.fit_transform(X_train)
        
    #         # Train a Random Forest Classifier
    #         model = RandomForestClassifier(n_estimators=100, random_state=42)
    #         model.fit(X_train_scaled, y_train)
        
    #         # Get feature importances
    #         feature_importances = pd.Series(model.feature_importances_, index=features.columns)

    #         print("Feature Importances for Month", month, ":", feature_importances)
        
    #         # Remove the least important features
    #         least_important_features = feature_importances[feature_importances < 0.04].index
    #         print("Least Important Features for Month", month, ":", least_important_features)

    #         # Ensure only existing columns are dropped
    #         remaining_features -= set(least_important_features)
    #         print("Remaining Features:", remaining_features)

    # # Filter the monthly_composition_analysis to keep only the remaining features
    # monthly_composition_analysis = monthly_composition_analysis[list(remaining_features)]
    # print("Monthly Composition Analysis after dropping:", monthly_composition_analysis) 

    # Do a Feature Importance Analysis for each month and set the least importance features for the respective month to 0
    for month in monthly_composition_analysis.index:
        month_data = filtered_df[filtered_df['YearMonth'] == month]
        if len(month_data) > 1:  # Check if there is more than one sample
            features = month_data[chemical_columns]
            target = month_data['State']
        
            # Split into training and testing sets
            X_train, _, y_train, _ = train_test_split(features, target, test_size=0.25, random_state=42)
        
            # Standardize the features
            scaler = StandardScaler()
            X_train_scaled = scaler.fit_transform(X_train)
        
            # Train a Random Forest Classifier
            model = RandomForestClassifier(n_estimators=100, random_state=42)
            model.fit(X_train_scaled, y_train)
        
            # Get feature importances
            feature_importances = pd.Series(model.feature_importances_, index=features.columns)

            print("Feature Importances for Month", month, ":", feature_importances)
        
            # Set the least important features to 0
            least_important_features = feature_importances[feature_importances < 0.05].index
            if len(least_important_features) == len(feature_importances):
                # If the number of least important features is less than 5 then don't set any feature to 0
                if len(least_important_features) < 5:
                    continue
                # Select all the columns except the one with highest importance if all are least important
                least_important_features = feature_importances[:-1].index

            monthly_composition_analysis.loc[month, least_important_features] = 0
 
    print("Monthly Composition Analysis after setting least important features to 0:", monthly_composition_analysis)
            
            
    return {'monthly_rejection_reasons': monthly_rejection_reasons, 'monthly_composition_analysis': monthly_composition_analysis}

# monthly_rejection_reasons('3')
# RejectionReasons()
# feature_importance()
# monthly_rejection_rates()
# correlation_analysis()

