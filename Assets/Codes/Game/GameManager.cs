using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Codes.Game
{
    public class GameManager
    {
        private static GameManager instance = new GameManager();
        public static GameManager Instance => instance;
        private GameManager() { }
        public List<Unit> UnitList = new List<Unit>();

        public bool IsUiDragging = false;
    }
}
