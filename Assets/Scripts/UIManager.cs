using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Health UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpSliderText;

    [Header("Message UI")]
    [SerializeField] private int sameMessageCount = 0; // read only
    [SerializeField] private string lastMessage; // read only
    [SerializeField] private bool isMessageHistoryOpen = false; // read only
    [SerializeField] private GameObject messageHistory;
    [SerializeField] private GameObject messageHistoryContent;
    [SerializeField] private GameObject last5MessagesContent;

    public bool IsMessageHistoryOpen { get => isMessageHistoryOpen; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() => AddMessage("Hello and welcome Adventurer !", "#0da2ff");

    public void SetHealthMax(int maxHp)
    {
        hpSlider.maxValue = maxHp;
    }


    public void SetHealth(int hp, int maxHp)
    {
        hpSlider.value = hp;
        hpSliderText.text = $"HP: {hp}/{maxHp}";
    }

    public void OnConnectedToServer()
    {
        messageHistory.SetActive(!messageHistory.activeSelf);
        isMessageHistoryOpen = messageHistory.activeSelf;
    }

    public void AddMessage(string newMessage, string colorHex)
    {
        if (lastMessage == newMessage)
        {
            TextMeshProUGUI messageHistoryLastChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI last5HistoryLastChild = last5MessagesContent.transform.GetChild(last5MessagesContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();

            messageHistoryLastChild.text = $"{newMessage} (x{++sameMessageCount})";
            last5HistoryLastChild.text = $"{newMessage} (x{sameMessageCount})";
            return;
        }
        else if (sameMessageCount > 0)
        {
            sameMessageCount = 0;
        }

        lastMessage = newMessage;

        TextMeshProUGUI messagePrefab = Instantiate(Resources.Load<TextMeshProUGUI>("Message")) as TextMeshProUGUI;
        messagePrefab.text = newMessage;
        messagePrefab.color = GetColorFromHex(colorHex);
        messagePrefab.transform.SetParent(messageHistoryContent.transform, false);

        for (int i = 0; i < last5MessagesContent.transform.childCount; i++)
        {
            if (messageHistoryContent.transform.childCount - 1 < i)
                return;

            TextMeshProUGUI messageHistoryLastChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI last5HistoryLastChild = last5MessagesContent.transform.GetChild(last5MessagesContent.transform.childCount - 1).GetComponent<TextMeshProUGUI>();

            last5HistoryLastChild.text = messageHistoryLastChild.text;
            last5HistoryLastChild.color = messageHistoryLastChild.color;
        }

    }

    private Color GetColorFromHex(string colorHex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(colorHex, out color))
            return color;

        Debug.Log("GetColorFromHex: could not parse color");
        return Color.white;
    }
}
