using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace creatspacewithoutparameter
{
    class CalCoolingload
    {
        /// <summary>
        /// 新风冷负荷计算
        /// </summary>
        /// <param name="qm">新风量</param>
        /// <param name="ho">室外空气焓值</param>
        /// <param name="hr">室内空气焓值</param>
        /// <returns></returns>
        public double CalFreshAirCoolingLoad(double qm,double ho,double hr)
        {
            double freshaircoolingload = qm*(ho-hr);
            return freshaircoolingload;
        }
        /// <summary>
        /// 人体总冷负荷计算
        /// </summary>
        /// <param name="area">面积</param>
        /// <param name="areapercapita">人均办公面积</param>
        /// <param name="humanload">人体负荷</param>
        /// <returns></returns>
        public double CalHumanCoolingLoad(double area, double areapercapita, double humanload)
        {
            double humancoolingload = area * areapercapita * humanload;
            return humancoolingload;
        }
        /// <summary>
        /// 设备总冷负荷计算
        /// </summary>
        /// <param name="numberoflight">灯具数量</param>
        /// <param name="lightload">灯具散热负荷</param>
        /// <param name="numberofequipment">设备数量</param>
        /// <param name="equipmentload">设备散热负荷</param>
        /// <returns></returns>
        public double CalEquipCoolingLoad(double numberoflight, double lightload, double numberofequipment, double equipmentload)
        {
            double equipcoolingload = numberoflight * lightload + numberofequipment * equipmentload;
            return equipcoolingload;
        }
        /// <summary>
        /// 室内冷负荷计算，输入数组，其中面积与传热系数按数组顺序一一对应
        /// </summary>
        /// <param name="K">传热系数</param>
        /// <param name="area">面积</param>
        /// <param name="outdoortemperature">室外温度</param>
        /// <returns></returns>
        public double CalIndoorCoolingLoad(double[] K, double[] area, double[] outdoortemperature)
        {
            if (K.Length == area.Length)
            {
                throw (new CustomException("输入面积及传热系数数量不同"));
            }
            else
            {
                double maxtemp = outdoortemperature[0];
                for (int i = 0; i < outdoortemperature.Length; i++)
                {
                    if (maxtemp < outdoortemperature[i])
                    {
                        maxtemp = outdoortemperature[i];
                    }
                }
                double n = 0;
                for (int i = 0; i < K.Length; i++)
                {
                    n += K[i] * area[i];
                }
                double indoorcoolingload = n * maxtemp;
                return indoorcoolingload;
            }
        }
        /// <summary>
        /// 总冷负荷计算
        /// </summary>
        /// <param name="freshaircoolingload">新风冷负荷</param>
        /// <param name="humancoolingload">人体总冷负荷</param>
        /// <param name="equipcoolingload">设备总冷负荷</param>
        ///  <param name="indoorcoolingload">室内冷负荷</param>
        /// <returns></returns>
        public double CalTotalCoolingLoad(double freshaircoolingload, double humancoolingload, double equipcoolingload, double indoorcoolingload)
        {
            double totalcoolingload = freshaircoolingload+ humancoolingload+ equipcoolingload+ indoorcoolingload;
            return totalcoolingload;
        }
        /// <summary>
        /// 总热负荷计算
        /// </summary>
        /// <param name="freshaircoolingload">新风冷负荷</param>
        ///  <param name="indoorcoolingload">室内冷负荷</param>
        /// <returns></returns>
        public double CalTotalHeatLoad(double freshaircoolingload,double indoorcoolingload)
        {
            double totalheatload = freshaircoolingload + indoorcoolingload;
            return totalheatload;
        }
        /// <summary>
        /// 自定义异常
        /// </summary>
        /// <returns></returns>
        public class CustomException : ApplicationException
        {
            public CustomException(string message) : base(message)
            {
            }
        }
    }
}
