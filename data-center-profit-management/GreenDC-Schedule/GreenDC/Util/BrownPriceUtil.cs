using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Util
{
    public class BrownPriceUtil
    {

        public static List<double> generateBrownPrice(int timeSlots, int slotPerDay, double dayPrice, double nightPrice, int dayBegin)
        {
            List<double> brownPrice = new List<double>(timeSlots);
            double price = dayPrice;
            int index = 0;
            int firstTurnPoint = dayBegin;

            for (int i = 0; i < firstTurnPoint * slotPerDay / 24; i++)
            {
                brownPrice.Add(nightPrice);
                index++;
            }

            while (index < timeSlots)
            {
                // Console.WriteLine("index ={0}", index);
                for (int i = 0; i < slotPerDay / 2 && i < timeSlots - index; i++)
                {
                    brownPrice.Add(price);
                }
                index += slotPerDay / 2;
                price = (price == dayPrice ? nightPrice : dayPrice);
            }

            return brownPrice;
        }
    }
}
