using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//Основной контроллер приложения, обрабатывает нажатия кнопок UI, отвечает за работу с сохранениями, игровыми меню и текущим состоянием кампании
public class sc_gameController : MonoBehaviour
{
    public sc_UI ui;
    public cl_campaign curCampaign;
    public sc_playController playField;

    int savescount = 0;
    public int curSaveIndex=4;
    public int curLvlOpen;
    string savePath;

    //Проверяет наличие папки для сохранений, создает директорию, если ее нет
    void Start()
    {
        savePath = Application.dataPath + "/saves";
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
    }

    //Метод возвращает количество сейвов в папке, используется при генерации новой кампании
    public void checkSaves()
    {
        savescount = Directory.GetFiles(savePath).Length;
    }

    //Вызывается при нажатии кнопки "New Game", проверяет, не превышено ли максимальное количество сохранений, если нет - генерирует новую кампанию, сохраняет ее и передает на UI для отрисовки
    public void newGame()
    {
        checkSaves();
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

    //Вызывается при нажатии на кнопку "Load Game", собирает названия сохранений в папке и передает их на UI для отрисовки
    public void InitLoadMenu()
    {
        string[] files = Directory.GetFiles(savePath);
        ui.showSaveSelector(files);
    }

    //Вызывается при нажатии на кнопку удаления сохранения, удаляет сейв, после чего запускает обновление информации на экране
    public void removeSave(int index)
    {
        File.Delete(savePath + "/save" + index + ".xml");
        renameSaves();
        InitLoadMenu();
        if(savescount!=0)savescount--;
    }

    //Вызывается после удаления одного из сейвов, восстанавливает порядок в именах сейвов (save1, save2...)
    public void renameSaves()
    {
        string[] files = Directory.GetFiles(savePath);
        for (int i = 0; i < files.Length; i++)
        {
            File.Move(files[i], savePath + "/save" + (i + 1) + ".xml");
        }
    }

    //Вызывается при нажатии на кнопку загрузки определенного сохранения, открывает файл, генерирует кампанию из полученного сида и передает ее на UI для отрисовки
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

    //Вызывается при создании, либо обновлении статуса кампании, сохраняет ее в указанный файл
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

    //Вызывается при выборе уровня в окне кампании, переключает UI на игровое поле, запускает контроллер игрового поля
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

    //Вызывается при уничтожении врага на игровом поле, проверяет, уничтожены ли все враги на уровне, если да - обновляет статус кампании и сохраняет ее
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

    //Вызывается при получении урона игроком, если была потрачена последняя жизнь, возвращает на экран кампании
    public void newHealth(int lives)
    {
        ui.displayLives(lives);
        if (lives == 0)
        {
            ui.openCampaign(curCampaign);
        }
    }
}
