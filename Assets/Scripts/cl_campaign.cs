using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Модель кампании, хранящая и предоставляющая методы для генерации кампаний (список уровеней) и уровней (список врагов)
[System.Serializable()]
public class cl_campaign 
{
    public int length = 10;
    public float avgLvlTime = 15f;
    
    //Значения, необходимые для работы сохранений
    public int seed; //Используется для получения воспроизводимых результатов при генерации случайных чисел
    public int[] states; //Массив, хранящий в себе состояние уровней в кампании, где: 0 - уровень недоступен, 1 - уровень открыт, 2 - уровень пройден

    [System.NonSerialized()] public List<cl_level> levels;
    [System.NonSerialized()] public System.Random randObj;

    //Генерирует список уровней, принимает сид из загруженного сохранения
    public void generate(bool fromSeed, int Seed)
    {
        if (fromSeed)
        {
            seed = Seed;
            randObj = new System.Random(Seed);
        } else
        {
            seed = Random.Range(0, 123456);
            states = new int[length];
            randObj = new System.Random(seed);
        }
        levels = new List<cl_level>();
        for (int i = 0; i < length; i++)
        {
            cl_level curlvl = new cl_level();
            if (!fromSeed)
            {
                states[i] = 0;
                if (i == 0) states[i] = 1;
            }

            float time = (70+randObj.Next() % 60f)/100*avgLvlTime;
            float difficulty = i + 1 * (50 + randObj.Next() % 100f) / 100;
            if (difficulty >= 10) difficulty = 9.8f;

            int enemyCount = System.Convert.ToInt32(time * difficulty / 3);
            if (enemyCount <= 0) enemyCount = 3;
            curlvl.generate(time, enemyCount, difficulty, randObj);
            levels.Add(curlvl);
        }
    }


    //Список врагов со следующими характеристиками - время спауна (количество секунд от запуска уровня), тип (0-2 - астероиды, 3 - корабль) и скорость движения
    public class cl_level
    {
        public List<Enemy> level;

        public class Enemy
        {
            public float spawntime;
            public int type;
            public float speed;
        }
        //Генерирует список врагов для кампании, чем выше сложность, тем больше кораблей и выше максимальная скорость объектов
        public void generate(float maxTime, int totalEnemies, float difficulty, System.Random randObj)
        {
            if (level == null)
            {
                level = new List<Enemy>();
            }
            level.Clear();
            for (int i = 0; i < totalEnemies; i++)
            {
                Enemy enemy = new Enemy();
                enemy.spawntime = maxTime / totalEnemies * i;

                enemy.type = System.Convert.ToInt32(randObj.Next() % 3);
                if (randObj.Next()%10f < difficulty) enemy.type = 3;
                enemy.speed = (30+randObj.Next()%70f) % difficulty + 1;
                level.Add(enemy);
            }
        }
    }
}
