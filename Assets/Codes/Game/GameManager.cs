using Assets.Codes.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codes.Game
{
    public class GameManager : MonoBehaviour
    {

        private static GameManager instance;
        public static GameManager Instance => instance;
        private GameManager() { }
        public List<Unit> UnitList = new List<Unit>();
        public GameObject BdGrid;
        public SpatialHash2D<MonoBehaviour> unitSh = new SpatialHash2D<MonoBehaviour>(10);

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            BdGrid.SetActive(false);
        }
        private void Update()
        {
            
            List<MonoBehaviour> unitList = GameManager.Instance.unitSh.Query(new Vector2(UnitList[0].transform.position.x, UnitList[0].transform.position.z), 20);
            Debug.Log(unitList.Count);
        }
    }
}
