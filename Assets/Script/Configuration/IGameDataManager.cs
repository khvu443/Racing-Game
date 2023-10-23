using Assets.Script.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Configuration
{
    public interface IGameDataManager
    {
        void LoadData(GameData data);
        void SaveData(ref GameData data);
    }
}
