using System;

namespace RetinaNetworking
{
    [Serializable]
    public class Data
    {
        public int datumA;
        public float datumB;
        public string datumC;
        public enumSample datumD;

        public Data(int _datumA, float _datumB, string _datumC, enumSample _datumD)
        {
            datumA = _datumA;
            datumB = _datumB;
            datumC = _datumC;
            datumD = _datumD;
        }

        public Data()
        {
            datumA = 0;
            datumB = 0;
            datumC = null;
            datumD = 0;
        }
    }

    [Serializable]
    public enum enumSample
    {
        bob,
        charlie,
        zed
    }
}

