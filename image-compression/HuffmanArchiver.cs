using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace image_compression
{
	public class HuffmanArchiver
	{
		/// <summary>
		/// Бинарное дерево.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		[Serializable]
		public class BinaryTree<T>
		{
			public BinaryTreeNode<T> Root { get; set; }

			public BinaryTree(BinaryTreeNode<T> root) => Root = root;
		}

		/// <summary>
		/// Узел бинарного дерева.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		[Serializable]
		public class BinaryTreeNode<T>
		{
			public BinaryTreeNode<T> LeftChild { get; set; }
			public BinaryTreeNode<T> RightChild { get; set; }
			public T Value { get; set; }

			public BinaryTreeNode(T value) => Value = value;

			public BinaryTreeNode(T value, BinaryTreeNode<T> leftChild, BinaryTreeNode<T> rightChild)
			{
				Value = value;
				LeftChild = leftChild;
				RightChild = rightChild;
			}
		}

		/// <summary>
		/// Таблица: число - вероятность числа.
		/// </summary>
		private Dictionary<int, double> _numberTable;

		/// <summary>
		/// Закодированная таблица: число - битовый код числа.
		/// </summary>
		private Dictionary<int, string> _codeTable;

		/// <summary>
		/// Дерево Хаффмана.
		/// </summary>
		private BinaryTree<string> _huffmanTree;

		private const string UncodedImageFile = "uncodedImage.txt";
		private const string EncodedImageFile = "encodedImage.txt";
		private const string DecodedImageFile = "decodedImage.txt";

		public HuffmanArchiver(ComplexMatrix matrix)
		{
			WriteToABinaryFile(matrix);
			EncodeFile();
			//DecodeFile();
		}

		private void WriteToABinaryFile(ComplexMatrix matrix)
		{
			using var writer = new BinaryWriter(File.Open(UncodedImageFile, FileMode.Create), Encoding.Default);
			for (var i = 0; i < matrix.Width; i++)
				for (var j = 0; j < matrix.Height; j++)
					writer.Write((int)matrix.Matrix[i][j].Real);
		}

		/// <summary>
		/// Создание таблицы: символ - вероятность
		/// </summary>
		/// <returns></returns>
		private Dictionary<int, double> CreateProbabilityTable()
		{
			if (!File.Exists(UncodedImageFile))
				throw new FileNotFoundException();

			var symbolCount = 0;
			// Первичная таблица: число - количество чисел.
			var first = new Dictionary<int, int>();
			// Вторичная таблица: int - вероятность числа.
			var second = new Dictionary<int, double>();
			// Чтение файла.
			int temp;
			using (var reader = new BinaryReader(File.Open(UncodedImageFile, FileMode.Open), Encoding.Default))
				while ((temp = reader.Read()) > -1)
				{
					symbolCount++;
					if (!first.ContainsKey(temp))
						first.Add(temp, 1);
					else first[temp]++;
				}

			foreach (var i in first)
				second.Add(i.Key, (double)i.Value / symbolCount);

			return second;
		}

		/// <summary>
		/// Создаёт бинарное дерево Хаффмана
		/// </summary>
		/// <returns></returns>
		private BinaryTree<string> CreateHuffmanTree()
		{
			var binaryTree = new Dictionary<string, double>();
			foreach (var i in _numberTable)
				binaryTree.Add(Convert.ToString(i.Key, 2).PadLeft(32, '0'), i.Value);

			var nodeList = new Dictionary<string, BinaryTreeNode<string>>();
			foreach (var i in binaryTree.Keys)
				nodeList.Add(i, new BinaryTreeNode<string>(i));

			while (binaryTree.Count > 1)
			{
				// Отбирает пару символов с наименьшей вероятностью.
				var helpmass = binaryTree.OrderByDescending(pair => pair.Value).Reverse().Take(2).ToArray();
				// Скрепляет эту пару символов, тем самым создаёт узел
				string s1 = helpmass[0].Key, s2 = helpmass[1].Key, s3 = string.Concat(s1, s2);
				double d1 = helpmass[0].Value, d2 = helpmass[1].Value;
				binaryTree.Remove(s1);
				binaryTree.Remove(s2);
				binaryTree.Add(s3, d1 + d2);

				nodeList.TryGetValue(s1, out var leftChild);
				nodeList.TryGetValue(s2, out var rightChild);
				nodeList.Remove(leftChild.Value);
				nodeList.Remove(rightChild.Value);
				nodeList.Add(s3, new BinaryTreeNode<string>(s3, leftChild, rightChild));
			}

			nodeList.TryGetValue(binaryTree.ElementAt(0).Key, out var root);

			return new BinaryTree<string>(root);
		}

		/// <summary>
		/// Создание кодовой таблицы
		/// </summary>
		/// <returns></returns>
		private Dictionary<int, string> CreateCodeTable()
		{
			var codeTable = new Dictionary<int, string>();

			foreach (var i in _numberTable.Keys.ToList())
			{
				var code = new StringBuilder();
				var temp = _huffmanTree.Root;
				while (temp.Value.Length > 1)
				{
					List<string> codes = new List<string>();
					if (temp.LeftChild != null)
						for (var j = 0; j < temp.LeftChild.Value.Length / 32; j++)
							codes.Add(temp.LeftChild.Value.Substring(j * 32, 32));

					if (temp.LeftChild != null && codes.Contains(Convert.ToString(i, 2).PadLeft(32, '0')))
					{
						code.Append("0");
						temp ??= temp.LeftChild;
					}
					else if (temp.RightChild != null)
					{
						code.Append("1");
						temp = temp.RightChild;
					}
					else break;
				}

				codeTable.Add(i, code.ToString());
			}

			return codeTable;
		}

		/// <summary>
		/// Кодирование файла.
		/// </summary>
		/// <param name="bufferSize"></param>
		private void EncodeFile(int bufferSize = 4)
		{
			if (!File.Exists(UncodedImageFile))
				throw new FileNotFoundException();

			var binaryCode = new StringBuilder();
			_numberTable = CreateProbabilityTable();
			_huffmanTree = CreateHuffmanTree();
			_codeTable = CreateCodeTable();

			int temp;
			using var reader = new BinaryReader(File.Open(UncodedImageFile, FileMode.Open), Encoding.Default);
			using var writer = new BinaryWriter(File.Open(EncodedImageFile, FileMode.Create), Encoding.Default);

			// Сохраняем дерево.
			var saveTree = new BinaryFormatter();
			saveTree.Serialize(writer.BaseStream, _huffmanTree);

			while ((temp = reader.Read()) > -1)
			{
				_codeTable.TryGetValue(temp, out var code);
				binaryCode.Append(code);
				if (binaryCode.Length > bufferSize * 32)
				{
					var ints = StringToInts(binaryCode.ToString().Substring(0, bufferSize * 32));
					binaryCode.Remove(0, bufferSize * 32);
					foreach (var i in ints)
						writer.Write(i);
				}
			}

			if (binaryCode.Length != 0)
			{
				var bytes = StringToInts(binaryCode.ToString().PadRight(bufferSize * 32, '0'));
				foreach (var i in bytes)
					writer.Write(i);
			}
		}

		/// <summary>
		/// Декодирование файла, закодированный с помощью алгоритма Хаффмана.
		/// </summary>
		/// <param name="readBufferSize"></param>
		/// <param name="writeBuffersize"></param>
		private void DecodeFile(int readBufferSize = 8, int writeBuffersize = 10)
		{
			if (!File.Exists(EncodedImageFile))
				throw new FileNotFoundException();

			var decodedStr = new StringBuilder();
			var binaryCode = new StringBuilder();

			using var reader = new BinaryReader(File.Open(EncodedImageFile, FileMode.Open), Encoding.Default);
			using var writer = new BinaryWriter(File.Open(DecodedImageFile, FileMode.Create), Encoding.Default);

			// Загружаем сохраненное дерево Хаффмана.
			var saveTree = new BinaryFormatter();
			_huffmanTree = (BinaryTree<string>)saveTree.Deserialize(reader.BaseStream);

			while (reader.BaseStream.Position != reader.BaseStream.Length)
			{
				var buffer = new List<int>();
				var currPos = reader.BaseStream.Position;
				for (var i = currPos; i < currPos + readBufferSize; i++)
					buffer.Add(reader.ReadInt32());

				foreach (var i in buffer)
					binaryCode.Append(Convert.ToString(i, 2));

				var temp = binaryCode.ToString();
				while (temp.Length >= readBufferSize)
				{
					try
					{
						// Error!!!
						decodedStr.Append(DecodeSymbol(ref temp));
						binaryCode = new StringBuilder(temp);
					}
					catch (Exception)
					{
						break;
					}

					if (decodedStr.Length >= writeBuffersize)
					{
						var writeBuffer = Encoding.Default.GetBytes(decodedStr.ToString());
						writer.Write(writeBuffer, 0, writeBuffer.Length);
						decodedStr.Remove(0, writeBuffersize);
					}
				}
			}

			var writeBuffer2 = Encoding.Default.GetBytes(decodedStr.ToString());
			writer.Write(writeBuffer2, 0, writeBuffer2.Length);
		}

		/// <summary>
		/// Декодирование одного символа с помощью данного дерева Хаффмана
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		private char DecodeSymbol(ref string code)
		{
			var node = _huffmanTree.Root;
			var count = 0;
			foreach (var i in code)
			{
				if (node == null || node.Value.Length == 1)
				{
					code = code.Remove(0, count);
					return node.Value.ElementAt(0);
				}

				if (i == '0')
				{
					node = node.LeftChild;
					count++;
				}
				else
				{
					node = node.RightChild;
					count++;
				}
			}

			throw new FileNotFoundException();
		}

		/// <summary>
		/// Преобразовывает строку
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static int[] StringToInts(string str)
		{
			var ints = new int[str.Length / 32];

			for (var i = 0; i < str.Length / 32; i++)
				ints[i] = Convert.ToInt32(str.Substring(i * 32, 32), 2);

			return ints;
		}
	}
}