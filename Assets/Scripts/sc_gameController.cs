using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class sc_gameController : MonoBehaviour
{
    public sc_UI ui;
    public cl_campaign curCampaign;
    public sc_playController playField;

    int savescount = 0;
    public int curSaveIndex=4;
    public int curLvlOpen;
    string savePath;

    void Start()
    {
        savePath = Application.dataPath + "/saves";
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
        checkSaves();
    }

    public void checkSaves()
    {
        string[] files = Directory.GetFiles(savePath);
        savescount = Directory.GetFiles(savePath).Length;
    }

    public void newGame()
    {
        if (savescount < 3)
        {
            curCampaign = new cl_campaign();
            curCampaign.generate(false, 0);
            ui.openCampaign(curCampaign);
            curSaveIndex = savescount+1;
            saveGame(curCampaign,true,4);
        } else
        {
            InitLoadMenu();
        }
    }

    public void InitLoadMenu()
    {
        string[] files = Directory.GetFiles(savePath);
        ui.showSaveSelector(files);
    }

    public void removeSave(int index)
    {
        File.Delete(savePath + "/save" + index + ".xml");
        renameSaves();
        InitLoadMenu();
        if(savescount!=0)savescount--;
    }

    public void renameSaves()
    {
        string[] files = Directory.GetFiles(savePath);
        for (int i = 0; i < files.Length; i++)
        {
            File.Move(files[i], savePath + "/save" + (i + 1) + ".xml");
        }
    }

    public void loadSave(int index)
    {
        Stream stream = File.Open(savePath + "/save" + index + ".xml", FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        curSaveIndex = index;
        cl_campaign loaded = (cl_campaign)formatter.Deserialize(stream);
        stream.Close();
        loaded.generate(true, loaded.seed);
        curCampaign = loaded;
        ui.openCampaign(curCampaign);
    }

    public void saveGame(cl_campaign camp,bool newSave, int index)
    {
        if (newSave)
        {
            index = savescount + 1;
            savescount++;
        }
        Stream stream = File.Open(savePath + "/save" + index + ".xml", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, camp);
        stream.Close();
    }

    public void openLevel(int index)
    {
        curLvlOpen = index;

        ui.switchToGameView(true);
        ui.updateProgress(0);
        ui.displayLives(3);

        playField.lives = 3;
        playField.cleanField();
        playField.playLevel(curCampaign.levels[index]);
    }

    public void newProgress(int prg)
    {
        ui.updateProgress(prg);
        if (prg == 100)
        {
            curCampaign.states[curLvlOpen] = 2;
            if (curLvlOpen != curCampaign.levels.Count - 1)
            {
                if (curCampaign.states[curLvlOpen + 1] == 0)
                {
                    curCampaign.states[curLvlOpen + 1] = 1;
                }
            }
            saveGame(curCampaign, false, curSaveIndex);
            ui.openCampaign(curCampaign);
        }
    }

    public void newHealth(int lives)
    {
        ui.displayLives(lives);
        if (lives == 0)
        {
            Debug.LogWarning("GameOver!");
            ui.openCampaign(curCampaign);
        }
    }
}
