using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VLSM_Calculator
{
	public partial class Form1 : Form
	{
		string a1         = "";
		string a2         = "";
		string a3         = "";
		string a4         = "";
		string maskNumber = "";
		int    mask;
		int    pool = 0;
		string result     = "";
		string filePath   = "";
		string newLine    = "\r\n";
		string bar = "*****************************************";
		string M1 = ""; string MM1 = "";
		string M2 = ""; string MM2 = "";
		string M3 = ""; string MM3 = "";
		string M4 = ""; string MM4 = "";
		IPAddress lastAddress;

		int numberOfSubnet = 0;				//	# of subnets requested by the user. (i.e. # of checks)
		string[] sNames;					//	names of hosts
		int[]    hNumbers;					//	# of hosts requested by the user <= "# of Host(s)" textbox
		int[]    rNumbers;					//	# of routable addresses for each subnet
		int totalNumOfRoutableAddress = 0;	//	Total # of routable addresses

		CheckBox[]      checkBoxes;
		TextBox[]       subnetNames;
		NumericUpDown[] hostNumbers;
		GroupBox[]      modes;
		RadioButton[]   min;
		RadioButton[]   max;
		RadioButton[]   bal;
		TextBox[]       routableAddresses;

		public Form1()
		{
			InitializeComponent();
		}
		private int canWeStart()
		{
			int ret = 0;

			int compare = 0;
			int temp1 = Convert.ToInt32(a1);
			int temp2 = Convert.ToInt32(a2);
			int temp3 = Convert.ToInt32(a3);
			int temp4 = Convert.ToInt32(a4);
			int tempMask = Convert.ToInt32(maskNumber);

			if (0 < tempMask && tempMask < 8)
			{
				if (temp2 != 0 || temp3 != 0 || temp4 != 0)
				{
					ret = 1;
				}
				else
				{
					int two = 128;
					for (int i = 0; i < tempMask; i++)
					{
						compare += two;
						two /= 2;
					}

					if (((~compare) & temp1) != 0)
					{
						ret = 1;
					}
				}
			}
			else if (8 <= tempMask && tempMask < 16)
			{
				if (temp3 != 0 || temp4 != 0)
				{
					ret = 2;
				}
				else
				{
					int two = 128;
					for (int i = 0; i < tempMask - 8; i++)
					{
						compare += two;
						two /= 2;
					}

					if (((~compare) & temp2) != 0)
					{
						ret = 2;
					}
				}
			}
			else if (16 <= tempMask && tempMask < 24)
			{
				if (temp4 != 0)
				{
					ret = 3;
				}
				else
				{
					int two = 128;
					for (int i = 0; i < tempMask-16; i++)
					{
						compare += two;
						two /= 2;
					}

					if (((~compare) & temp3) != 0)
					{
						ret = 3;
					}
				}
			}
			else if (24 <= tempMask && tempMask < 32)
			{
				int two = 128;
				for (int i = 0; i < tempMask - 24; i++)
				{
					compare += two;
					two /= 2;
				}

				if (((~compare) & temp4) != 0)
				{
					ret = 4;
				}
			}

			return ret;
		}

		private void buttonCalculate_Click(object sender, EventArgs e)
		{
			bool passed = true;

			passed  = readIpAddress(textBoxA1,   ref a1,   0, 255);
			passed &= readIpAddress(textBoxA2,   ref a2,   0, 255);
			passed &= readIpAddress(textBoxA3,   ref a3,   0, 255);
			passed &= readIpAddress(textBoxA4,   ref a4,   0, 255);
			passed &= readIpAddress(textBoxMask, ref maskNumber, 0, 32);

			if (passed)
			{
				int errorNum = canWeStart();
				if (errorNum != 0)
				{
					passed = false;
					switch (errorNum)
					{
						case 1:
							makeRedBox(textBoxA1);
							makeRedBox(textBoxA2);
							makeRedBox(textBoxA3);
							makeRedBox(textBoxA4);
							break;
						case 2:
							makeRedBox(textBoxA2);
							makeRedBox(textBoxA3);
							makeRedBox(textBoxA4);
							break;
						case 3:
							makeRedBox(textBoxA3);
							makeRedBox(textBoxA4);
							break;
						case 4:
							makeRedBox(textBoxA4);
							break;
					}
				}
			}

			if (passed)
			{
				textBoxB1.Text = convertToBinary(Convert.ToInt32(a1));
				textBoxB2.Text = convertToBinary(Convert.ToInt32(a2));
				textBoxB3.Text = convertToBinary(Convert.ToInt32(a3));
				textBoxB4.Text = convertToBinary(Convert.ToInt32(a4));

				calculateMask(Convert.ToInt32(maskNumber));
				pool = calculatePool(Convert.ToInt32(maskNumber));
				remainingPool.Text = pool.ToString();

				totalNumOfRoutableAddress = 0;
				numberOfSubnet = 0;
				for (int i = 0; i < 8; i++)
				{
					rNumbers[i] = 0;
					if (checkBoxes[i].Checked)
						numberOfSubnet = i + 1;
				}

				for (int i = 0; i < numberOfSubnet; i++)
				{
					if (subnetNames[i].Text.Trim() == "")
					{
						passed = false;
						subnetNames[i].BackColor = Color.LightPink;
					}
					else
					{
						sNames[i] = subnetNames[i].Text.Trim();
					}
					hNumbers[i] = (int)hostNumbers[i].Value;
				}
			}

			if (passed)
			{
				if (!isPossible())
				{
					passed = false;
					MessageBox.Show("Not enough pool! Please change subnet options.");
				}
				//string str = textBoxB1.Text + textBoxB2.Text + textBoxB3.Text + textBoxB4.Text;
				lastAddress = new IPAddress(Convert.ToInt32(a1), Convert.ToInt32(a2), Convert.ToInt32(a3), Convert.ToInt32(a4), mask);
			}

			if (passed)
			{
				result = "VLSM Calculator" + newLine;
				result += "Made by Soonil Hong" + newLine;
				result += "s.hong.5181@gmail.com" + newLine;
				result += newLine + bar + newLine + newLine;
				result += "IP address :" + newLine;
				result += a1 + " . " + a2 + " . " + a3 + " . " + a4 + " / " + maskNumber + newLine;
				result += M1 + " . " + M2 + " . " + M3 + " . " + M4 + newLine;
				result += MM1 + " . " + MM2 + " . " + MM3 + " . " + MM4 + newLine;
				result += newLine + bar + newLine;

				calculateRoutableSubnet();

				recordResult();
				
				textBoxResult.Text = result;
			}
		}

		private uint convertBinaryToDecimal(string str)
		{
			uint ret = 0;
			uint two = 1;
			for (int i = 31; i >= 0; i--)
			{
				if (str.Substring(i, 1) == "1")
				{
					ret += two;
				}
				two *= 2;
			}
			return ret;
		}

		private void recordResult()
		{
			int n = numberOfSubnet;
			int[] idx = new int[n];
			sortAllIndex(idx, n);

			for (int i = 0; i < n; i++)
			{
				result += newLine;
				result += "Subnet Name           : " + sNames[idx[i]] + newLine;
				result += "# of requested subnet : " + hNumbers[idx[i]].ToString() + newLine;
				result += "subnet mode           : " + ((min[idx[i]].Checked) ? ("Min") : ((max[idx[i]].Checked) ? ("Max") : ("Bal"))) + newLine;

				calculateSubnet(idx[i]);

				result += newLine + bar + newLine;
			}

			result += newLine + "Calculation done!!" + newLine;

		}

		private void calculateSubnet(int n)
		{
			int howManyMoreBit = calculatePower(rNumbers[n] + 2);
			int subnetMask = 32 - howManyMoreBit;
			lastAddress.subnetmask = subnetMask;

			result += "Net Address            : " + lastAddress.Addr1 + " . " + lastAddress.Addr2 + " . " + lastAddress.Addr3 + " . " + lastAddress.Addr4 + newLine;
			result += "Subnetmask             : " + lastAddress.subnetmask + newLine;
			lastAddress.addOne();
			result += "First routable address : " + lastAddress.Addr1 + " . " + lastAddress.Addr2 + " . " + lastAddress.Addr3 + " . " + lastAddress.Addr4 + newLine;
			//lastAddress.makeLast();
			lastAddress.makeBroadcast();
			result += "Last routable address  : " + lastAddress.Addr1 + " . " + lastAddress.Addr2 + " . " + lastAddress.Addr3 + " . " + (lastAddress.Addr4-1) + newLine;
			result += "Broadcasd address      : " + lastAddress.Addr1 + " . " + lastAddress.Addr2 + " . " + lastAddress.Addr3 + " . " + lastAddress.Addr4 + newLine;
			result += "Number of routable address(es) : " + rNumbers[n] + newLine;
			lastAddress.addOne();
		}

		private int calculatePower(int n)
		{
			int ret = 0;
			while (n != 0)
			{
				ret++;
				n /= 2;
			}
			return (ret - 1);
		}

		private void sortAllIndex(int[] idx, int n)
		{
			int[,] temp = new int[2, n];
			int count = 0;

			for (int i = 0; i < n; i++)
			{
				temp[0, count] = rNumbers[i];
				temp[1, count] = i;
				count++;
			}

			for (int i = 0; i < n - 1; i++)
			{
				for (int j = 0; j < n - i - 1; j++)
				{
					if (temp[0, j] < temp[0, j + 1])
					{
						int t = temp[0, j];
						temp[0, j] = temp[0, j + 1];
						temp[0, j + 1] = t;

						t = temp[1, j];
						temp[1, j] = temp[1, j + 1];
						temp[1, j + 1] = t;
					}
				}
			}

			for (int i = 0; i < n; i++)
			{
				idx[i] = temp[1, i];
			}
		}

		private void calculateRoutableSubnet()
		{
			bool hasBal         = hasBalanced();
			int  numOfMin       = 0;
			int  numOfMaxAndBal = 0;
			totalNumOfRoutableAddress = 0;

			for (int i = 0; i < 8; i++)
			{
				routableAddresses[i].Text = "";
				rNumbers[i] = 0;
			}

			for (int i = 0; i < numberOfSubnet; i++)
			{
				int temp = makeHostNum(hNumbers[i]);
				pool -= temp;
				remainingPool.Text = pool.ToString();
				rNumbers[i] = temp - 2; ;
				routableAddresses[i].Text = (rNumbers[i]).ToString();

				if (min[i].Checked)
				{
					numOfMin++;
				}
				else
				{
					numOfMaxAndBal++;
				}
			}

			//	now.. numOfMin       == # of subnets with "Min" option.
			//	now.. numOfMaxAndBal == # of subnets with "Max" or "Bal"

			int[] sortedIdx = new int[numOfMaxAndBal];
			sortIndex(sortedIdx, numOfMaxAndBal);
			int[] balancedIdx = new int[numOfMaxAndBal];
			balanceIndex(balancedIdx, numOfMaxAndBal);

			if (hasBal)	//	there is at least one "Bal"
			{
				//	numOfMaxAndBal => number of "Bal" because "Max" is considered as "Bal" in this case.
				
				makeItBal(balancedIdx, numOfMaxAndBal);
			}
			else // there is no "Bal". The others are all "Max".
			{
				//	numOfMaxAndBal => number of "Max" because there is no "Bal"
				for (int i = 0; i < numOfMaxAndBal; i++)
				{
					makeItMax(sortedIdx[i]);
				}
			}

			for (int i = 0; i < numberOfSubnet; i++)
			{
				totalNumOfRoutableAddress += rNumbers[i];
				totalRoutableAddress.Text = totalNumOfRoutableAddress.ToString();
			}
		}

		private void makeItBal(int[] arr, int n)
		{
			bool keepgoing = true;
			while (keepgoing && !areSame(arr, n))
			{
				keepgoing = tryToRaise(arr[0]);
				balanceIndex(arr, n);
			}
			if (areSame(arr, n))
			{
				while (tryToRaiseTogether(arr, n))
				{
					// do nothing in here
				}
				
			}
		}

		private bool tryToRaiseTogether(int[] arr, int n)
		{
			bool ret = false;
			int temp = rNumbers[arr[0]] + 2;
			if (pool >= temp * n)
			{
				for (int i = 0; i < n; i++)
				{
					pool -= temp;
					remainingPool.Text = pool.ToString();
					rNumbers[arr[i]] = (temp * 2) - 2;
					routableAddresses[arr[i]].Text = rNumbers[arr[i]].ToString();
				}
				ret = true;
			}
			return ret;
		}

		private bool tryToRaise(int n)
		{
			bool ret = false;
			int temp = rNumbers[n] + 2;
			if (pool >= temp)
			{
				pool -= temp;
				temp *= 2;
				remainingPool.Text = pool.ToString();
				rNumbers[n] = temp - 2;
				routableAddresses[n].Text = rNumbers[n].ToString();
				ret = true;
			}
			return ret;
		}

		private bool areSame(int[] arr, int n)
		{
			bool ret = true;
			for (int i = 0; ret && i < n - 1; i++)
			{
				if (rNumbers[arr[i]] != rNumbers[arr[i + 1]])
					ret = false;
			}
			return ret;
		}

		private void makeItMax(int n)
		{
			int temp = rNumbers[n] + 2;
			while (pool >= temp)
			{
				pool -= temp;
				temp *= 2;
			}
			remainingPool.Text = pool.ToString();
			rNumbers[n] = temp - 2;
			routableAddresses[n].Text = rNumbers[n].ToString();
		}

		private void balanceIndex(int[] balancedIdx, int n)
		{
			int[,] temp = new int[2, n];
			int count = 0;

			for (int i = 0; i < numberOfSubnet; i++)
			{
				if (!min[i].Checked)
				{
					temp[0, count] = rNumbers[i];
					temp[1, count] = i;
					count++;
				}
			}

			for (int i = 0; i < n - 1; i++)
			{
				for (int j = 0; j < n - i - 1; j++)
				{
					if (temp[0, j] > temp[0, j + 1])
					{
						int t = temp[0, j];
						temp[0, j] = temp[0, j + 1];
						temp[0, j + 1] = t;

						t = temp[1, j];
						temp[1, j] = temp[1, j + 1];
						temp[1, j + 1] = t;
					}
				}
			}

			for (int i = 0; i < n; i++)
			{
				balancedIdx[i] = temp[1, i];
			}
		}

		private void sortIndex(int[] sortedIdx, int n)
		{
			int[,] temp = new int[2, n];
			int count = 0;

			for (int i = 0; i < numberOfSubnet; i++)
			{
				if (!min[i].Checked)
				{
					temp[0, count] = hNumbers[i];
					temp[1, count] = i;
					count++;
				}
			}

			for (int i = 0; i < n - 1; i++)
			{
				for (int j = 0; j < n - i - 1; j++)
				{
					if(temp[0,j] < temp[0,j+1])
					{
						int t = temp[0, j];
						temp[0, j] = temp[0, j + 1];
						temp[0, j + 1] = t;

						t = temp[1, j];
						temp[1, j] = temp[1, j + 1];
						temp[1, j + 1] = t;
					}
				}
			}
			for (int i = 0; i < n; i++)
			{
				sortedIdx[i] = temp[1, i];
			}
		}

		private bool isPossible()
		{
			int total = 0;

			for (int i = 0; i < numberOfSubnet; i++)
			{
				total += makeHostNum(hNumbers[i]);
			}

			return (total <= pool);
		}

		private int makeHostNum(int n)
		{
			n += 3;		//	including gateway, net address, and broadcasting address
			int ret = 1;

			while (n > ret)
			{
				ret *= 2;
			}

			return ret;
		}

		private bool hasBalanced()
		{
			bool ret = false;
			for (int i = 0; i < 8; i++)
			{
				if (bal[i].Checked)
					ret = true;
			}
			return ret;
		}

		private int calculatePool(int maskNumber)
		{
			int ret = 32 - maskNumber;

			ret = PowOfTwo(ret) - 2;

			return ret;
		}

		private int PowOfTwo(int n)
		{
			int ret = 1;
			for (int i = 0; i < n; i++)
			{
				ret *= 2;
			}
			return ret;
		}

		private bool readIpAddress(TextBox tb, ref string addr, int min, int max)
		{
			bool ret = true;

			try
			{
				int ipAddr = Convert.ToInt32(tb.Text);
				if (min <= ipAddr && ipAddr <= max)
				{
					addr = ipAddr.ToString();
				}
				else
				{
					throw new Exception("IP address is not valid!!");
				}
			}
			catch
			{
				tb.BackColor = Color.LightPink;
				ret = false;
			}

			return ret;
		}

		private void makeRedBox(TextBox tb)
		{
			tb.BackColor = Color.LightPink;
		}

		private string convertToBinary(int n)
		{
			string str = "";

			for (int i = 0; i < 8; i++)
			{
				if ((n & 1) == 1)
					str = "1" + str;
				else
					str = "0" + str;

				n /= 2;
			}

			return str;
		}

		private void calculateMask(int mask)
		{
			this.mask = mask;
			string str = "";
			for (int i = 0; i < mask; i++)
			{
				str += "1";
			}
			for (int i = 0; i < 32 - mask; i++)
			{
				str += "0";
			}

			MM1 = textBoxSS1.Text = str.Substring(0, 8);
			MM2 = textBoxSS2.Text = str.Substring(8, 8);
			MM3 = textBoxSS3.Text = str.Substring(16, 8);
			MM4 = textBoxSS4.Text = str.Substring(24, 8);

			M1 = textBoxS1.Text = convertToDecimal(Convert.ToInt32(textBoxSS1.Text));
			M2 = textBoxS2.Text = convertToDecimal(Convert.ToInt32(textBoxSS2.Text));
			M3 = textBoxS3.Text = convertToDecimal(Convert.ToInt32(textBoxSS3.Text));
			M4 = textBoxS4.Text = convertToDecimal(Convert.ToInt32(textBoxSS4.Text));
		}

		private string convertToDecimal(int n)
		{
			int ret = 0;
			int power = 1;

			do {
				int lastDigit = n % 10;
				ret += (lastDigit * power);
				n /= 10;
				power *= 2;
			} while(n != 0);

			return ret.ToString();
		}

		private void manageCheckBoxes(int n)
		{
			if (checkBoxes[n].Checked)
			{
				enableControl(subnetNames[n], hostNumbers[n], modes[n]);
				if (n != 7)
					checkBoxes[n + 1].Enabled = true;
			}
			else
			{
				min[n].Checked = true;
				rNumbers[n] = 0;
				routableAddresses[n].Text = "";
				totalRoutableAddress.Text = "";
				disableControl(subnetNames[n], hostNumbers[n], modes[n]);
				for (int i = n + 1; i < 8; i++)
				{
					checkBoxes[i].Checked = false;
					checkBoxes[i].Enabled = false;
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////
		///////////////////////////////////////////////////////////////////////////
		///////////////////////////////////////////////////////////////////////////

		private void Form1_Load(object sender, EventArgs e)
		{
			checkBoxes  = new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8 };
			subnetNames = new TextBox[] { textBox_subnetName1, textBox_subnetName2, textBox_subnetName3, textBox_subnetName4, textBox_subnetName5, textBox_subnetName6, textBox_subnetName7, textBox_subnetName8 };
			hostNumbers = new NumericUpDown[] { numberOfHost1, numberOfHost2, numberOfHost3, numberOfHost4, numberOfHost5, numberOfHost6, numberOfHost7, numberOfHost8 };
			modes       = new GroupBox[] { groupBox1, groupBox2, groupBox3, groupBox4, groupBox5, groupBox6, groupBox7, groupBox8 };
			min         = new RadioButton[] { Min1, Min2, Min3, Min4, Min5, Min6, Min7, Min8 };
			max         = new RadioButton[] { Max1, Max2, Max3, Max4, Max5, Max6, Max7, Max8 };
			bal         = new RadioButton[] { Bal1, Bal2, Bal3, Bal4, Bal5, Bal6, Bal7, Bal8 };
			routableAddresses = new TextBox[] { numRoutable1, numRoutable2, numRoutable3, numRoutable4, numRoutable5, numRoutable6, numRoutable7, numRoutable8 };
			sNames      = new string[8];
			hNumbers    = new int[8];
			rNumbers    = new int[8];
		}

		private void buttonReset_Click(object sender, EventArgs e)
		{
			textBoxA1.Text = "";
			textBoxA2.Text = "";
			textBoxA3.Text = "";
			textBoxA4.Text = "";
			textBoxMask.Text = "";

			textBoxA1.BackColor = Color.White;
			textBoxA2.BackColor = Color.White;
			textBoxA3.BackColor = Color.White;
			textBoxA4.BackColor = Color.White;
			textBoxMask.BackColor = Color.White;

			textBoxResult.Text = "";

			textBoxB1.Text = "";
			textBoxB2.Text = "";
			textBoxB3.Text = "";
			textBoxB4.Text = "";

			textBoxS1.Text = "";
			textBoxS2.Text = "";
			textBoxS3.Text = "";
			textBoxS4.Text = "";

			textBoxSS1.Text = "";
			textBoxSS2.Text = "";
			textBoxSS3.Text = "";
			textBoxSS4.Text = "";

			a1 = "";
			a2 = "";
			a3 = "";
			a4 = "";
			maskNumber = "";
			result = "";
			filePath = "";

			remainingPool.Text = "";
			totalRoutableAddress.Text = "";

			checkBox1.Checked = false;
		}

		private void buttonSaveAs_Click(object sender, EventArgs e)
		{
			filePath = "";
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Text File|*.txt";
			sfd.FileName = "subnet";
			sfd.Title = "VLSM Calculator";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				filePath = sfd.FileName;
				StreamWriter sw = new StreamWriter(File.Create(filePath));
				//sw.Write(result);
				sw.Write(textBoxResult.Text);
				sw.Dispose();
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			manageCheckBoxes(0);
			if (!checkBox1.Checked)
			{
				remainingPool.Text = "";
			}
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			manageCheckBoxes(1);
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			manageCheckBoxes(2);
		}

		private void checkBox4_CheckedChanged(object sender, EventArgs e)
		{
			manageCheckBoxes(3);
		}

		private void checkBox5_CheckedChanged(object sender, EventArgs e)
		{
			manageCheckBoxes(4);
		}

		private void checkBox6_CheckedChanged(object sender, EventArgs e)
		{
			manageCheckBoxes(5);
		}

		private void checkBox7_CheckedChanged(object sender, EventArgs e)
		{
			manageCheckBoxes(6);
		}

		private void checkBox8_CheckedChanged(object sender, EventArgs e)
		{
			manageCheckBoxes(7);
		}

		private void enableControl(TextBox tb, NumericUpDown nud, GroupBox gb)
		{
			tb.Enabled = true;
			tb.BackColor = Color.White;
			nud.Enabled = true;
			nud.BackColor = Color.White;
			gb.Enabled = true;
		}

		private void disableControl(TextBox tb, NumericUpDown nud, GroupBox gb)
		{
			tb.Enabled = false;
			tb.BackColor = Color.FromKnownColor(KnownColor.Control);
			nud.Enabled = false;
			nud.BackColor = Color.FromKnownColor(KnownColor.Control);
			gb.Enabled = false;
			gb.BackColor = Color.FromKnownColor(KnownColor.Control);
		}

		private void textBoxMask_Enter(object sender, EventArgs e)
		{
			textBoxMask.BackColor = Color.White;
			textBoxA1.BackColor = Color.White;
			textBoxA2.BackColor = Color.White;
			textBoxA3.BackColor = Color.White;
			textBoxA4.BackColor = Color.White;
		}

		private void textBoxA1_Enter(object sender, EventArgs e)
		{
			textBoxMask.BackColor = Color.White;
			textBoxA1.BackColor = Color.White;
			textBoxA2.BackColor = Color.White;
			textBoxA3.BackColor = Color.White;
			textBoxA4.BackColor = Color.White;
		}

		private void textBoxA2_Enter(object sender, EventArgs e)
		{
			textBoxMask.BackColor = Color.White;
			textBoxA1.BackColor = Color.White;
			textBoxA2.BackColor = Color.White;
			textBoxA3.BackColor = Color.White;
			textBoxA4.BackColor = Color.White;
		}

		private void textBoxA3_Enter(object sender, EventArgs e)
		{
			textBoxMask.BackColor = Color.White;
			textBoxA1.BackColor = Color.White;
			textBoxA2.BackColor = Color.White;
			textBoxA3.BackColor = Color.White;
			textBoxA4.BackColor = Color.White;
		}

		private void textBoxA4_Enter(object sender, EventArgs e)
		{
			textBoxMask.BackColor = Color.White;
			textBoxA1.BackColor = Color.White;
			textBoxA2.BackColor = Color.White;
			textBoxA3.BackColor = Color.White;
			textBoxA4.BackColor = Color.White;
		}

		private void textBox_subnetName1_Enter(object sender, EventArgs e)
		{
			textBox_subnetName1.BackColor = Color.White;
		}

		private void textBox_subnetName2_Enter(object sender, EventArgs e)
		{
			textBox_subnetName2.BackColor = Color.White;
		}

		private void textBox_subnetName3_Enter(object sender, EventArgs e)
		{
			textBox_subnetName3.BackColor = Color.White;
		}

		private void textBox_subnetName4_Enter(object sender, EventArgs e)
		{
			textBox_subnetName4.BackColor = Color.White;
		}

		private void textBox_subnetName5_Enter(object sender, EventArgs e)
		{
			textBox_subnetName5.BackColor = Color.White;
		}

		private void textBox_subnetName6_Enter(object sender, EventArgs e)
		{
			textBox_subnetName6.BackColor = Color.White;
		}

		private void textBox_subnetName7_Enter(object sender, EventArgs e)
		{
			textBox_subnetName7.BackColor = Color.White;
		}

		private void textBox_subnetName8_Enter(object sender, EventArgs e)
		{
			textBox_subnetName8.BackColor = Color.White;
		}

	}
}
