import sqlite3
import os

DB_PATH = "therapySort.db"


print("Current working directory:", os.getcwd())
print("DB will be created at:", os.path.abspath(DB_PATH))

conn = sqlite3.connect(DB_PATH)
print("Connected to SQLite")


cur = conn.cursor()

cur.execute("DROP TABLE IF EXISTS therapists;")
print("Dropped old table (if existed)")

cur.execute("""

CREATE TABLE therapists (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    location TEXT,
    qualifications TEXT,
    verification TEXT,
    endorsements INTEGER,
    availability TEXT,
    experience_years INTEGER,
    sessions_completed INTEGER,
    description TEXT,
    job_title TEXT,
    phone TEXT,
    accepting_clients TEXT
);
""")
print("Created therapists table")


conn.commit()
conn.close()

print("Database initialized and therapists table created.")

