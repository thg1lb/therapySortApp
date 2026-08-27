import random
random.seed(42) # loads random set of data each time

import pandas as pd
from flask import Flask, request, jsonify
from flask_cors import CORS
import sqlite3

DB_PATH = "therapySort.db"

app = Flask(__name__)
CORS(app)

def get_db_connection():
    # row_factory = dictionary (essentially) for easier access to columns by name
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn

@app.route("/", methods=["GET"])
def index():
    return "TherapySort API is running! Try /filter with different parameters.", 200

@app.route("/filter", methods=["GET"])
def filter_therapists():
    try:
        # parameter collection (from unity)
        qualifications = request.args.get("qualifications", "").strip()
        location = request.args.get("location", "").strip()
        availability = request.args.get("availability", "").strip()
        verification = request.args.get("verification", "").strip()
        min_endorsements = request.args.get("endorsements", "").strip()
        keyword = request.args.get("keyword", "").strip()
        limit = request.args.get("limit", "").strip()
        job_title = request.args.get("job_title", "").strip()
        accepting_clients = request.args.get("accepting_clients", "").strip()
        phone = request.args.get("phone", "").strip()

# building sql WHERE clause
        base_query = """
            SELECT
                id,
                name,
                location,
                qualifications,
                job_title,
                availability,
                verification,
                endorsements,
                experience_years,
                sessions_completed,
                description,
                job_title,
                accepting_clients
            FROM therapists
        """

        conditions = []
        values = []

        if qualifications:
            conditions.append("LOWER(qualifications) LIKE ?")
            values.append("%" + qualifications.lower() + "%")

        if location:
            conditions.append("LOWER(location) LIKE ?")
            values.append("%" + location.lower() + "%")

        if availability:
            conditions.append("LOWER(availability) LIKE ?")
            values.append("%" + availability.lower() + "%")

        if verification:
            # exact match (case-insensitive)
            conditions.append("LOWER(verification) = ?")
            values.append(verification.lower())

        if min_endorsements:
            try:
                min_val = int(min_endorsements)
                conditions.append("endorsements >= ?")
                values.append(min_val)
            except ValueError:
                return jsonify({"error": "endorsements must be an integer"}), 400

        if keyword:
            # match keyword in multiple text columns
            conditions.append("""(
                LOWER(description) LIKE ?
                OR LOWER(job_title) LIKE ?
                OR LOWER(qualifications) LIKE ?
            )""")
            kw = "%" + keyword.lower() + "%"
            values.extend([kw, kw, kw])
            
        if job_title:
            conditions.append("LOWER(job_title) LIKE ?")
            values.append("%" + job_title.lower() + "%")

        if accepting_clients:
            conditions.append("LOWER(accepting_clients) = ?")
            values.append(accepting_clients.lower())

        # (optional) WHERE combination
        if conditions:
            base_query += " WHERE " + " AND ".join(conditions)

        # (optional) adding limit
        if limit:
            try:
                limit_val = int(limit)
                base_query += " LIMIT ?"
                values.append(limit_val)
            except ValueError:
                return jsonify({"error": "limit must be an integer"}), 400

        # query execution
        conn = get_db_connection()
        cur = conn.cursor()
        cur.execute(base_query, values)
        rows = cur.fetchall()
        conn.close()

        # conversion into python dict
        results = []
        for r in rows:
            results.append({
                "ID": r["id"],
                "Name": r["name"],
                "Location": r["location"],
                "Qualifications": r["qualifications"],
                "Job_Title": r["job_title"],
                "Availability": r["availability"],
                "Verification": r["verification"],
                "Endorsements": r["endorsements"],
                "Experience_Years": r["experience_years"],
                "Sessions_Completed": r["sessions_completed"],
                "Description": r["description"]
            })

        # error message if nothing: "No therapists found" (displayed in unity)
        return jsonify(results), 200
    
    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == "__main__":
    app.run(host="127.0.0.1", port=5000, debug=True)
    
