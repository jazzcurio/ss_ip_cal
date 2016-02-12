using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLSM_Calculator
{
	class IPAddress
	{
		int addr1;
        int addr2;
        int addr3;
        int addr4;
		string addr1binStr;
        string addr2binStr;
        string addr3binStr;
        string addr4binStr;

		public int subnetmask { get; set; }

		public int Addr1 {
			get { return addr1; }
			set { 
				addr1 = value;
				addr1binStr = toBinaryStr(value);
			}
		}
		public int Addr2
		{
			get { return addr2; }
			set
			{
				addr2 = value;
				addr2binStr = toBinaryStr(value);
			}
		}
		public int Addr3
		{
			get { return addr3; }
			set
			{
				addr3 = value;
				addr3binStr = toBinaryStr(value);
			}
		}
		public int Addr4
		{
			get { return addr4; }
			set
			{
				addr4 = value;
				addr4binStr = toBinaryStr(value);
			}
		}
		

		public string Addr1binStr {
			get { return addr1binStr; }
			set
			{
				addr1binStr = value;
				addr1 = toDecimal(value);
			}
		}
		public string Addr2binStr
		{
			get { return addr2binStr; }
			set
			{
				addr2binStr = value;
				addr2 = toDecimal(value);
			}
		}
		public string Addr3binStr
		{
			get { return addr3binStr; }
			set
			{
				addr3binStr = value;
				addr3 = toDecimal(value);
			}
		}
		public string Addr4binStr
		{
			get { return addr4binStr; }
			set
			{
				addr4binStr = value;
				addr4 = toDecimal(value);
			}
		}

		public IPAddress()
		{
			addr1 = addr2 = addr3 = addr4 = subnetmask = 0;
			addr1binStr = addr2binStr = addr3binStr = addr4binStr = "";
		}

		public IPAddress(int a, int b, int c, int d, int e = 0)
		{
			addr1 = a;
			addr2 = b;
			addr3 = c;
			addr4 = d;
			subnetmask = e;
			addr1binStr = toBinaryStr(a);
			addr2binStr = toBinaryStr(b);
			addr3binStr = toBinaryStr(c);
			addr4binStr = toBinaryStr(d);
		}

		public IPAddress(string a, string b, string c, string d)
		{
			addr1binStr = a;
			addr2binStr = b;
			addr3binStr = c;
			addr4binStr = d;

			addr1 = toDecimal(a);
			addr2 = toDecimal(b);
			addr3 = toDecimal(c);
			addr4 = toDecimal(d);
		}

		public void addOne()
		{
			addr4++;
			if (addr4 == 256)
			{
				addr3++;
				addr4 = 0;
			}
			if (addr3 == 256)
			{
				addr2++;
				addr3 = 0;
			}
			if (addr2 == 256)
			{
				addr1++;
				addr2 = 0;
			}
		}

		public void makeBroadcast()
		{
			int temp1 = 0; int temp2 = 0; int temp3 = 0; int temp4 = 0;
			if (0 < subnetmask && subnetmask < 8)
			{
				int two = 1;
				for (int i = 0; i < 8 - subnetmask; i++)
				{
					temp1 += two;
					two *= 2;
				}
				temp2 = temp3 = temp4 = 255;

				addr1 |= temp1;
				addr2 |= temp2;
				addr3 |= temp3;
				addr4 |= temp4;
			}
			else if (8 <= subnetmask && subnetmask < 16)
			{
				int two = 1;
				for (int i = 0; i < 16 - subnetmask; i++)
				{
					temp2 += two;
					two *= 2;
				}
				temp3 = temp4 = 255;

				addr1 |= temp1;
				addr2 |= temp2;
				addr3 |= temp3;
				addr4 |= temp4;
			}
			else if (16 <= subnetmask && subnetmask < 24)
			{
				int two = 1;
				for (int i = 0; i < 24 - subnetmask; i++)
				{
					temp3 += two;
					two *= 2;
				}
				temp4 = 255;

				addr1 |= temp1;
				addr2 |= temp2;
				addr3 |= temp3;
				addr4 |= temp4;
			}
			else if (24 <= subnetmask)
			{
				int two = 1;
				for (int i = 0; i < 32 - subnetmask; i++)
				{
					temp4 += two;
					two *= 2;
				}

				addr1 |= temp1;
				addr2 |= temp2;
				addr3 |= temp3;
				addr4 |= temp4;
			}
		}

		string toBinaryStr(int n)
		{
			string ret = "";
			int count = 0;
			while (n > 0)
			{
				int a = n % 2;
				n /= 2;

				ret = a.ToString() + ret;
				count++;
			}
			for (int i = 0; i < 8 - count; i++)
			{
				ret = "0" + ret;
			}
			return ret;
		}

		int toDecimal(string str)
		{
			int ret = 0;
			int two = 1;
			for (int i = 7; i >= 0; i--)
			{
				if (str.Substring(i, 1) == "1")
				{
					ret += two;
				}
				two *= 2;
			}
			return ret;
		}
	}
}
