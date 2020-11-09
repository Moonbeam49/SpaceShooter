using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Класс, отвечающий за отрисовку игровых меню и переключения между ними
public class sc_UI : MonoBehaviour
{
    public sc_gameController gameC;

    public GameObject GameUI,GameField, Menu;
    public GameObject[] menuScreens;


    public Image[] lives;
    public Text progress;
    public GameObject LevelButPrefab;
    public Material lineMaterial;

    //Метод, используемый для отрисовки кампании на UI, очищает соответствующее поле, определяет его размеры, создает и расставляет необходимые кнопки, добавляет евент открытия соответствующего уровня при их нажатии,
    //отображает статус уровня, исходя из текущего прогресса, рисует линии между открытыми и пройденными уровнями
    public void openCampaign(cl_campaign curCampaign)
    {
        switchToGameView(false);
        setMenuScreen(2);

        foreach(Transform child in menuScreens[2].transform)
        {
            if (child.name !="Back") Destroy(child.gameObject);
        }

        float maxX = Menu.transform.GetComponent<RectTransform>().rect.width/2 - LevelButPrefab.transform.GetComponent<RectTransform>().rect.width;
        float maxY = Menu.transform.GetComponent<RectTransform>().rect.height/2 - LevelButPrefab.transform.GetComponent<RectTransform>().rect.height;

        float prevX = 0;
        GameObject[] buts = new GameObject[curCampaign.levels.Count];
        System.Random rndObj = new System.Random(curCampaign.seed);

        for (int i = 0; i < curCampaign.levels.Count; i++)
        {
            buts[i] = Instantiate(LevelButPrefab, menuScreens[2].transform);

            if (prevX > 0)
            {
                buts[i].transform.GetComponent<RectTransform>().localPosition = new Vector3(rndObj.Next(System.Convert.ToInt32(-maxX), 0), -maxY + i * (maxY * 2 / curCampaign.levels.Count),0);
                
            } else
            {
                buts[i].transform.GetComponent<RectTransform>().localPosition = new Vector3(rndObj.Next(0, System.Convert.ToInt32(maxX)), -maxY + i * (maxY * 2 / curCampaign.levels.Count),0);
            }

            prevX = buts[i].transform.GetComponent<RectTransform>().localPosition.x;
            buts[i].transform.Find("Text").GetComponent<Text>().text = (i + 1) + "\nlevel";

            int flag = i;
            buts[i].GetComponent<Button>().onClick.AddListener(delegate() { gameC.openLevel(flag); });

            switch (curCampaign.states[i])
            {
                case 0:
                    buts[i].GetComponent<Button>().interactable = false;
                    break;
                case 1:

                    break;
                case 2:
                    buts[i].GetComponent<Image>().color = Color.green;
                    break;
            }

            if (i != 0 && curCampaign.states[i] != 0)
            {
                GameObject lineObj = new GameObject();
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lineObj.transform.parent = menuScreens[2].transform;
                lineObj.transform.position = buts[i - 1].transform.position;
                lr.material = lineMaterial;
                lr.startWidth = 0.2f;
                lr.endWidth = 0.2f;
                lr.SetPositions(new Vector3[] { new Vector3(buts[i-1].transform.position.x, buts[i - 1].transform.position.y,0), new Vector3(buts[i].transform.position.x, buts[i].transform.position.y,0) });
            }
        }
    }

    //Отрысовывает меню загрузки игры, исходя из полученного списка файлов
    public void showSaveSelector(string[] levels)
    {
        setMenuScreen(1);

        for (int i = 0; i < 3; i++)
        {
            if (i < levels.Length)
            {
                GameObject curBut = menuScreens[1].transform.Find("Save" + (i + 1)).gameObject;
                string[] tmpArr = levels[i].Split('/');
                curBut.transform.Find("Text").GetComponent<Text>().text = tmpArr[tmpArr.Length-1].Split('\\')[1];
                curBut.transform.Find("Remove").gameObject.SetActive(true);
                curBut.GetComponent<Button>().interactable = true;
            } else
            {
                GameObject curBut = menuScreens[1].transform.Find("Save" + (i + 1)).gameObject;
                curBut.transform.Find("Text").GetComponent<Text>().text = "SaveFile not found";
                curBut.transform.Find("Remove").gameObject.SetActive(false);
                curBut.GetComponent<Button>().interactable = false;
            }
        }
    }

    //Обновляет информацию о количестве жизней на UI
    public void displayLives(int count)
    {
        for (int i =0; i < lives.Length; i++)
        {
            if (i < count)
            {
                lives[i].enabled = true;
            } else
            {
                lives[i].enabled = false;
            }
        }
    }

    //Обновляет информацию о текущем прогрессе на открытом уровне на UI
    public void updateProgress(int num)
    {
        progress.text = "Progress: " + num + "%";
    }

    //Внутренняя функция, используемая для переключения между меню и игровым экраном
    public void switchToGameView(bool toGame)
    {
        if (toGame)
        {
            Menu.SetActive(false);
            GameUI.SetActive(true);
            GameField.SetActive(true);
        } else
        {
            Menu.SetActive(true);
            GameUI.SetActive(false);
            GameField.SetActive(false);
        }
    }
    //Внутреняя функция, используемая для переключения экранов в меню
    public void setMenuScreen(int screen)
    {
        for (int i = 0; i < menuScreens.Length; i++)
        {
            if (i != screen)
            {
                menuScreens[i].SetActive(false);
            } else
            {
                menuScreens[i].SetActive(true);
            }
        }
    }

}
