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
public class ActivityCalendarPanel : UdonSharpBehaviour
{
    [Header("图片轮换时间间隔")]
    [SerializeField] private float LoadDuration = 5f;
    [Header("活动信息URL")]
    [SerializeField] private VRCUrl url;
    [Header("活动图片URL")]
    [SerializeField] private VRCUrl imageurl;
    [SerializeField] private Renderer materialRend;
    [SerializeField] private RectTransform ActivityContent;
    [SerializeField] private Transform TagListContent = null;
    [SerializeField] private Text timenow;
    [SerializeField] private Text Inform;
    [SerializeField] private ScrollRect scollview;
    [SerializeField] private float offsetY;
    [SerializeField] private Toggle[] PageButton;
    [SerializeField] private RawImage[] rawImages;
    private float nextLoadTime = 0f;
    private Texture2D downloadedTexture;
    private VRCImageDownloader imageDownloader;
    private IUdonEventReceiver udonEventReceiver;
    private float itemhight;
    [HideInInspector] public DataList _activitys;
    DataList _taglist;
    DataDictionary _json;
    void Start()
    {
        itemhight = ActivityContent.GetChild(0).GetComponent<RectTransform>().rect.height;
        materialRend = GetComponent<Renderer>();
        downloadedTexture = new Texture2D(2040, 1020);
        imageDownloader = new VRCImageDownloader();
        udonEventReceiver = (IUdonEventReceiver)this;
        timenow.text = DateTime.Now.ToString("yyyy-MM-dd");
        SendCustomEventDelayedSeconds(nameof(Download), 5);
        nextLoadTime = Time.time;
    }
    public void Download()
    {
        var texInfo = new TextureInfo();
        texInfo.GenerateMipMaps = true;
        imageDownloader.DownloadImage(imageurl, materialRend.material, udonEventReceiver, texInfo);
        VRCStringDownloader.LoadUrl(url, udonEventReceiver);
    }
    void Update()
    {
        if (Time.time - nextLoadTime <= LoadDuration) return;
        float x = scollview.normalizedPosition.x;
        float increment = .125f;
        x += increment;
        if (x > 1f)
        {
            x = 0f; // 归零
        }
        scollview.normalizedPosition = new Vector2(x, 0);
        ChangeButtonColor();
        nextLoadTime = Time.time;
    }
    public void UpdateContentheight()
    {
        Debug.Log("_activitys.Count" + _activitys.Count);
        ActivityContent.sizeDelta = new Vector2(ActivityContent.sizeDelta.x, _activitys.Count * (offsetY + itemhight) + offsetY);
        Debug.Log("ActivityContent.sizeDelta" + ActivityContent.sizeDelta.y);
    }
    public void ChangeButtonColor()
    {
        float value = scollview.normalizedPosition.x;
        for (int i = 0; i < PageButton.Length; i++)
        {
            if (value >= (float)i / PageButton.Length && value <= ((float)(i + 1) / PageButton.Length) || value < 0f && i == 0 || Mathf.FloorToInt(value) == 1 && i == PageButton.Length - 1)
            {
                PageButton[i].isOn = true;
            }
        }
    }
    public void GenerateTagList()
    {
        Debug.Log("在读取Tag列表");
        TagListContent.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < TagListContent.childCount; i++)
        {

            if (_taglist.TryGetValue(i, out var v))
            {
                Debug.Log($"[<color=#ff70ab>TagList</color>] 读取Tag列表成功 索引{i}");
                Text tagText = TagListContent.GetChild(i + 1).Find("Label").GetComponentInChildren<Text>();
                tagText.text = v.String;
                TagListContent.GetChild(i + 1).gameObject.SetActive(true);
            }
            else
            {
                Debug.Log($"[<color=#ff70ab>TagList</color>] 读取Tag列表失败 索引{i}");
                if (i >= TagListContent.childCount - 1) break;
                TagListContent.GetChild(i + 1).gameObject.SetActive(false);
            }
        }
    }
    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        string ApiBase = result.Result;
        Debug.Log($"[<color=#ff70ab>StringLoad</color>] 下载字符串成功 内容为 " + ApiBase);
        if (!VRCJson.TryDeserializeFromJson(result.Result, out var json))
        {
            Debug.Log($"[<color=#ff70ab>StringtoJson</color>] 解析json字符串失败. {json.ToString()}");
            return;
        }
        Debug.Log($"json" + json.TokenType);
        if (json.TokenType != TokenType.DataDictionary) return;
        _json = json.DataDictionary;
        if (_json.TryGetValue("Activity", out var ActivityValue))
        {
            Debug.Log($"Activityalue" + ActivityValue.TokenType);
            _activitys = ActivityValue.DataList;
        }
        if (_json.TryGetValue("TagList", out var TagListValue))
        {
            Debug.Log($"TagListValue" + TagListValue.TokenType);
            _taglist = TagListValue.DataList;
        }
        if (_json.TryGetValue("inform", out var informValue))
        {
            if (Inform != null)
            {
                Inform.text = informValue.String;
            }
        }
        if (TagListContent != null)
        {
            GenerateTagList();
        }
        ItemRefresh();
        UpdateContentheight();
        ActivityContentValueChange();
    }
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.Log($"[<color=#ff70ab>StringLoad</color>] 下载字符串失败. ");
    }
    public override void OnImageLoadSuccess(IVRCImageDownload result)
    {
        downloadedTexture = result.Result;
        foreach (var image in rawImages)
        {
            image.texture = downloadedTexture;
        }
    }
    public override void OnImageLoadError(IVRCImageDownload result)
    {
        Debug.Log($"Image not loaded: {result.Error.ToString()}: {result.ErrorMessage}.");
    }
    public void Refresh()
    {
        var texInfo = new TextureInfo();
        texInfo.GenerateMipMaps = true;
        imageDownloader.DownloadImage(imageurl, materialRend.material, udonEventReceiver, texInfo);
        VRCStringDownloader.LoadUrl(url, udonEventReceiver);
    }
    public void SearchTag(string tag)
    {
        Debug.Log(tag + "li被选中");
        if (tag == "全部") tag = "";
        if (_json.TryGetValue("Activity" + tag, out var ActivityValue))
        {
            _activitys = ActivityValue.DataList;
            Debug.Log($"Activityalue" + tag + ActivityValue.TokenType + $"ActivityalueCount" + _activitys.Count);
        }

        ItemRefresh();
        UpdateContentheight();
        ActivityContentValueChange();

    }
    public void ItemRefresh()
    {
        for (int i = 0; i < ActivityContent.childCount; i++)
        {
            ActivityContent.GetChild(i).GetComponent<ActivityItem>().OnIdChanged();
            Debug.Log(ActivityContent.GetChild(i).GetComponent<ActivityItem>().Id);
        }
    }
    public void ActivityContentValueChange()
    {
        var positionY = ActivityContent.anchoredPosition.y;
        positionY = positionY < 0 ? 0 : positionY;
        int remainder = Mathf.FloorToInt(positionY / (itemhight + offsetY)) % 6;
        remainder = remainder < 0 ? 0 : remainder;
        ActivityItemIdChange(Mathf.FloorToInt(positionY / (itemhight + offsetY)), remainder);
    }
    public void ActivityItemIdChange(int id, int start_i)
    {
        for (int i = start_i; i < ActivityContent.childCount; i++)
        {
            ActivityContent.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -id * (offsetY + itemhight));
            Debug.Log(i + "个item的高度为" + ActivityContent.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y);
            ActivityContent.GetChild(i).GetComponent<ActivityItem>().Id = id;
            id++;
        }
        if (start_i != 0)
        {
            for (int i = 0; i < start_i; i++)
            {
                ActivityContent.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -id * (offsetY + itemhight));
                Debug.Log(i + "个item的高度为" + ActivityContent.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y);
                ActivityContent.GetChild(i).GetComponent<ActivityItem>().Id = id;
                id++;
            }
        }
    }
}
