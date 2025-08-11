using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Dto.Geometry;

namespace ToriGeneration.Core.Helpers
{
    public class PointRotationHelper
    {
        public Point RotatePoint(Point point, Point center, Point rotation)
        {
            // Перенос точки относительно центра
            double x = point.X - center.X;
            double y = point.Y - center.Y;
            double z = point.Z - center.Z;

            // Вращение вокруг оси Z
            double cosZ = Math.Cos(rotation.Z);
            double sinZ = Math.Sin(rotation.Z);
            double xZ = (x * cosZ) - (y * sinZ);
            double yZ = (x * sinZ) + (y * cosZ);

            // Вращение вокруг оси Y
            double cosY = Math.Cos(rotation.Y);
            double sinY = Math.Sin(rotation.Y);
            double xY = (xZ * cosY) + (z * sinY);
            double zY = (z * cosY) - (xZ * sinY);

            // Вращение вокруг оси X
            double cosX = Math.Cos(rotation.X);
            double sinX = Math.Sin(rotation.X);
            double yX = (yZ * cosX) - (zY * sinX);
            double zX = (yZ * sinX) + (zY * cosX);

            // Возвращаем точку в центр
            var result = new Point
            {
                X = xY + center.X,
                Y = yX + center.Y,
                Z = zX + center.Z
            };

            return result;
        }
    }
}
