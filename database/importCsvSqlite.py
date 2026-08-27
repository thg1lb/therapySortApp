import sqlite3
import pandas as pd

DB_PATH = "therapySort.db"
CSV_PATH = "mental_health_dataset.csv"

df = pd.read_csv(CSV_PATH)

df = df.rename(columns={
    "Name": "name",
    "Location": "location",
    "Qualifications": "qualifications",
    "Verification": "verification",
    "Endorsements": "endorsements",
    "Availability": "availability",
    "Experience_Years": "experience_years",
    "Sessions_Completed": "sessions_completed",
    "Description": "description",
    "Job_Title": "job_title",
    "Phone": "phone",
    "Accepting_Clients": "accepting_clients"
})

conn = sqlite3.connect(DB_PATH)
cursor = conn.cursor()

for _, row in df.iterrows():
    cursor.execute("""
        INSERT INTO therapists (name, location, qualifications, verification, 
        endorsements, availability, experience_years, sessions_completed, 
        description, job_title, phone, accepting_clients)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (
        row.get("name", ""),
        row.get("location", ""),
        row.get("qualifications", ""),
        row.get("verification", ""),
        int(row.get("endorsements", 0)) if pd.notna(row.get("endorsements")) else None,
        row.get("availability", ""),
        int(row.get("experience_years", 0)) if pd.notna(row.get("experience_years")) else None,
        row.get("sessions_completed", ""),
        row.get("description", ""),
        row.get("job_title", ""),
        row.get("phone", ""),
        row.get("accepting_clients", "")
    ))

conn.commit()
conn.close()
print("Data imported into therapists table from CSV.")

