using UdonSharp;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.Image;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
public class ActivityItem : UdonSharpBehaviour
{
    [SerializeField] private ActivityItem ActivitydetailItem;
    [SerializeField] private ActivityCalendarPanel activitycalendar;
    [SerializeField] private GameObject Recent;
    [SerializeField] private GameObject Longterm;
    private int _id = -1;
    // 定义 Id 属性
    public int Id
    {
        get { return _id; }
        set
        {
            if (_id != value)
            {
                _id = value;
                OnIdChanged();
            }
        }
    }
    // 定义 Id 属性
    public Text[] year;
    public Text[] date;
    public Text[] time;
    public Text[] week;
    public Text[] starttime;
    public Text[] endtime;
    public Text[] title;
    public Text[] brief;
    public Text[] startdate;
    public Text[] enddate;
    public Text[] startyear;
    public Text[] endyear;
    public Text[] initiator;
    public Text[] join;
    public Text[] detail;
    public InputField[] joincopy;
    void Start()
    {
        year = FindTextComponentsInChildren("Year");
        date = FindTextComponentsInChildren("Date");
        time = FindTextComponentsInChildren("Time");
        week = FindTextComponentsInChildren("Week");
        starttime = FindTextComponentsInChildren("StartTime");
        endtime = FindTextComponentsInChildren("EndTime");
        title = FindTextComponentsInChildren("Title");
        brief = FindTextComponentsInChildren("Brief");
        startdate = FindTextComponentsInChildren("StartDate");
        enddate = FindTextComponentsInChildren("EndDate");
        startyear = FindTextComponentsInChildren("StartYear");
        endyear = FindTextComponentsInChildren("EndYear");
        initiator = FindTextComponentsInChildren("Initiator");
        join = FindTextComponentsInChildren("Join");
        detail = FindTextComponentsInChildren("Detail");
        joincopy = FindInputFieldComponentsInChildren("JoinCopy");
        gameObject.SetActive(false);
    }
    private Text[] FindTextComponentsInChildren(string name)
    {
        Text[] foundTextComponents = new Text[0];
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                Text textComponent = child.GetComponent<Text>();
                if (textComponent != null)
                {
                    Text[] newArray = new Text[foundTextComponents.Length + 1];
                    for (int i = 0; i < foundTextComponents.Length; i++)
                    {
                        newArray[i] = foundTextComponents[i];
                    }
                    newArray[foundTextComponents.Length] = textComponent;
                    foundTextComponents = newArray;
                }
            }
            else
            {
                Text[] childTextComponents = child.GetComponentsInChildren<Text>(true); // 包括未启用的物体
                foreach (Text textComponent in childTextComponents)
                {
                    if (textComponent.name == name)
                    {
                        Text[] newArray = new Text[foundTextComponents.Length + 1];
                        for (int i = 0; i < foundTextComponents.Length; i++)
                        {
                            newArray[i] = foundTextComponents[i];
                        }
                        newArray[foundTextComponents.Length] = textComponent;
                        foundTextComponents = newArray;
                    }
                }
            }
        }
        return foundTextComponents;
    }
    private InputField[] FindInputFieldComponentsInChildren(string name)
    {
        InputField[] foundInputFieldComponents = new InputField[0];
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                InputField inputFieldComponent = child.GetComponent<InputField>();
                if (inputFieldComponent != null)
                {
                    InputField[] newArray = new InputField[foundInputFieldComponents.Length + 1];
                    for (int i = 0; i < foundInputFieldComponents.Length; i++)
                    {
                        newArray[i] = foundInputFieldComponents[i];
                    }
                    newArray[foundInputFieldComponents.Length] = inputFieldComponent;
                    foundInputFieldComponents = newArray;
                }
            }
            else
            {
                InputField[] childInputFieldComponents = child.GetComponentsInChildren<InputField>(true); // 包括未启用的物体
                foreach (InputField inputFieldComponent in childInputFieldComponents)
                {
                    if (inputFieldComponent.name == name)
                    {
                        InputField[] newArray = new InputField[foundInputFieldComponents.Length + 1];
                        for (int i = 0; i < foundInputFieldComponents.Length; i++)
                        {
                            newArray[i] = foundInputFieldComponents[i];
                        }
                        newArray[foundInputFieldComponents.Length] = inputFieldComponent;
                        foundInputFieldComponents = newArray;
                    }
                }
            }
        }
        return foundInputFieldComponents;
    }
    public void OnIdChanged()
    {
        Debug.Log("OnIdChanged被调用");
        if (activitycalendar._activitys.TryGetValue(Id, TokenType.DataDictionary, out var v))
        {
            if (v.DataDictionary.TryGetValue("year", TokenType.String, out var yearvalue) && year != null)
            {
                foreach (Text textComponent in year)
                {
                    textComponent.text = yearvalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("date", TokenType.String, out var datevalue) && date != null)
            {
                foreach (Text textComponent in date)
                {
                    textComponent.text = datevalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("time", TokenType.String, out var timevalue) && time != null)
            {
                foreach (Text textComponent in time)
                {
                    textComponent.text = timevalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("week", TokenType.String, out var weekvalue) && week != null)
            {
                foreach (Text textComponent in week)
                {
                    textComponent.text = weekvalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("startdate", TokenType.String, out var startdatevalue) && startdate != null)
            {
                foreach (Text textComponent in startdate)
                {
                    textComponent.text = startdatevalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("starttime", TokenType.String, out var starttimevalue) && starttime != null)
            {
                foreach (Text textComponent in starttime)
                {
                    textComponent.text = starttimevalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("enddate", TokenType.String, out var enddatevalue) && enddate != null)
            {
                foreach (Text textComponent in enddate)
                {
                    textComponent.text = enddatevalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("endtime", TokenType.String, out var endtimevalue) && endtime != null)
            {
                foreach (Text textComponent in endtime)
                {
                    textComponent.text = endtimevalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("title", TokenType.String, out var titlevalue) && title != null)
            {
                foreach (Text textComponent in title)
                {
                    textComponent.text = titlevalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("brief", TokenType.String, out var briefvalue) && brief != null)
            {
                foreach (Text textComponent in brief)
                {
                    textComponent.text = briefvalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("join", TokenType.String, out var joincopyvalue) && join != null && joincopy != null)
            {
                foreach (Text textComponent in join)
                {
                    textComponent.text = joincopyvalue.String;
                }
                foreach (InputField textComponent in joincopy)
                {
                    textComponent.text = joincopyvalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("startyear", TokenType.String, out var startyearvalue) && startyear != null)
            {
                foreach (Text textComponent in startyear)
                {
                    textComponent.text = startyearvalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("endyear", TokenType.String, out var endyearvalue) && endyear != null)
            {
                foreach (Text textComponent in endyear)
                {
                    textComponent.text = endyearvalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("initiator", TokenType.String, out var initiatorvalue) && initiator != null)
            {
                foreach (Text textComponent in initiator)
                {
                    textComponent.text = initiatorvalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("detail", TokenType.String, out var deatailvalue) && detail != null)
            {
                foreach (Text textComponent in detail)
                {
                    textComponent.text = deatailvalue.String;
                }
            }
            if (v.DataDictionary.TryGetValue("type", TokenType.String, out var typevalue))
            {
                if (Recent != null && Longterm != null)
                {
                    if (typevalue.String == "0")
                    {
                        Recent.SetActive(true);
                        Longterm.SetActive(false);
                    }
                    else
                    {
                        Recent.SetActive(false);
                        Longterm.SetActive(true);
                    }
                }
            }
            Debug.Log(Id + "有数据开启item");
            gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(Id + "无数据关闭item");
            gameObject.SetActive(false);
        }
    }
    public void ShowActivityDetail()
    {
        ActivitydetailItem.Id = Id;
    }
    public void SearchTag()
    {
        Toggle tagToggle = GetComponent<Toggle>();
        Text tag = GetComponentInChildren<Text>();
        if (tagToggle.isOn)
        {
            // 在这里编写 Toggle 被选中时触发的操作
            Debug.Log(tag.text + "被选中");
            activitycalendar.SearchTag(tag.text);
        }
    }
}
