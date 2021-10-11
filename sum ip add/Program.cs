using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace sum_ip_add
{
    class Program
    {
        private static List<byte[]> _IpAddres = new List<byte[]>();
        private const int _numberOfBytesInIp = 4;
        private const int _numberOfBitsInByte = 8;

        struct SumIp
        {
            public byte[] sumIp { get; }
            public byte[] subnetMask { get; }

            public int rememberIndexDifferingByte { get; }
            public int rememberIndexDifferingBit { get; }

            public SumIp(byte[] sumIp, byte[] subnetMask, int rememberIndexDifferingByte = -1, int rememberIndexDifferingBit = -1)
            {
                this.sumIp = new byte[_numberOfBytesInIp];
                Array.Copy(sumIp, this.sumIp, _numberOfBytesInIp);

                this.subnetMask = new byte[_numberOfBytesInIp];
                Array.Copy(subnetMask, this.subnetMask, _numberOfBytesInIp);


                this.rememberIndexDifferingByte = rememberIndexDifferingByte;
                this.rememberIndexDifferingBit = rememberIndexDifferingBit;
            }
        }

        

        static void Main(string[] args)
        {
            string getInformation;
            byte[] tepmIp;

            WriteStartInf();

            while (true)
            {
                getInformation = Console.ReadLine();

                switch (getInformation)
                {
                    case "ex":
                        break;
                    case "clean":
                        CleanIpList();
                        continue;
                    case "sum":
                        if (_IpAddres.Count <= 1)
                        {
                            Console.WriteLine("Ви не ввели жодного або лише один ip");
                            Console.ReadLine();
                            CleanIpList();
                            continue;
                        }else
                            break;
                    default:
                        try
                        {
                            tepmIp = getInformation.Split('.').Select(Byte.Parse).ToArray();
                            if (tepmIp.Length != _numberOfBytesInIp) throw new Exception();
                            _IpAddres.Add(new byte[_numberOfBytesInIp]);
                            Array.Copy(tepmIp, _IpAddres.Last(), tepmIp.Length);
                        }
                        catch
                        {
                            Console.WriteLine("Ви ввели не коректне ip");
                            Console.ReadLine();
                            WriteStartInf();
                            ShowIP(_IpAddres);
                        }
                        

                        //_IpAddres.Add()
                        continue;
                }

                if (getInformation == "ex")
                {
                    break;
                }

                PrintValues(GetSumIp(), _IpAddres);
                Console.WriteLine("\n\nДля продовження натиснiть Enter, для виходу впишiть ex");

                switch (Console.ReadLine())
                {
                    case "ex":
                        break;

                    default:
                        CleanIpList();
                        continue;
                }

                break;
            }

            //PrintValues(GetSumIp(), _IpAddres);
        }

        private static SumIp GetSumIp()
        {
            int rememberIndexDifferingByte = -1;// номер байта, що відрізняється 
            int rememberIndexDifferingBit = -1;// номер біта, що відрізняється у відрізняющихся байтів
            byte[] tempSumIp = new byte[_numberOfBytesInIp];
            byte[] differingBytes;
            BitArray bitArray;
            BitArray bitSum = new BitArray(_numberOfBitsInByte, false);// сумарний байт

            for (int ip_i = 0; ip_i < _numberOfBytesInIp; ip_i++)
            {
                for (int listIp_j = 1; listIp_j < _IpAddres.Count(); listIp_j++)
                {
                    if (_IpAddres[listIp_j][ip_i] != _IpAddres[listIp_j - 1][ip_i])
                    {
                        rememberIndexDifferingByte = ip_i;
                        break;
                    }                       
                }

                if (rememberIndexDifferingByte != -1)
                    break;
            }

            if (rememberIndexDifferingByte == -1)
            {
                return new SumIp(_IpAddres.First(), new byte[] { 254, 254, 254, 254 });
            }

            differingBytes = new byte[_IpAddres.Count];

            for (int i = 0; i < _IpAddres.Count; i++)
            {
                differingBytes[i] = _IpAddres[i][rememberIndexDifferingByte];
            }

            bitArray = new BitArray(differingBytes);

            for (int colum = 0; colum < _numberOfBitsInByte; colum++)//7
            {
                for (int row = 1; row < bitArray.Length / _numberOfBitsInByte; row++)
                {
                    if(bitArray[colum + (_numberOfBitsInByte * row)] != bitArray[colum + (_numberOfBitsInByte * (row - 1))])
                    {
                        rememberIndexDifferingBit = colum;
                        break;
                    }
                }
            }

            for (int i = 0; i < _numberOfBitsInByte; i++)
            {
                if(i > rememberIndexDifferingBit)
                    bitSum[i] = bitArray[i];
            }

            Array.Copy(_IpAddres.First(), tempSumIp, tempSumIp.Length);

            //tempSumIp = _IpAddres.First();

            for (int i = tempSumIp.Length - 1; i > rememberIndexDifferingByte; i--)
            {
                tempSumIp[i] = 0;
            }
            
            tempSumIp[rememberIndexDifferingByte] = BitArrayToByteArray(bitSum)[0];

            byte[] subnetMask = GetSubnetMask(rememberIndexDifferingByte, rememberIndexDifferingBit);

            return new SumIp(tempSumIp, subnetMask, rememberIndexDifferingByte, rememberIndexDifferingBit);
        }


        private static byte[] GetSubnetMask(int rememberIndexDifferingByte, int rememberIndexDifferingBit)
        {
            BitArray bitArray = new BitArray(32, false);

            for (int i = 0; i < rememberIndexDifferingByte * 8; i++)
            {
                bitArray[i] = true;
            }

            for (int i = rememberIndexDifferingByte * 8 + (rememberIndexDifferingBit + 1); i <= (rememberIndexDifferingByte * 8 + rememberIndexDifferingBit) + (8 - (rememberIndexDifferingBit + 1)); i++)
            {
                bitArray[i] = true;
            }

            return BitArrayToByteArray(bitArray);
        }

        private static void WriteStartInf()
        {
            Console.Clear();
            Console.WriteLine("Ведiть ip адреси для суммаризацiї");
            Console.WriteLine("Для очищення списту введiть clean, а для проведення суммаризiцiї sum, для виходу впишiть ex");
        }

        private static void CleanIpList()
        {
            _IpAddres.Clear();
            Console.Clear();
            WriteStartInf();
        }

        private static void ShowIP<T>(IEnumerable<IEnumerable<T>> iterator)
        {
            foreach (var items in iterator)
            {

                for (int i = 0; i < items.Count(); i++)
                {
                    if (i != items.Count() - 1)
                        Console.Write($"{items.ElementAt(i)}.");
                    else
                        Console.Write($"{items.ElementAt(i)}");
                }
                Console.WriteLine();
            }
        }

        private static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }


        private static void PrintValues(SumIp sumInfo, List<byte[]> ipAddres)
        {

            int numberOfNull = (8 - (sumInfo.rememberIndexDifferingBit + 1)) + (sumInfo.rememberIndexDifferingByte) * 8;
            BitArray bitArray;

            Console.WriteLine("\t\t");
            foreach (var ip in ipAddres)
            {

                ShowIPAndBit(ip, numberOfNull);
                /*for (int i = 0; i < ip.Count(); i++)
                {
                    if (i != ip.Count() - 1)
                        Console.Write($"{ip[i]}.");
                    else
                        Console.Write($"{ip[i]}\t\t");
                }

                for (int i = 0; i < ip.Length; i++)
                {
                    bitArray = new BitArray(new byte[] { ip[i] });

                    for (int j = bitArray.Length - 1; j >= 0; j--)
                    {
                        if((i * _numberOfBitsInByte) + (_numberOfBitsInByte - j) == numberOfNull)
                            Console.Write(bitArray[j] ? "1|" : "0|");
                        else if (j == 0 && i != ip.Length - 1)
                            Console.Write(bitArray[j] ? "1. " : "0. ");
                        else
                            Console.Write(bitArray[j] ? "1 " : "0 ");

                    }
                    
                }*/
                

            }

            for (int i = 0; i < 120; i++)
            {
                Console.Write("_");
            }

            ShowIPAndBit(sumInfo.sumIp, numberOfNull);
            ShowIPAndBit(sumInfo.subnetMask, numberOfNull);

            /*for (int i = 0; i < sumInfo.sumIp.Length; i++)
            {
                if (i != sumInfo.sumIp.Length - 1)
                    Console.Write($"{sumInfo.sumIp[i]}.");
                else
                    Console.Write($"{sumInfo.sumIp[i]}\t\t");
            }

            for (int i = 0; i < sumInfo.sumIp.Length; i++)
            {
                bitArray = new BitArray(new byte[] { sumInfo.sumIp[i] });

                for (int j = bitArray.Length - 1; j >= 0; j--)
                {
                    if ((i * _numberOfBitsInByte) + (_numberOfBitsInByte - j) == numberOfNull)
                        Console.Write(bitArray[j] ? "1|" : "0|");
                    else if (j == 0 && i != sumInfo.sumIp.Length - 1)
                        Console.Write(bitArray[j] ? "1. " : "0. ");
                    else
                        Console.Write(bitArray[j] ? "1 " : "0 ");

                }

            }*/

        }

        private static void ShowIPAndBit<T>(IEnumerable<T> mas, int numberOfNull)
        {
            BitArray bitArray;

            for (int i = 0; i < mas.Count(); i++)
            {
                if (i != mas.Count() - 1)
                    Console.Write($"{mas.ElementAt(i)}.");
                else
                    Console.Write($"{mas.ElementAt(i)}\t\t");
            }

            for (int i = 0; i < mas.Count(); i++)
            {
                bitArray = new BitArray(new byte[] { Convert.ToByte(mas.ElementAt(i)) });

                for (int j = bitArray.Length - 1; j >= 0; j--)
                {
                    if ((i * _numberOfBitsInByte) + (_numberOfBitsInByte - j) == numberOfNull)
                        Console.Write(bitArray[j] ? "1|" : "0|");
                    else if (j == 0 && i != mas.Count() - 1)
                        Console.Write(bitArray[j] ? "1. " : "0. ");
                    else
                        Console.Write(bitArray[j] ? "1 " : "0 ");

                }

            }
            Console.WriteLine();
        }

    }
}
