using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private bool isMenuOpen = false; // read only

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

    [Header("Inventory UI")]
    [SerializeField] private bool isInventoryOpen = false; // read only
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryContent;

    [Header("Drop Menu UI")]
    [SerializeField] private bool isDropMenuOpen = false; // read only
    [SerializeField] private GameObject dropMenu;
    [SerializeField] private GameObject dropMenuContent;

    public bool IsMenuOpen { get => isMenuOpen; }
    public bool IsMessageHistoryOpen { get => isMessageHistoryOpen; }
    public bool IsInventoryOpen { get => isInventoryOpen; }
    public bool IsDropMenuOpen { get => IsDropMenuOpen; }

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

    public void ToggleMessageHistory()
    {
        messageHistory.SetActive(!messageHistory.activeSelf);
        isMessageHistoryOpen = messageHistory.activeSelf;
    }

    public void ToggleInventory(Actor actor = null)
    {
        inventory.SetActive(!inventory.activeSelf);
        isMenuOpen = isInventoryOpen = inventory.activeSelf;

        if (isMenuOpen) UpdateMenu(actor, inventoryContent);
    }

    public void ToggleDropMenu(Actor actor = null)
    {
        dropMenu.SetActive(!dropMenu.activeSelf);
        isMenuOpen = isDropMenuOpen = dropMenu.activeSelf;

        if (isMenuOpen) UpdateMenu(actor, inventoryContent);
    }

    public void ToggleMenu()
    {
        if (IsMenuOpen)
        {
            isMenuOpen = !isMenuOpen;

            if (isMessageHistoryOpen) ToggleMessageHistory();

            if (isInventoryOpen) ToggleInventory();

            if (isDropMenuOpen) ToggleDropMenu();
        }
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

            TextMeshProUGUI messageHistoryLastChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI last5HistoryLastChild = last5MessagesContent.transform.GetChild(last5MessagesContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();

            last5HistoryLastChild.text = messageHistoryLastChild.text;
            last5HistoryLastChild.color = messageHistoryLastChild.color;
        }
    }

    private Color GetColorFromHex(string colorHex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(colorHex, out color))
            return color;

        Debug.LogError("GetColorFromHex: could not parse color");
        return Color.white;
    }

    private void UpdateMenu(Actor actor, GameObject menuContent)
    {
        for (int resetNum = 0; resetNum < menuContent.transform.childCount; resetNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(resetNum).gameObject;
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            menuContentChild.GetComponent<Button>().onClick.RemoveAllListeners();
            menuContentChild.SetActive(false);
        }

        char c = 'a';
        for (int itemNum = 0; itemNum < actor.Inventory.Items.Count; itemNum++)
        {
            GameObject menuContentChild = menuContent.transform.GetChild(itemNum).gameObject;
            Item item = actor.Inventory.Items[itemNum];
            menuContentChild.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"({c++}) {item.name}";
            menuContentChild.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (menuContent == inventoryContent)
                    Action.UseAction(actor, item);
                else if (menuContent == dropMenuContent)
                    Action.DropAction(actor, item);

                UpdateMenu(actor, menuContent);
            });
            menuContentChild.SetActive(true);
        }

        eventSystem.SetSelectedGameObject(menuContent.transform.GetChild(0).gameObject);
    }

}
