using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Data
{
    [System.Serializable]
    public class GameData
    {

        public CarSetting CarSetting { get; set; }
        public float TimeFinish { get; set; }

        public GameData()
        {
            CarSetting = new CarSetting();
            CarSetting.maxRpm = 10000;
            CarSetting.breakingForce = 1000000;
            CarSetting.gears = new float[5];
            CarSetting.gears[0] = 2;
            CarSetting.gears[1] = 1.7f;
            CarSetting.gears[2] = 1.3f;
            CarSetting.gears[3] = 0.9f;
            CarSetting.gears[4] = -2f;
            CarSetting.gearNum = 0;

            TimeFinish = 0;
        }
    }

    public class CarSetting
    {
        public float maxRpm { get; set; }
        public float breakingForce { get; set; }
        public float[] gears { get; set; }
        public int gearNum { get; set; }
    }
}
