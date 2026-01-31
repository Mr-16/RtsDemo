using Assets.Codes.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codes.Game
{
    public static class GameHelper
    {
        public static Vec2I WorldToGrid(Vector3 worldPos, float cellSize)
        {
            int x = (int)MathF.Floor((worldPos.x + GameConstant.WorldWidth / 2) / cellSize);
            int y = (int)MathF.Floor((worldPos.z + GameConstant.WorldHeight / 2) / cellSize);
            x = (int)Math.Clamp(x, 0, GameConstant.WorldWidth - 1);
            y = (int)Math.Clamp(y, 0, GameConstant.WorldHeight - 1);
            return new Vec2I(x, y);
        }
    }
}
