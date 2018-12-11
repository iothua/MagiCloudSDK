using System.Collections.Generic;

namespace Chemistry.Data
{
    /// <summary>
    /// 反应数据（单次反应的信息）
    /// </summary>
    public class DI_ReactionInfo
    {
        public class DrugItem
        {
            /// <summary>
            /// 药品名字
            /// </summary>
            private string _name;

            /// <summary>
            /// 系数
            /// </summary>
            private float _coefficient;

            /// <summary>
            /// 药品类型
            /// </summary>
            private EDrugType _drugType;

            private string _chemical;

            public DrugItem(string name, string chemical, float coefficient, EDrugType drugType)
            {
                _name = name;
                _coefficient = coefficient;
                _drugType = drugType;
                _chemical = chemical;
            }

            /// <summary>
            /// 药品名字
            /// </summary>
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Chemical {
                get { return _chemical; }
                set { _chemical = value; }
            }

            /// <summary>
            /// 系数
            /// </summary>
            public float Coefficient
            {
                get { return _coefficient; }
                set { _coefficient = value; }
            }

            /// <summary>
            /// 药品类型
            /// </summary>
            public EDrugType DrugType
            {
                get { return _drugType; }
                set { _drugType = value; }
            }

            public string DrugTypeStr {
                get {
                    string str = string.Empty;

                    switch (_drugType)
                    {
                        case EDrugType.Empty:
                            str = "无";
                            break;
                        case EDrugType.Gas:
                            str = "气体";
                            break;
                        case EDrugType.Liquid:
                            str = "液体";
                            break;
                        case EDrugType.Solid:
                            str = "固体";
                            break;
                        case EDrugType.Solid_Powder:
                            str = "固体粉末";
                            break;
                        case EDrugType.Solution:
                            str = "溶液";
                            break;
                    }

                    return str;
                }
            }

            public override string ToString()
            {
                return Name + "," + Chemical + "," + Coefficient + "," + DrugTypeStr;
            }
        }

        /// <summary>
        /// 反应ID
        /// </summary>
        public string id;

        /// <summary>
        /// 方程式名字
        /// </summary>
        public string equationName;

        /// <summary>
        /// 描述信息
        /// </summary>
        public string describe;

        /// <summary>
        /// 反应物
        /// </summary>
        public List<DrugItem> reactants = new List<DrugItem>();

        /// <summary>
        /// 条件
        /// </summary>
        public List<string> conditions = new List<string>();

        /// <summary>
        /// 产物
        /// </summary>
        public List<DrugItem> products = new List<DrugItem>();

        /// <summary>
        /// 获取到药品列表数据字符串
        /// </summary>
        /// <param name="drugItems"></param>
        /// <returns></returns>
        public string GetDrugItems(List<DrugItem> drugItems)
        {
            string str = string.Empty;

            for (int i = 0; i < drugItems.Count; i++)
            {
                if (i == 0)
                {
                    str = drugItems[i].ToString();
                }
                else
                {
                    str += "+" + drugItems[i].ToString();
                }
            }

            id = str;

            return id;
        }

        /// <summary>
        /// 反应物集合
        /// </summary>
        public string ReactantStr {
            get {
                return GetDrugItems(reactants);
            }
        }

        /// <summary>
        /// 产物集合
        /// </summary>
        public string ProductStr {
            get {
                return GetDrugItems(products);
            }
        }

        /// <summary>
        /// 条件集合
        /// </summary>
        public string ConditionStr {
            get {

                string str = string.Empty;

                for (int i = 0; i < conditions.Count; i++)
                {
                    if (i == 0)
                    {
                        str = conditions[0].ToString();
                    }
                    else
                    {
                        str += "+" + conditions[i].ToString();
                    }
                }

                return str;
            }
        }


    }

}