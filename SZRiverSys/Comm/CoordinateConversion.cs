using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace SZRiverSys.Comm
{

    public class Point
    {
        private double x;
        private double y;


        public double getX()
        {
            return x;
        }


        public void setX(double x)
        {
            this.x = x;
        }


        public double getY()
        {
            return y;
        }


        public void setY(double y)
        {
            this.y = y;
        }
    }

    public class CoordinateConversion
    {
        /**
        * 经纬度转墨卡托
        * @param LonLat 经纬度坐标
        * @return*/
        public static Point lonLatToMercator(Point LonLat)
        {
            Point mercator = new Point();
            double x = (LonLat.getX() * 20037508.342789 / 180);
            double y = (Math.Log(Math.Tan((90 + LonLat.getY()) * Math.PI / 360)) / (Math.PI / 180));
            y = (double)(y * 20037508.342789 / 180);
            mercator.setX(x);
            mercator.setY(y);
            return mercator;
        }

        public static double[] lonLatToMercator(double x, double y)
        {
            double x0 = (x * 20037508.342789 / 180);
            double y0 = (Math.Log(Math.Tan((90 + y) * Math.PI / 360)) / (Math.PI / 180));
            y0 = (double)(y0 * 20037508.342789 / 180);
            return new double[] { x0, y0};
        }
        public static double[] lonLatToMercator(double[] coord)
        {
            return lonLatToMercator(coord[0], coord[1]);
        }
        /**
        * 墨卡托转经纬度
        * @param mercator 墨卡托坐标
        * @return*/
        public static Point mercatorToLonLat(Point mercator)
        {
            Point lonlat = new Point();
            double x = (mercator.getX() / 20037508.342789 * 180);
            double y = (mercator.getY() / 20037508.342789 * 180);
            y = (double)(180 / Math.PI * (2 * Math.Atan(Math.Exp(y * Math.PI / 180)) - Math.PI / 2));
            lonlat.setX(x);
            lonlat.setY(y);
            return lonlat;
        }


        public static double[] mercatorToLonLat(double x, double y)
        {
            Point lonlat = new Point();
            double x0 = (x / 20037508.342789 * 180);
            double y0 = (y / 20037508.342789 * 180);
            y0 = (double)(180 / Math.PI * (2 * Math.Atan(Math.Exp(y0 * Math.PI / 180)) - Math.PI / 2));
            lonlat.setX(x);
            lonlat.setY(y);
            return new double[] { x0, y0 };
        }
        public static double[] mercatorToLonLat(double[] coord)
        {
            return mercatorToLonLat(coord[0], coord[1]);
        }
    }
}

