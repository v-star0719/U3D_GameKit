namespace GameKit
{
    public class ItemConfig
    {
        public string name;
        public int gid;
        public int lv;
        public int shipScaleType;
        public EmItemType itemType;

        public override string ToString()
        {
            if(shipScaleType >= 0)
            {
                string scale = "未知";
                if(shipScaleType == 0) scale = "小";
                if(shipScaleType == 1) scale = "中";
                if(shipScaleType == 2) scale = "大";
                return $"({scale}) lv.{lv}";
            }
            else
            {
                return $"{name} lv.{lv}";
            }
        }
    }
}
