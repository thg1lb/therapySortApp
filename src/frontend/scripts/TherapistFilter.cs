using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Therapist
{

    // defining all of the possible parameters
    public int ID;
    public string Name;
    public string Location;
    public string Qualifications;
    public string Verification;
    public string Endorsements;
    public string Availability;
    public int Experience_Years;
    public string Sessions_Completed;
    public string Description;
}

[System.Serializable]
public class TherapistList
{
    public Therapist[] therapists;
}

public class TherapistFilter : MonoBehaviour
{

    [Header("Backend")]
    [SerializeField] private string backendUrl = "http://127.0.0.1:5000/filter";


    [Header("Input Fields")]
    public TMP_InputField locationInput;
    public TMP_InputField qualificationsInput;
    public TMP_InputField jobTitleInput;
    public TMP_InputField availabilityInput;
    public Toggle verificationToggle;
    public TMP_InputField endorsementsMinInput;
    public TMP_InputField experienceYearsInput;
    public TMP_InputField sessionsCompletedInput;
    public TMP_InputField limitInput;
    public TMP_InputField keywordInput;

    [Header("UI References")]
    public Button searchButton;
    public TextMeshProUGUI resultsText;
    public GameObject therapistCardPrefab;
    public Transform resultsContainer;


    // Start is called before the first frame update
    void Start()
    {
        if (searchButton != null)
        {
            searchButton.onClick.AddListener(SearchTherapists);
        }

        SearchTherapists(); // displays all therapists on start
    }

    public void SearchTherapists()
    {
        StartCoroutine(GetFilteredTherapists());
    }

    // retrieves filtered therapists from given url
    IEnumerator GetFilteredTherapists()
    {
        resultsText.text = "Searching ...";

        string url = BuildFilterUrl();
        // Debug.Log("Requesting " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            bool error = request.result != UnityWebRequest.Result.Success;

            if (error)
            {
                resultsText.text = "Error: " + request.error;
                yield break;
            }

            string jsonResponse = request.downloadHandler.text;

            DisplayResults(jsonResponse);

        }

    }

    private string BuildFilterUrl()
    {
        string url = backendUrl + "?";
        Debug.Log("Requesting URL: " + url);

        bool firstParam = true;

        void AddParam(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!firstParam)
                {
                    url += "&";
                }

                url += key + "=" + UnityWebRequest.EscapeURL(value);
                firstParam = false;
            }

        }

        void AddToggleParam(string key, Toggle toggle, string valueIfOn)
        {
            if (toggle != null && toggle.isOn)
            {
                AddParam(key, valueIfOn);
            }
        }

        AddParam("location", locationInput != null ? locationInput.text : "");
        AddParam("availability", availabilityInput != null ? availabilityInput.text : "");
        AddParam("qualifications", qualificationsInput != null ? qualificationsInput.text : "");
        AddToggleParam("verification",  verificationToggle, "Verified");
        AddParam("endorsements", endorsementsMinInput != null ? endorsementsMinInput.text : ""); // min rating
        AddParam("limit", limitInput != null ? limitInput.text : "");
        AddParam("keyword", keywordInput != null ? keywordInput.text : "");

        return url;

    }

    private void DisplayResults(string jsonResponse)
    {

        // Wrap raw array (from Flask) into an object so JsonUtility can parse it
        string wrappedJson = "{\"therapists\":" + jsonResponse + "}";

        TherapistList therapistList;

        try
        {
            therapistList = JsonUtility.FromJson<TherapistList>(wrappedJson);
            Debug.Log("Parsed therapists: " + (therapistList?.therapists == null ? "null" : therapistList.therapists.Length.ToString()));
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
            resultsText.text = "Sorry, couldn't read results.";
            return;
        }

        // Clear previous cards
        foreach (Transform child in resultsContainer)
        {
            Destroy(child.gameObject);
        }

        // No matches edge case
        if (therapistList == null || therapistList.therapists == null || therapistList.therapists.Length == 0)
        {
            resultsText.text = "No therapists matched your filters.";
            return;
        }

        resultsText.text = therapistList.therapists.Length + " result(s)";

        // Make cards
        foreach (Therapist t in therapistList.therapists)
        {
            GameObject card = Instantiate(therapistCardPrefab, resultsContainer);

            // Find text elements on the prefab
            // Assumes children named exactly like this:
            TMP_Text nameField = card.transform.Find("NameText")?.GetComponent<TMP_Text>();
            TMP_Text qualField = card.transform.Find("QualificationsText")?.GetComponent<TMP_Text>();
            TMP_Text locField = card.transform.Find("LocationText")?.GetComponent<TMP_Text>();
            TMP_Text availField = card.transform.Find("AvailabilityText")?.GetComponent<TMP_Text>();
            TMP_Text metaField = card.transform.Find("MetaText")?.GetComponent<TMP_Text>();

            // Fill them with clean labels (so your card reads like a real profile instead of raw CSV dump)
            if (nameField != null)
                nameField.text = SafeText(t.Name, "Name not listed");

            if (qualField != null)
                qualField.text = SafeText(t.Qualifications, "Qualifications not listed");

            if (locField != null)
                locField.text = SafeText(t.Location, "Location not listed");

            if (availField != null)
                availField.text = "Availability: " + SafeText(t.Availability, "Unknown");

            if (metaField != null)
            {
                // Build a compact summary line the client can scan fast
                string endorsePart = string.IsNullOrEmpty(t.Endorsements) ? "No rating" : "Rating: " + t.Endorsements;
                string verifyPart = string.IsNullOrEmpty(t.Verification) ? "Unverified" : t.Verification;
                metaField.text = endorsePart + "  •  " + verifyPart;

            }
        }
    }

    private string SafeText(string s, string fallback)
    {
        if (string.IsNullOrEmpty(s)) return fallback;
        return s;
    }

}