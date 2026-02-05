using Assets.Codes.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codes.Game
{
    public static class GlobalHelper
    {
        public static Vec2I WorldToGrid(Vector3 worldPos, float cellSize)
        {
            int x = (int)MathF.Floor((worldPos.x + GlobalConstant.WorldWidth / 2) / cellSize);
            int y = (int)MathF.Floor((worldPos.z + GlobalConstant.WorldHeight / 2) / cellSize);
            x = Math.Clamp(x, 0, (int)MathF.Ceiling(GlobalConstant.WorldWidth / cellSize) - 1);
            y = Math.Clamp(y, 0, (int)MathF.Ceiling(GlobalConstant.WorldHeight / cellSize) - 1);
            return new Vec2I(x, y);
        }
    }
}
