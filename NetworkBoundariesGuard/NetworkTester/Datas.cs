using System.Runtime.InteropServices;

namespace NetworkTester
{
    [StructLayout(LayoutKind.Sequential)]
    public class Datas
    {
        public float x = 0;

        public float y = 0;

        public Datas(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Datas()
        {
        }

        public override string ToString()
        {
            return $"Datas x = {x} et y = {y}";
        }

        public string GetTypeOfDatas()
        {
            return "POSITIONS3D";
        }
    }
}
