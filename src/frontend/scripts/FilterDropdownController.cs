using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FilterDropdownController : MonoBehaviour
{
    [Header("Dropdown")]
    public TMP_Dropdown filterDropdown;

    [Header("Filter Groups (parents of label + input)")]
    public GameObject locationGroup;
    public GameObject availabilityGroup;
    public GameObject qualificationsGroup;
    public GameObject verificationGroup;
    public GameObject endorsementsGroup;
    public GameObject keywordGroup;
    public GameObject limitGroup;

    private Dictionary<int, GameObject> optionToGroup;

    private void Awake()
    {
        // build options for dropdown :)
        if (filterDropdown != null)
        {
            filterDropdown.ClearOptions();
            var options = new List<string>
            {
                "Location",
                "Availability",
                "Qualifications",
                "Verification",
                "Min Endorsements",
                "Keyword",
                "Limit"
            };

            filterDropdown.AddOptions(options);
            filterDropdown.onValueChanged.AddListener(OnFilterChanged);

        }

        optionToGroup = new Dictionary<int, GameObject>
        {
            { 0, locationGroup },
            { 1, availabilityGroup },
            { 2, qualificationsGroup },
            { 3, verificationGroup },
            { 4, endorsementsGroup },
            { 5, keywordGroup },
            { 6, limitGroup }
        };

        OnFilterChanged(0);
    }

    private void OnFilterChanged(int index)
    {

        SetGroupActive(locationGroup, false);
        SetGroupActive(availabilityGroup, false);
        SetGroupActive(qualificationsGroup, false);
        SetGroupActive(verificationGroup, false);
        SetGroupActive(endorsementsGroup, false);
        SetGroupActive(keywordGroup, false);
        SetGroupActive(limitGroup, false);

        if (optionToGroup.TryGetValue(index, out GameObject group) && group != null)
        {
            group.SetActive(true);
        }

    }

    private void SetGroupActive(GameObject group, bool active)
    {

        if (group != null)
            group.SetActive(active);

    }


}







