using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeManager : MonoBehaviour
{
    [Tooltip("The list of every panel the manager could spawn.")]
    [SerializeField] private GameObject[] masterPanelList;

    [Tooltip("The shade for the upgrade menu.")]
    [SerializeField] private GameObject shade;

    [Tooltip("The button to reroll the current selection.")]
    [SerializeField] private GameObject rerollButton;

    [Tooltip("The sound that plays after getting an upgrade.")]
    [SerializeField] protected AudioSource upgradeSound;

    const int PANEL_AMOUNT = 3;
    private List<GameObject> activePanels;
    public static UpgradeManager instance;
    private bool upgradeMenuOpen = false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Ensure that every panel has their data.
        foreach (GameObject panel in masterPanelList)
        {
            UpgradeComponent upgrade = panel.GetComponent<UpgradeComponent>();
            upgrade.Start();
            upgrade.ReloadUpgrade();
            panel.SetActive(false);
        }

        activePanels = new List<GameObject>();

        SelectPanels();
        CloseUpgradeMenu();
    }

    private void SelectPanels()
    {
        activePanels.Clear();

        foreach (GameObject panel in masterPanelList)
        {
            activePanels.Add(panel);
            panel.SetActive(true);
        }

        while (activePanels.Count > PANEL_AMOUNT)
        {
            int removeIndex = Random.Range(0, activePanels.Count);
            activePanels[removeIndex].SetActive(false);
            activePanels.RemoveAt(removeIndex);
        }

        ArrangePanels();
    }

    public void ArrangePanels()
    {
        for (int i = 0; i < PANEL_AMOUNT; i++)
        {
            activePanels[i].GetComponent<RectTransform>().anchoredPosition = new Vector2((600f * i) - 600f, 0);
        }
    }

    public void OpenUpgradeMenu()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.MuffleMusic();
        }

        upgradeSound.Play();
        shade.SetActive(true);
        rerollButton.SetActive(true);
        SelectPanels();
        foreach (GameObject panel in activePanels)
        {
            panel.SetActive(true);
        }
        EventSystem.current.SetSelectedGameObject(activePanels[0].GetComponent<UpgradeComponent>().GetButton());

        if (PauseManager.instance != null)
        {
            PauseManager.instance.FreezeTime();
            PauseManager.instance.DisablePausing();
        }
    }

    public void CloseUpgradeMenu()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.UnmuffleMusic();
        }
        
        shade.SetActive(false);
        rerollButton.SetActive(false);
        foreach (GameObject panel in activePanels)
        {
            panel.GetComponent<UpgradeComponent>().ReloadUpgrade();
            panel.SetActive(false);
        }
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1f;

        if (PauseManager.instance != null)
        {
            PauseManager.instance.UnfreezeTime();
            PauseManager.instance.EnablePausing();
        }
    }

    public bool IsUpgradeMenuOpen()
    {
        return upgradeMenuOpen;
    }

    public void RerollUpgrades()
    {
        SelectPanels();
        foreach (GameObject panel in activePanels)
        {
            panel.GetComponent<UpgradeComponent>().ReloadUpgrade();
        }
        EventSystem.current.SetSelectedGameObject(activePanels[0].GetComponent<UpgradeComponent>().GetButton());
    }
}
